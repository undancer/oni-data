using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace KSerialization
{
	public static class IOHelper
	{
		private static byte[] s_stringBuffer = new byte[1024];

		private static byte[] s_singleBuffer = new byte[4];

		public static void WriteKleiString(this BinaryWriter writer, string str)
		{
			if (str != null)
			{
				Encoding uTF = Encoding.UTF8;
				int byteCount = uTF.GetByteCount(str);
				writer.Write(byteCount);
				if (byteCount < s_stringBuffer.Length)
				{
					uTF.GetBytes(str, 0, str.Length, s_stringBuffer, 0);
					writer.Write(s_stringBuffer, 0, byteCount);
				}
				else
				{
					Debug.LogWarning($"Writing large string {str} of {byteCount} bytes");
					writer.Write(uTF.GetBytes(str));
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		public unsafe static void WriteSingleFast(this BinaryWriter writer, float value)
		{
			byte* ptr = (byte*)(&value);
			if (BitConverter.IsLittleEndian)
			{
				s_singleBuffer[0] = *ptr;
				s_singleBuffer[1] = ptr[1];
				s_singleBuffer[2] = ptr[2];
				s_singleBuffer[3] = ptr[3];
			}
			else
			{
				s_singleBuffer[0] = ptr[3];
				s_singleBuffer[1] = ptr[2];
				s_singleBuffer[2] = ptr[1];
				s_singleBuffer[3] = *ptr;
			}
			writer.Write(s_singleBuffer);
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void WriteBoundaryTag(this BinaryWriter writer, object tag)
		{
			writer.Write((uint)tag);
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void CheckBoundaryTag(this IReader reader, object expected)
		{
			uint num = reader.ReadUInt32();
			if ((uint)expected != num)
			{
				Debug.LogError($"Expected Tag {expected.ToString()}(0x{(uint)expected:X}) but got 0x{num:X} instead");
			}
		}

		[Conditional("DEBUG_VALIDATE")]
		public static void Assert(bool condition)
		{
			DebugUtil.Assert(condition);
		}

		public static Vector2I ReadVector2I(this IReader reader)
		{
			Vector2I result = default(Vector2I);
			result.x = reader.ReadInt32();
			result.y = reader.ReadInt32();
			return result;
		}

		public static Vector2 ReadVector2(this IReader reader)
		{
			Vector2 result = default(Vector2);
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			return result;
		}

		public static Vector3 ReadVector3(this IReader reader)
		{
			Vector3 result = default(Vector3);
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			return result;
		}

		public static Color ReadColour(this IReader reader)
		{
			byte b = reader.ReadByte();
			byte b2 = reader.ReadByte();
			byte b3 = reader.ReadByte();
			byte b4 = reader.ReadByte();
			Color result = default(Color);
			result.r = (float)(int)b / 255f;
			result.g = (float)(int)b2 / 255f;
			result.b = (float)(int)b3 / 255f;
			result.a = (float)(int)b4 / 255f;
			return result;
		}
	}
}
