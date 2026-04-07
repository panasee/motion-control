using System.Runtime.InteropServices;

namespace ADTToolBox;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ScopeCmndResType
{
	public uint dwCmndNo;

	public uint dwCmndRes;

	public uint dwScopeStt;

	public int dwSampleCnt;

	public uint dwUploadLen;
}
