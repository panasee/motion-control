using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class ExPosFollow : Form
{
	private enum UartReceiveSttType
	{
		DataArrived,
		NoDataToRead,
		PortNotOpen,
		OtherEx
	}

	private enum UartSendResType
	{
		SendedOk,
		PortNotOpen,
		CheckSumErr,
		SendTimeOut,
		FrameFormatErr,
		ExPosListEmpty
	}

	private struct stExPosType
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public float[] mExPos;
	}

	private struct stCmndSendExPosType
	{
		public byte CheckSum;

		public byte dtLen;

		public byte ValidPosCnt;

		public byte CmndIndx;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public stExPosType[] CmndPos;
	}

	private MainFrm PMainFrm;

	private byte gbAxisEnBit;

	private int gbStartSendIndx;

	private UartReceiveSttType UartReceiveStt;

	private static EventWaitHandle EventSPDataR = new AutoResetEvent(initialState: false);

	private byte[] CommReceiveBuffer = new byte[256];

	private int SpIntrThrdHod = 4;

	private readonly byte FRAMEHEAD = 170;

	private readonly byte FRAMETAIL = 85;

	private byte gbPosCmndSendOkCnt;

	private byte gbBufferCnt;

	private byte gbSendTimes;

	private int testStt;

	private const int constCmndPosBufferSize = 8;

	private List<stExPosType> mListExPos = new List<stExPosType>(256);

	private IContainer components;

	private Button bnLoadExPosFile;

	private Button BnStart;

	private Button BnStop;

	private CheckBox AxisEnC;

	private CheckBox AxisEnB;

	private CheckBox AxisEnA;

	private CheckBox AxisEnZ;

	private CheckBox AxisEnY;

	private CheckBox AxisEnX;

	private Label label1;

	private Label label2;

	private RichTextBox rtbExPosFileInfo;

	private Label label3;

	private Button bnDownExPosFile;

	private Button bnStopDownLoadExPosFile;

	private OpenFileDialog openFileDialog1;

	private ColumnHeader Index;

	private ColumnHeader XPos;

	private ColumnHeader YPos;

	private ColumnHeader ZPos;

	private ColumnHeader APos;

	private ColumnHeader BPos;

	private ColumnHeader CPos;

	private ListView lbExPosInfo;

	private Label label4;

	private Button bn_CloseSp;

	private Button bn_OpenSp;

	private ComboBox cb_SpName;

	private SerialPort serlPrt;

	private Timer timer1;

	private TextBox tbStartIndx;

	private Label label5;

	private Label label6;

	private Button button1;

	private Timer timer2;

	public ExPosFollow()
	{
		InitializeComponent();
	}

	private void BnStart_Click(object sender, EventArgs e)
	{
		PMainFrm.Mccard.MoCtrCard_ExPosFollow_Start(gbAxisEnBit);
	}

	private void ExPosFollow_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		gbAxisEnBit = 0;
		gbStartSendIndx = 0;
		label5_DoubleClick(sender, e);
		timer1.Enabled = false;
	}

	private void AxisEnX_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		CheckBox val = (CheckBox)sender;
		byte b = byte.MaxValue;
		if (((Control)AxisEnX).Name == ((Control)val).Name)
		{
			b = 0;
		}
		else if (((Control)AxisEnY).Name == ((Control)val).Name)
		{
			b = 1;
		}
		else if (((Control)AxisEnZ).Name == ((Control)val).Name)
		{
			b = 2;
		}
		else if (((Control)AxisEnA).Name == ((Control)val).Name)
		{
			b = 3;
		}
		else if (((Control)AxisEnB).Name == ((Control)val).Name)
		{
			b = 4;
		}
		else if (((Control)AxisEnC).Name == ((Control)val).Name)
		{
			b = 5;
		}
		if (b < byte.MaxValue)
		{
			if (val.Checked)
			{
				gbAxisEnBit |= (byte)(1 << (int)b);
			}
			else
			{
				gbAxisEnBit &= (byte)(~(1 << (int)b));
			}
		}
	}

	private void BnStop_Click(object sender, EventArgs e)
	{
		PMainFrm.Mccard.MoCtrCard_ExPosFollow_Stop(gbAxisEnBit);
	}

	private void fun_DebugInfo(string mInfo)
	{
		((Control)rtbExPosFileInfo).Text = ((Control)rtbExPosFileInfo).Text + mInfo + Environment.NewLine;
		((TextBoxBase)rtbExPosFileInfo).SelectionStart = ((Control)rtbExPosFileInfo).Text.Length;
		((TextBoxBase)rtbExPosFileInfo).ScrollToCaret();
	}

	private void bnLoadExPosFile_Click(object sender, EventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between I4 and Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		try
		{
			mListExPos.Clear();
			uint num = 0u;
			((FileDialog)openFileDialog1).Filter = "外部位置数据文件(*.expos)|*.expos";
			((FileDialog)openFileDialog1).FileName = "ExPosFile.expos";
			((FileDialog)openFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
			if (1 != (int)((CommonDialog)openFileDialog1).ShowDialog())
			{
				return;
			}
			string fileName = ((FileDialog)openFileDialog1).FileName;
			StreamReader streamReader = new StreamReader(fileName);
			fun_DebugInfo(fileName);
			while (!streamReader.EndOfStream)
			{
				string[] array = streamReader.ReadLine().Split(new char[1] { '\t' });
				num++;
				if (7 != array.Length && 6 != array.Length)
				{
					fun_DebugInfo(string.Format(GlobalData.GlobalLanguage.ExPosFollowFrm_DataError, num.ToString()));
					break;
				}
				stExPosType item = new stExPosType
				{
					mExPos = new float[6]
				};
				int num2 = 0;
				for (num2 = 0; num2 < 6; num2++)
				{
					item.mExPos[num2] = float.Parse(array[num2]);
				}
				mListExPos.Add(item);
			}
			streamReader.Close();
			fun_DebugInfo(string.Format(GlobalData.GlobalLanguage.ExPosFollowFrm_DecodeFileDone, num.ToString()));
			lbExPosInfo.Items.Clear();
			num = 1u;
			foreach (stExPosType mListExPo in mListExPos)
			{
				ListViewItem val = new ListViewItem(num.ToString());
				val.SubItems.Add(mListExPo.mExPos[0].ToString("f6"));
				val.SubItems.Add(mListExPo.mExPos[1].ToString("f6"));
				val.SubItems.Add(mListExPo.mExPos[2].ToString("f6"));
				val.SubItems.Add(mListExPo.mExPos[3].ToString("f6"));
				val.SubItems.Add(mListExPo.mExPos[4].ToString("f6"));
				val.SubItems.Add(mListExPo.mExPos[5].ToString("f6"));
				lbExPosInfo.Items.Add(val);
				num++;
			}
		}
		catch (Exception ex)
		{
			fun_DebugInfo(GlobalData.GlobalLanguage.ExPosFollowFrm_ExceptionRiseWhenDecodeFile + ex.ToString());
		}
	}

	private void rtbExPosFileInfo_DoubleClick(object sender, EventArgs e)
	{
		((Control)rtbExPosFileInfo).Text = "";
	}

	private void bn_OpenSp_Click(object sender, EventArgs e)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		try
		{
			serlPrt.PortName = ((Control)cb_SpName).Text;
			if (serlPrt.IsOpen)
			{
				serlPrt.Close();
			}
			serlPrt.ReadTimeout = 500;
			serlPrt.ReceivedBytesThreshold = 1024;
			serlPrt.RtsEnable = false;
			serlPrt.WriteTimeout = 500;
			serlPrt.ReadBufferSize = 4096;
			serlPrt.WriteBufferSize = 4096;
			serlPrt.BaudRate = 115200;
			serlPrt.Parity = (Parity)0;
			serlPrt.DataBits = 8;
			serlPrt.StopBits = (StopBits)1;
			serlPrt.Encoding = Encoding.GetEncoding("Gb2312");
			serlPrt.DataReceived += new SerialDataReceivedEventHandler(SeriaPortDataReceiveHandler);
			serlPrt.Open();
			fun_DebugInfo(((Control)cb_SpName).Text + GlobalData.GlobalLanguage.ExPosFollowFrm_OpenComunicationPortSucceed);
		}
		catch (Exception)
		{
			fun_DebugInfo(((Control)cb_SpName).Text + GlobalData.GlobalLanguage.ExPosFollowFrm_OpenComunicationPortFailed);
		}
	}

	private void bn_CloseSp_Click_1(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		try
		{
			serlPrt.Close();
			fun_DebugInfo(GlobalData.GlobalLanguage.ExPosFollowFrm_ClosePortSucceed);
		}
		catch (Exception)
		{
			fun_DebugInfo(GlobalData.GlobalLanguage.ExPosFollowFrm_ClosePortFailed);
		}
	}

	private void bnStopDownLoadExPosFile_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		fun_DebugInfo(GlobalData.GlobalLanguage.ExPosFollowFrm_StopSendExPositionPoint);
	}

	private void bnDownExPosFile_Click(object sender, EventArgs e)
	{
		gbStartSendIndx = int.Parse(((Control)tbStartIndx).Text);
		timer1.Enabled = false;
		timer1.Interval = 1;
		timer1.Enabled = true;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		UartSendResType uartSendResType = fun_SendExPos(gbStartSendIndx, 8);
		if (uartSendResType == UartSendResType.SendedOk)
		{
			gbStartSendIndx += gbPosCmndSendOkCnt;
		}
		else if (UartSendResType.ExPosListEmpty == uartSendResType)
		{
			gbSendTimes++;
			fun_DebugInfo(string.Format(GlobalData.GlobalLanguage.ExPosFollowFrm_SomeCommandSendedOKAndCurrentCount, gbSendTimes, gbBufferCnt));
			timer1.Enabled = false;
			testStt = 1;
		}
		else
		{
			fun_DebugInfo(string.Format(GlobalData.GlobalLanguage.ExPosFollowFrm_SomeCommandSendedFailed, uartSendResType.ToString()));
			timer1.Enabled = false;
		}
	}

	private UartSendResType fun_SendExPos(int nStartIndx, short nCnt)
	{
		UartSendResType result = UartSendResType.SendedOk;
		stCmndSendExPosType stCmndSendExPosType2 = new stCmndSendExPosType
		{
			CmndPos = new stExPosType[8]
		};
		int num = 0;
		gbPosCmndSendOkCnt = 0;
		if (nStartIndx < mListExPos.Count)
		{
			int num2 = nStartIndx + nCnt;
			if (num2 > mListExPos.Count)
			{
				num2 = mListExPos.Count;
			}
			stCmndSendExPosType2.CheckSum = 0;
			stCmndSendExPosType2.CmndIndx = 1;
			stCmndSendExPosType2.ValidPosCnt = (byte)(num2 - nStartIndx);
			stCmndSendExPosType2.dtLen = (byte)(4 + stCmndSendExPosType2.ValidPosCnt * 24);
			for (num = nStartIndx; num < num2; num++)
			{
				stCmndSendExPosType2.CmndPos[num - nStartIndx] = mListExPos[num];
			}
			int size = Marshal.SizeOf((object)stCmndSendExPosType2);
			byte[] array = CustomMethord.StructToBytes(stCmndSendExPosType2, size);
			for (num = 0; num < stCmndSendExPosType2.dtLen; num++)
			{
				stCmndSendExPosType2.CheckSum += array[num];
			}
			array[0] = stCmndSendExPosType2.CheckSum;
			if (serlPrt.IsOpen)
			{
				ushort num3 = 0;
				while (true)
				{
					serlPrt.ReceivedBytesThreshold = SpIntrThrdHod;
					UartReceiveStt = UartReceiveSttType.OtherEx;
					serlPrt.Write(array, 0, (int)stCmndSendExPosType2.dtLen);
					EventSPDataR.WaitOne(100);
					if (UartReceiveStt == UartReceiveSttType.DataArrived)
					{
						if (FRAMEHEAD == CommReceiveBuffer[0] && FRAMETAIL == CommReceiveBuffer[3])
						{
							gbPosCmndSendOkCnt = CommReceiveBuffer[1];
							gbBufferCnt = CommReceiveBuffer[2];
						}
						else
						{
							result = UartSendResType.FrameFormatErr;
						}
						break;
					}
					if (++num3 >= 3)
					{
						result = UartSendResType.SendTimeOut;
						break;
					}
				}
			}
			else
			{
				result = UartSendResType.PortNotOpen;
			}
		}
		else
		{
			result = UartSendResType.ExPosListEmpty;
		}
		return result;
	}

	private void SeriaPortDataReceiveHandler(object sender, SerialDataReceivedEventArgs e)
	{
		int bytesToRead = serlPrt.BytesToRead;
		if (bytesToRead < SpIntrThrdHod)
		{
			return;
		}
		try
		{
			if (bytesToRead > 256)
			{
				serlPrt.DiscardInBuffer();
				return;
			}
			serlPrt.ReceivedBytesThreshold = 1024;
			serlPrt.Read(CommReceiveBuffer, 0, bytesToRead);
			UartReceiveStt = UartReceiveSttType.DataArrived;
			EventSPDataR.Set();
		}
		catch (InvalidOperationException)
		{
			UartReceiveStt = UartReceiveSttType.PortNotOpen;
		}
		catch (TimeoutException)
		{
			UartReceiveStt = UartReceiveSttType.NoDataToRead;
		}
	}

	private void label5_DoubleClick(object sender, EventArgs e)
	{
		((Control)cb_SpName).Text = "";
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		timer2.Enabled = false;
		timer2.Interval = 1000;
		timer2.Enabled = true;
	}

	private void timer2_Tick(object sender, EventArgs e)
	{
		if (1 == testStt || testStt == 2)
		{
			if (1 == testStt)
			{
				testStt++;
				return;
			}
			testStt = 0;
			PMainFrm.Mccard.MoCtrCard_ResetCoordinate(byte.MaxValue, 0f);
			PMainFrm.Mccard.MoCtrCard_ExPosFollow_Start(gbAxisEnBit);
			gbStartSendIndx = 0;
			timer1.Enabled = true;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ExPosFollow));
		bnLoadExPosFile = new Button();
		BnStart = new Button();
		BnStop = new Button();
		AxisEnC = new CheckBox();
		AxisEnB = new CheckBox();
		AxisEnA = new CheckBox();
		AxisEnZ = new CheckBox();
		AxisEnY = new CheckBox();
		AxisEnX = new CheckBox();
		label1 = new Label();
		label2 = new Label();
		rtbExPosFileInfo = new RichTextBox();
		label3 = new Label();
		bnDownExPosFile = new Button();
		bnStopDownLoadExPosFile = new Button();
		openFileDialog1 = new OpenFileDialog();
		lbExPosInfo = new ListView();
		Index = new ColumnHeader();
		XPos = new ColumnHeader();
		YPos = new ColumnHeader();
		ZPos = new ColumnHeader();
		APos = new ColumnHeader();
		BPos = new ColumnHeader();
		CPos = new ColumnHeader();
		label4 = new Label();
		bn_CloseSp = new Button();
		bn_OpenSp = new Button();
		cb_SpName = new ComboBox();
		serlPrt = new SerialPort(components);
		timer1 = new Timer(components);
		tbStartIndx = new TextBox();
		label5 = new Label();
		label6 = new Label();
		button1 = new Button();
		timer2 = new Timer(components);
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(bnLoadExPosFile, "bnLoadExPosFile");
		((Control)bnLoadExPosFile).Name = "bnLoadExPosFile";
		((ButtonBase)bnLoadExPosFile).UseVisualStyleBackColor = true;
		((Control)bnLoadExPosFile).Click += bnLoadExPosFile_Click;
		componentResourceManager.ApplyResources(BnStart, "BnStart");
		((Control)BnStart).Name = "BnStart";
		((ButtonBase)BnStart).UseVisualStyleBackColor = true;
		((Control)BnStart).Click += BnStart_Click;
		componentResourceManager.ApplyResources(BnStop, "BnStop");
		((Control)BnStop).Name = "BnStop";
		((ButtonBase)BnStop).UseVisualStyleBackColor = true;
		((Control)BnStop).Click += BnStop_Click;
		componentResourceManager.ApplyResources(AxisEnC, "AxisEnC");
		((Control)AxisEnC).Name = "AxisEnC";
		((ButtonBase)AxisEnC).UseVisualStyleBackColor = true;
		AxisEnC.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(AxisEnB, "AxisEnB");
		((Control)AxisEnB).Name = "AxisEnB";
		((ButtonBase)AxisEnB).UseVisualStyleBackColor = true;
		AxisEnB.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(AxisEnA, "AxisEnA");
		((Control)AxisEnA).Name = "AxisEnA";
		((ButtonBase)AxisEnA).UseVisualStyleBackColor = true;
		AxisEnA.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(AxisEnZ, "AxisEnZ");
		((Control)AxisEnZ).Name = "AxisEnZ";
		((ButtonBase)AxisEnZ).UseVisualStyleBackColor = true;
		AxisEnZ.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(AxisEnY, "AxisEnY");
		((Control)AxisEnY).Name = "AxisEnY";
		((ButtonBase)AxisEnY).UseVisualStyleBackColor = true;
		AxisEnY.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(AxisEnX, "AxisEnX");
		((Control)AxisEnX).Name = "AxisEnX";
		((ButtonBase)AxisEnX).UseVisualStyleBackColor = true;
		AxisEnX.CheckedChanged += AxisEnX_CheckedChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(rtbExPosFileInfo, "rtbExPosFileInfo");
		((Control)rtbExPosFileInfo).Name = "rtbExPosFileInfo";
		((Control)rtbExPosFileInfo).DoubleClick += rtbExPosFileInfo_DoubleClick;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(bnDownExPosFile, "bnDownExPosFile");
		((Control)bnDownExPosFile).Name = "bnDownExPosFile";
		((ButtonBase)bnDownExPosFile).UseVisualStyleBackColor = true;
		((Control)bnDownExPosFile).Click += bnDownExPosFile_Click;
		componentResourceManager.ApplyResources(bnStopDownLoadExPosFile, "bnStopDownLoadExPosFile");
		((Control)bnStopDownLoadExPosFile).Name = "bnStopDownLoadExPosFile";
		((ButtonBase)bnStopDownLoadExPosFile).UseVisualStyleBackColor = true;
		((Control)bnStopDownLoadExPosFile).Click += bnStopDownLoadExPosFile_Click;
		((FileDialog)openFileDialog1).FileName = "openFileDialog1";
		componentResourceManager.ApplyResources(openFileDialog1, "openFileDialog1");
		componentResourceManager.ApplyResources(lbExPosInfo, "lbExPosInfo");
		lbExPosInfo.Columns.AddRange((ColumnHeader[])(object)new ColumnHeader[7] { Index, XPos, YPos, ZPos, APos, BPos, CPos });
		lbExPosInfo.GridLines = true;
		lbExPosInfo.HideSelection = false;
		lbExPosInfo.MultiSelect = false;
		((Control)lbExPosInfo).Name = "lbExPosInfo";
		lbExPosInfo.UseCompatibleStateImageBehavior = false;
		lbExPosInfo.View = (View)1;
		componentResourceManager.ApplyResources(Index, "Index");
		componentResourceManager.ApplyResources(XPos, "XPos");
		componentResourceManager.ApplyResources(YPos, "YPos");
		componentResourceManager.ApplyResources(ZPos, "ZPos");
		componentResourceManager.ApplyResources(APos, "APos");
		componentResourceManager.ApplyResources(BPos, "BPos");
		componentResourceManager.ApplyResources(CPos, "CPos");
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(bn_CloseSp, "bn_CloseSp");
		((Control)bn_CloseSp).Name = "bn_CloseSp";
		((ButtonBase)bn_CloseSp).UseVisualStyleBackColor = true;
		((Control)bn_CloseSp).Click += bn_CloseSp_Click_1;
		componentResourceManager.ApplyResources(bn_OpenSp, "bn_OpenSp");
		((Control)bn_OpenSp).Name = "bn_OpenSp";
		((ButtonBase)bn_OpenSp).UseVisualStyleBackColor = true;
		((Control)bn_OpenSp).Click += bn_OpenSp_Click;
		componentResourceManager.ApplyResources(cb_SpName, "cb_SpName");
		((ListControl)cb_SpName).FormattingEnabled = true;
		((Control)cb_SpName).Name = "cb_SpName";
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(tbStartIndx, "tbStartIndx");
		((Control)tbStartIndx).Name = "tbStartIndx";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		((Control)label5).DoubleClick += label5_DoubleClick;
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(button1, "button1");
		((Control)button1).Name = "button1";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button1).Click += button1_Click;
		timer2.Tick += timer2_Tick;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)button1);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)tbStartIndx);
		((Control)this).Controls.Add((Control)(object)bn_CloseSp);
		((Control)this).Controls.Add((Control)(object)bn_OpenSp);
		((Control)this).Controls.Add((Control)(object)cb_SpName);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)lbExPosInfo);
		((Control)this).Controls.Add((Control)(object)bnStopDownLoadExPosFile);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)bnDownExPosFile);
		((Control)this).Controls.Add((Control)(object)rtbExPosFileInfo);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)BnStart);
		((Control)this).Controls.Add((Control)(object)BnStop);
		((Control)this).Controls.Add((Control)(object)AxisEnC);
		((Control)this).Controls.Add((Control)(object)AxisEnB);
		((Control)this).Controls.Add((Control)(object)AxisEnA);
		((Control)this).Controls.Add((Control)(object)AxisEnZ);
		((Control)this).Controls.Add((Control)(object)AxisEnY);
		((Control)this).Controls.Add((Control)(object)AxisEnX);
		((Control)this).Controls.Add((Control)(object)bnLoadExPosFile);
		((Form)this).FormBorderStyle = (FormBorderStyle)1;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "ExPosFollow";
		((Form)this).Load += ExPosFollow_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
