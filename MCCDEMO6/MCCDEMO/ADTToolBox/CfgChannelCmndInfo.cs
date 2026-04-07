using System.Runtime.InteropServices;

namespace ADTToolBox;

public struct CfgChannelCmndInfo
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public ChannelCfgInfoType[] Channel;
}
