using System.Runtime.InteropServices;

namespace ADTToolBox;

public struct stScopeSaveOpenType
{
	public uint dwSampleTick;

	public uint dwSampleCnt;

	public uint dwChannelSum;

	public uint dwChannelEnableBit;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public stScopeSaveOpenChannelType[] Channel;
}
