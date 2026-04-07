using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MCC6DLL;

internal class DataExchange
{
	private enum eDebugLevelType
	{
		eDebugLevInfo,
		eDebugLevDebug,
		eDebugLevWarn,
		eDebugLevErr,
		eDebugLevException
	}

	private enum eCommPortNameType
	{
		eCommPortNull,
		eCommPortRS232,
		eCommPortTCP
	}

	private eCommPortNameType CommPort = eCommPortNameType.eCommPortNull;

	private TcpClient MyClient = new TcpClient();

	private NetworkStream MyStream = null;

	private Semaphore semRevSendSync = new Semaphore(1, 1);

	private SerialPort serlPrt = new SerialPort();

	private byte[] RecvBuf = new byte[4096];

	private RichTextBox MyDebugOutCtrl = null;

	public void DataExchange_SetDebugHandler(RichTextBox DebugOutCtrl)
	{
		MyDebugOutCtrl = DebugOutCtrl;
	}

	private void Fun_PrintInfo(string sInfo)
	{
		if (MyDebugOutCtrl != null)
		{
			((Control)MyDebugOutCtrl).Text = DateTime.Now.ToString() + "\tCOMM INFO\t" + sInfo + Environment.NewLine + ((Control)MyDebugOutCtrl).Text;
		}
	}

	private void fun_PrintfInfo(eDebugLevelType eLev, string sInfo)
	{
		string text = "";
		switch (eLev)
		{
		case eDebugLevelType.eDebugLevInfo:
			text = "INFO\t";
			break;
		case eDebugLevelType.eDebugLevWarn:
			text = "WARN\t";
			break;
		case eDebugLevelType.eDebugLevErr:
			text = "ERROR\t";
			break;
		case eDebugLevelType.eDebugLevException:
			text = "EXCEPT\t";
			break;
		}
		if (MyDebugOutCtrl != null)
		{
			((Control)MyDebugOutCtrl).Text = DateTime.Now.ToString() + "\tCOMM " + text + sInfo + Environment.NewLine + ((Control)MyDebugOutCtrl).Text;
		}
	}

	public bool Fun_CreateLink_TCP(string IPAddr, int nPort)
	{
		bool flag = false;
		try
		{
			MyClient = new TcpClient();
			IPAddress address = IPAddress.Parse(IPAddr);
			MyClient.Connect(address, nPort);
			MyStream = MyClient.GetStream();
			MyStream.ReadTimeout = 200;
			CommPort = eCommPortNameType.eCommPortTCP;
			Fun_PrintInfo("网络连接成功!");
			return true;
		}
		catch (Exception ex)
		{
			Fun_PrintInfo("通讯连接失败，出错信息：" + ex.ToString());
			return false;
		}
	}

	public void Fun_CloseLink_TCP()
	{
		try
		{
			MyClient.Close();
			Fun_PrintInfo("网络连接成功!");
		}
		catch (Exception ex)
		{
			Fun_PrintInfo("通讯连接失败，出错信息：" + ex.ToString());
		}
	}

	public bool Fun_SendBuffer_TCP(byte[] Buffer, int nSendLen, int nRevLen)
	{
		bool flag = false;
		try
		{
			if (semRevSendSync.WaitOne(500))
			{
				MyStream.Write(Buffer, 0, nSendLen);
				MyStream.Flush();
				flag = true;
			}
		}
		catch (Exception)
		{
			flag = false;
		}
		if (!flag)
		{
			semRevSendSync.Release();
		}
		return flag;
	}

	public bool Fun_GetBuffer_TCP(int nLen, byte[] nRevBuf)
	{
		bool result = false;
		int num = 0;
		try
		{
			Thread.Sleep(5);
			num = MyStream.Read(RecvBuf, 0, nLen);
			MyStream.Flush();
			if (num == nLen)
			{
				Array.Copy(RecvBuf, nRevBuf, nLen);
				result = true;
			}
		}
		catch (Exception)
		{
			result = false;
		}
		semRevSendSync.Release();
		return result;
	}

	public bool Fun_CreateLink_RS232(string SerialPortName)
	{
		bool flag = false;
		try
		{
			serlPrt.PortName = SerialPortName;
			if (serlPrt.IsOpen)
			{
				serlPrt.Close();
			}
			serlPrt.ReadTimeout = 200;
			serlPrt.ReceivedBytesThreshold = 1024;
			serlPrt.RtsEnable = true;
			serlPrt.WriteTimeout = 500;
			serlPrt.ReadBufferSize = 1024;
			serlPrt.WriteBufferSize = 1024;
			serlPrt.BaudRate = 115200;
			serlPrt.Parity = (Parity)0;
			serlPrt.DataBits = 8;
			serlPrt.StopBits = (StopBits)1;
			serlPrt.Encoding = Encoding.GetEncoding("Gb2312");
			serlPrt.Open();
			CommPort = eCommPortNameType.eCommPortRS232;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool Fun_CloseLink_RS232()
	{
		serlPrt.Close();
		return true;
	}

	public bool Fun_SendBuffer_RS232(byte[] Buffer, int nSendLen, int nRevLen)
	{
		bool flag = false;
		try
		{
			if (semRevSendSync.WaitOne(500))
			{
				serlPrt.Write(Buffer, 0, nSendLen);
				flag = true;
			}
		}
		catch (Exception)
		{
			flag = false;
		}
		if (!flag)
		{
			semRevSendSync.Release();
		}
		return flag;
	}

	public bool Fun_GetBuffer_RS232(int nLen, byte[] nRevBuf)
	{
		bool flag = false;
		int num = 0;
		int num2 = 0;
		try
		{
			bool flag2 = true;
			while (true)
			{
				Thread.Sleep(5);
				if (serlPrt.BytesToRead >= nLen)
				{
					flag = true;
					break;
				}
				if (num2++ > 100)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				num = serlPrt.Read(RecvBuf, 0, nLen);
				serlPrt.DiscardInBuffer();
				Array.Copy(RecvBuf, nRevBuf, nLen);
			}
		}
		catch (Exception)
		{
			flag = false;
		}
		semRevSendSync.Release();
		return flag;
	}

	public bool Fun_GetBuffer(int nLen, byte[] nRevBuf)
	{
		bool flag = false;
		if (eCommPortNameType.eCommPortRS232 == CommPort)
		{
			return Fun_GetBuffer_RS232(nLen, nRevBuf);
		}
		if (eCommPortNameType.eCommPortTCP == CommPort)
		{
			return Fun_GetBuffer_TCP(nLen, nRevBuf);
		}
		return false;
	}

	public bool Fun_SendBuffer(byte[] Buffer, int nSendLen, int nRevLen)
	{
		bool flag = false;
		if (eCommPortNameType.eCommPortRS232 == CommPort)
		{
			return Fun_SendBuffer_RS232(Buffer, nSendLen, nRevLen);
		}
		if (eCommPortNameType.eCommPortTCP == CommPort)
		{
			return Fun_SendBuffer_TCP(Buffer, nSendLen, nRevLen);
		}
		return false;
	}

	public bool Fun_CloseLink()
	{
		if (eCommPortNameType.eCommPortRS232 == CommPort)
		{
			Fun_CloseLink_RS232();
		}
		else if (eCommPortNameType.eCommPortTCP == CommPort)
		{
			Fun_CloseLink_TCP();
		}
		return true;
	}
}
