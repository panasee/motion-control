using System.Runtime.InteropServices;

namespace ADTToolBox;

[StructLayout(LayoutKind.Explicit, Size = 4)]
internal struct ScopeUploadDtType
{
	[FieldOffset(0)]
	public byte BuffHead;

	[FieldOffset(1)]
	public byte BuffLen;

	[FieldOffset(2)]
	public byte BuffCheck;

	[FieldOffset(3)]
	public byte BuffHeadNot;

	[FieldOffset(4)]
	public uint SampleCnt;

	[FieldOffset(8)]
	public int IVal0;

	[FieldOffset(8)]
	public float FVal0;

	[FieldOffset(12)]
	public int IVal1;

	[FieldOffset(12)]
	public float FVal1;

	[FieldOffset(16)]
	public int IVal2;

	[FieldOffset(16)]
	public float FVal2;

	[FieldOffset(20)]
	public int IVal3;

	[FieldOffset(20)]
	public float FVal3;

	[FieldOffset(24)]
	public int IVal4;

	[FieldOffset(24)]
	public float FVal4;

	[FieldOffset(28)]
	public int IVal5;

	[FieldOffset(28)]
	public float FVal5;

	[FieldOffset(32)]
	public int IVal6;

	[FieldOffset(32)]
	public float FVal6;

	[FieldOffset(36)]
	public int IVal7;

	[FieldOffset(36)]
	public float FVal7;
}
