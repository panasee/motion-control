using System.Runtime.InteropServices;

namespace ADTToolBox;

public struct stListElementType
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public double[] mDtVal;
}
