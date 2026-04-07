using System;
using System.Runtime.InteropServices;

namespace MCCDEMO;

internal class CustomMethord
{
	public static object ByteToStruct(byte[] bytes, Type type)
	{
		int num = Marshal.SizeOf(type);
		if (num > bytes.Length)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(bytes, 0, intPtr, num);
		object? result = Marshal.PtrToStructure(intPtr, type);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public object CharToStruct(char[] bytes, Type type)
	{
		int num = Marshal.SizeOf(type);
		if (num > bytes.Length)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(bytes, 0, intPtr, num);
		object? result = Marshal.PtrToStructure(intPtr, type);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public static byte[] StructToBytes(object structObj, int size)
	{
		byte[] array = new byte[size];
		IntPtr intPtr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(structObj, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, size);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public char[] StructToChars(object structObj, int size)
	{
		char[] array = new char[size];
		IntPtr intPtr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(structObj, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, size);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}
}
