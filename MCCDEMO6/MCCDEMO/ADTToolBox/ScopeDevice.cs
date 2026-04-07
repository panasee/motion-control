using System.Collections.Generic;

namespace ADTToolBox;

public class ScopeDevice
{
	public uint dwSampleTick;

	public uint dwSampleCnt;

	public uint dwChannelSum;

	public uint dwChannelEnableBit;

	public uint dwSampledCnt;

	public uint dwCntIndx;

	public ChannelType[] Channel;

	public List<stListElementType> mDtList;

	public uint dwCheckSumValidCnt;

	public uint dwCheckSumErrCnt;

	public ScopeDevice()
	{
		dwSampleTick = 100u;
		dwSampleCnt = 100u;
		dwChannelSum = 0u;
		dwChannelEnableBit = 0u;
		dwSampledCnt = 0u;
		dwCntIndx = 0u;
		dwCheckSumErrCnt = 0u;
		dwCheckSumValidCnt = 0u;
		Channel = new ChannelType[8];
		mDtList = new List<stListElementType>();
		ushort num = 0;
		for (num = 0; num < 8; num++)
		{
			Channel[num] = new ChannelType(10u);
		}
	}
}
