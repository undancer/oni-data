using System.Text;

public class FastReader : IReader
{
	private int idx;

	private byte[] bytes;

	public bool IsFinished
	{
		get
		{
			if (bytes != null)
			{
				return idx == bytes.Length;
			}
			return true;
		}
	}

	public int Position
	{
		get
		{
			return idx;
		}
		set
		{
			idx = value;
		}
	}

	public FastReader(byte[] bytes)
	{
		this.bytes = bytes;
	}

	public unsafe byte ReadByte()
	{
		byte result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *ptr;
		}
		idx++;
		return result;
	}

	public unsafe sbyte ReadSByte()
	{
		byte result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *ptr;
		}
		idx++;
		return (sbyte)result;
	}

	public unsafe ushort ReadUInt16()
	{
		ushort result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(ushort*)ptr;
		}
		idx += 2;
		return result;
	}

	public unsafe short ReadInt16()
	{
		short result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(short*)ptr;
		}
		idx += 2;
		return result;
	}

	public unsafe uint ReadUInt32()
	{
		uint result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(uint*)ptr;
		}
		idx += 4;
		return result;
	}

	public unsafe int ReadInt32()
	{
		int result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(int*)ptr;
		}
		idx += 4;
		return result;
	}

	public unsafe ulong ReadUInt64()
	{
		long result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(long*)ptr;
		}
		idx += 8;
		return (ulong)result;
	}

	public unsafe long ReadInt64()
	{
		long result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(long*)ptr;
		}
		idx += 8;
		return result;
	}

	public unsafe float ReadSingle()
	{
		float result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(float*)ptr;
		}
		idx += 4;
		return result;
	}

	public unsafe double ReadDouble()
	{
		double result;
		fixed (byte* ptr = &bytes[idx])
		{
			result = *(double*)ptr;
		}
		idx += 8;
		return result;
	}

	public char[] ReadChars(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = (char)bytes[idx + i];
		}
		idx += length;
		return array;
	}

	public byte[] ReadBytes(int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = bytes[idx + i];
		}
		idx += length;
		return array;
	}

	public string ReadKleiString()
	{
		int num = ReadInt32();
		string result = null;
		if (num >= 0)
		{
			result = Encoding.UTF8.GetString(bytes, idx, num);
			idx += num;
		}
		return result;
	}

	public void SkipBytes(int length)
	{
		idx += length;
	}

	public byte[] RawBytes()
	{
		return bytes;
	}
}
