using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace ADTToolBox;

public class ChannelType
{
	private static Semaphore sem = new Semaphore(1, 1);

	public uint dwChannelIndx;

	public uint dwDevId;

	public uint dwItem;

	public string sLineName;

	public Color cLine;

	public uint dwSampledDtLen;

	public int BuffHead;

	public int BuffTail;

	public int BuffDtCnt;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10000)]
	public double[] mValue;

	public ChannelType(uint nChannelIndx)
	{
		dwChannelIndx = nChannelIndx;
		dwDevId = 0u;
		dwItem = 0u;
		sLineName = "";
		cLine = Color.Red;
		dwSampledDtLen = 0u;
		BuffHead = 0;
		BuffTail = 0;
		BuffDtCnt = 0;
		mValue = new double[10000];
	}

	public ushort WriteDtToBuff(double[] nData, int nLen)
	{
		sem.WaitOne();
		int num = 0;
		for (num = 0; num < nLen; num++)
		{
			mValue[BuffHead++] = nData[num];
			if (BuffHead >= 10000)
			{
				BuffHead = 0;
			}
		}
		BuffDtCnt += nLen;
		sem.Release();
		return 0;
	}

	public ushort WriteDtToBuff(double nData)
	{
		sem.WaitOne();
		mValue[BuffHead++] = nData;
		if (BuffHead >= 10000)
		{
			BuffHead = 0;
		}
		BuffDtCnt++;
		sem.Release();
		return 0;
	}

	public ushort ReadDtFromBuff(double[] nData, int nLen, int nActLen)
	{
		ushort result = 0;
		int num = 0;
		while (BuffDtCnt > 0)
		{
			nData[num] = mValue[BuffTail++];
			if (BuffTail >= 10000)
			{
				BuffTail = 0;
			}
			num++;
			BuffDtCnt--;
		}
		return result;
	}
}
