using System;
using System.Runtime.InteropServices;
using System.Text;

public class UTF8Marshaler : ICustomMarshaler
{
	private static UTF8Marshaler instance_;

	public IntPtr MarshalManagedToNative(object obj)
	{
		if (obj == null)
		{
			return IntPtr.Zero;
		}
		if (!(obj is string))
		{
			throw new MarshalDirectiveException("Invalid obj in UTF8Marshaler.");
		}
		byte[] bytes = Encoding.UTF8.GetBytes((string)obj);
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte((IntPtr)((long)intPtr + bytes.Length), 0);
		return intPtr;
	}

	public object MarshalNativeToManaged(IntPtr data)
	{
		return MarshalNativeToString(data);
	}

	public void CleanUpNativeData(IntPtr data)
	{
		Marshal.FreeHGlobal(data);
	}

	public void CleanUpManagedData(object obj)
	{
	}

	public int GetNativeDataSize()
	{
		return -1;
	}

	public static ICustomMarshaler GetInstance(string cookie)
	{
		if (instance_ == null)
		{
			return instance_ = new UTF8Marshaler();
		}
		return instance_;
	}

	public static string MarshalNativeToString(IntPtr data)
	{
		int i;
		for (i = 0; Marshal.ReadByte(data, i) != 0; i++)
		{
		}
		if (i == 0)
		{
			return string.Empty;
		}
		byte[] array = new byte[i];
		Marshal.Copy(data, array, 0, i);
		return Encoding.UTF8.GetString(array);
	}
}
