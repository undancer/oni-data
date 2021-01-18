using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KSerialization
{
	public static class Helper
	{
		private static SerializationTypeInfo TYPE_INFO_MASK = (SerializationTypeInfo)255;

		public static bool IsUserDefinedType(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.VALUE_MASK) == 0;
		}

		public static bool IsArray(SerializationTypeInfo type_info)
		{
			SerializationTypeInfo serializationTypeInfo = type_info & SerializationTypeInfo.VALUE_MASK;
			return serializationTypeInfo == SerializationTypeInfo.Array;
		}

		public static bool IsGenericType(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.IS_GENERIC_TYPE) != 0;
		}

		public static bool IsValueType(SerializationTypeInfo type_info)
		{
			return (type_info & SerializationTypeInfo.IS_VALUE_TYPE) != 0;
		}

		public static SerializationTypeInfo EncodeSerializationType(Type type)
		{
			SerializationTypeInfo serializationTypeInfo = SerializationTypeInfo.UserDefined;
			if (type == typeof(sbyte))
			{
				serializationTypeInfo = SerializationTypeInfo.SByte;
			}
			else if (type == typeof(byte))
			{
				serializationTypeInfo = SerializationTypeInfo.Byte;
			}
			else if (type == typeof(bool))
			{
				serializationTypeInfo = SerializationTypeInfo.Boolean;
			}
			else if (type == typeof(short))
			{
				serializationTypeInfo = SerializationTypeInfo.Int16;
			}
			else if (type == typeof(ushort))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt16;
			}
			else if (type == typeof(int))
			{
				serializationTypeInfo = SerializationTypeInfo.Int32;
			}
			else if (type == typeof(uint))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt32;
			}
			else if (type == typeof(long))
			{
				serializationTypeInfo = SerializationTypeInfo.Int64;
			}
			else if (type == typeof(ulong))
			{
				serializationTypeInfo = SerializationTypeInfo.UInt64;
			}
			else if (type == typeof(float))
			{
				serializationTypeInfo = SerializationTypeInfo.Single;
			}
			else if (type == typeof(double))
			{
				serializationTypeInfo = SerializationTypeInfo.Double;
			}
			else if (type == typeof(string))
			{
				serializationTypeInfo = SerializationTypeInfo.String;
			}
			else if (type == typeof(Vector2I))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector2I;
			}
			else if (type == typeof(Vector2))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector2;
			}
			else if (type == typeof(Vector3))
			{
				serializationTypeInfo = SerializationTypeInfo.Vector3;
			}
			else if (type == typeof(Color))
			{
				serializationTypeInfo = SerializationTypeInfo.Colour;
			}
			else if (typeof(Array).IsAssignableFrom(type))
			{
				serializationTypeInfo = SerializationTypeInfo.Array;
			}
			else if (type.IsEnum)
			{
				serializationTypeInfo = SerializationTypeInfo.Enumeration;
			}
			else if (type.IsGenericType)
			{
				serializationTypeInfo = SerializationTypeInfo.IS_GENERIC_TYPE;
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				serializationTypeInfo = ((genericTypeDefinition == typeof(List<>)) ? (serializationTypeInfo | SerializationTypeInfo.List) : ((genericTypeDefinition == typeof(Dictionary<, >)) ? (serializationTypeInfo | SerializationTypeInfo.Dictionary) : ((genericTypeDefinition == typeof(HashSet<>)) ? (serializationTypeInfo | SerializationTypeInfo.HashSet) : ((genericTypeDefinition == typeof(KeyValuePair<, >)) ? (serializationTypeInfo | SerializationTypeInfo.Pair) : ((!(genericTypeDefinition == typeof(Queue<>))) ? (serializationTypeInfo | SerializationTypeInfo.UserDefined) : (serializationTypeInfo | SerializationTypeInfo.Queue))))));
			}
			else
			{
				serializationTypeInfo = SerializationTypeInfo.UserDefined;
				if (type.IsValueType)
				{
					serializationTypeInfo |= SerializationTypeInfo.IS_VALUE_TYPE;
				}
			}
			return serializationTypeInfo & TYPE_INFO_MASK;
		}

		public static void WriteValue(this BinaryWriter writer, TypeInfo type_info, object value)
		{
			switch (type_info.info & SerializationTypeInfo.VALUE_MASK)
			{
			case SerializationTypeInfo.SByte:
				writer.Write((sbyte)value);
				break;
			case SerializationTypeInfo.Byte:
				writer.Write((byte)value);
				break;
			case SerializationTypeInfo.Boolean:
				writer.Write((byte)(((bool)value) ? 1u : 0u));
				break;
			case SerializationTypeInfo.Int16:
				writer.Write((short)value);
				break;
			case SerializationTypeInfo.UInt16:
				writer.Write((ushort)value);
				break;
			case SerializationTypeInfo.Int32:
				writer.Write((int)value);
				break;
			case SerializationTypeInfo.UInt32:
				writer.Write((uint)value);
				break;
			case SerializationTypeInfo.Int64:
				writer.Write((long)value);
				break;
			case SerializationTypeInfo.UInt64:
				writer.Write((ulong)value);
				break;
			case SerializationTypeInfo.Single:
				writer.WriteSingleFast((float)value);
				break;
			case SerializationTypeInfo.Double:
				writer.Write((double)value);
				break;
			case SerializationTypeInfo.String:
				writer.WriteKleiString((string)value);
				break;
			case SerializationTypeInfo.Enumeration:
				writer.Write((int)value);
				break;
			case SerializationTypeInfo.Vector2I:
			{
				Vector2I vector2I = (Vector2I)value;
				writer.Write(vector2I.x);
				writer.Write(vector2I.y);
				break;
			}
			case SerializationTypeInfo.Vector2:
			{
				Vector2 vector = (Vector2)value;
				writer.WriteSingleFast(vector.x);
				writer.WriteSingleFast(vector.y);
				break;
			}
			case SerializationTypeInfo.Vector3:
			{
				Vector3 vector2 = (Vector3)value;
				writer.WriteSingleFast(vector2.x);
				writer.WriteSingleFast(vector2.y);
				writer.WriteSingleFast(vector2.z);
				break;
			}
			case SerializationTypeInfo.Colour:
			{
				Color color = (Color)value;
				writer.Write((byte)(color.r * 255f));
				writer.Write((byte)(color.g * 255f));
				writer.Write((byte)(color.b * 255f));
				writer.Write((byte)(color.a * 255f));
				break;
			}
			case SerializationTypeInfo.Array:
				if (value != null)
				{
					Array array = value as Array;
					TypeInfo typeInfo3 = type_info.subTypes[0];
					long position16 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(array.Length);
					long position17 = writer.BaseStream.Position;
					if (IsPOD(typeInfo3.info))
					{
						WriteArrayFast(writer, typeInfo3, array);
					}
					else if (IsValueType(typeInfo3.info))
					{
						SerializationTemplate serializationTemplate4 = Manager.GetSerializationTemplate(typeInfo3.type);
						for (int i = 0; i < array.Length; i++)
						{
							serializationTemplate4.SerializeData(array.GetValue(i), writer);
						}
					}
					else
					{
						for (int j = 0; j < array.Length; j++)
						{
							writer.WriteValue(typeInfo3, array.GetValue(j));
						}
					}
					long position18 = writer.BaseStream.Position;
					long num7 = position18 - position17;
					writer.BaseStream.Position = position16;
					writer.Write((int)num7);
					writer.BaseStream.Position = position18;
				}
				else
				{
					writer.Write(0);
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.UserDefined:
				if (value != null)
				{
					long position7 = writer.BaseStream.Position;
					writer.Write(0);
					long position8 = writer.BaseStream.Position;
					SerializationTemplate serializationTemplate3 = Manager.GetSerializationTemplate(type_info.type);
					serializationTemplate3.SerializeData(value, writer);
					long position9 = writer.BaseStream.Position;
					long num4 = position9 - position8;
					writer.BaseStream.Position = position7;
					writer.Write((int)num4);
					writer.BaseStream.Position = position9;
				}
				else
				{
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.HashSet:
				if (value != null)
				{
					TypeInfo typeInfo2 = type_info.subTypes[0];
					long position4 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(0);
					long position5 = writer.BaseStream.Position;
					int num2 = 0;
					IEnumerable enumerable = value as IEnumerable;
					if (IsValueType(typeInfo2.info))
					{
						SerializationTemplate serializationTemplate2 = Manager.GetSerializationTemplate(typeInfo2.type);
						foreach (object item in enumerable)
						{
							serializationTemplate2.SerializeData(item, writer);
							num2++;
						}
					}
					else
					{
						foreach (object item2 in enumerable)
						{
							writer.WriteValue(typeInfo2, item2);
							num2++;
						}
					}
					long position6 = writer.BaseStream.Position;
					long num3 = position6 - position5;
					writer.BaseStream.Position = position4;
					writer.Write((int)num3);
					writer.Write(num2);
					writer.BaseStream.Position = position6;
				}
				else
				{
					writer.Write(0);
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.List:
				if (value != null)
				{
					TypeInfo typeInfo4 = type_info.subTypes[0];
					ICollection collection2 = value as ICollection;
					long position19 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(collection2.Count);
					long position20 = writer.BaseStream.Position;
					if (IsPOD(typeInfo4.info))
					{
						WriteListPOD(writer, typeInfo4, collection2);
					}
					else if (IsValueType(typeInfo4.info))
					{
						SerializationTemplate serializationTemplate5 = Manager.GetSerializationTemplate(typeInfo4.type);
						foreach (object item3 in collection2)
						{
							serializationTemplate5.SerializeData(item3, writer);
						}
					}
					else
					{
						foreach (object item4 in collection2)
						{
							writer.WriteValue(typeInfo4, item4);
						}
					}
					long position21 = writer.BaseStream.Position;
					long num8 = position21 - position20;
					writer.BaseStream.Position = position19;
					writer.Write((int)num8);
					writer.BaseStream.Position = position21;
				}
				else
				{
					writer.Write(0);
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.Dictionary:
				if (value != null)
				{
					TypeInfo type_info4 = type_info.subTypes[0];
					TypeInfo type_info5 = type_info.subTypes[1];
					IDictionary dictionary = value as IDictionary;
					ICollection keys = dictionary.Keys;
					ICollection values = dictionary.Values;
					long position13 = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(values.Count);
					long position14 = writer.BaseStream.Position;
					foreach (object item5 in values)
					{
						writer.WriteValue(type_info5, item5);
					}
					foreach (object item6 in keys)
					{
						writer.WriteValue(type_info4, item6);
					}
					long position15 = writer.BaseStream.Position;
					long num6 = position15 - position14;
					writer.BaseStream.Position = position13;
					writer.Write((int)num6);
					writer.BaseStream.Position = position15;
				}
				else
				{
					writer.Write(0);
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.Pair:
				if (value != null)
				{
					PropertyInfo property = type_info.type.GetProperty("Key");
					PropertyInfo property2 = type_info.type.GetProperty("Value");
					object value2 = property.GetValue(value, null);
					object value3 = property2.GetValue(value, null);
					TypeInfo type_info2 = type_info.subTypes[0];
					TypeInfo type_info3 = type_info.subTypes[1];
					long position10 = writer.BaseStream.Position;
					writer.Write(0);
					long position11 = writer.BaseStream.Position;
					writer.WriteValue(type_info2, value2);
					writer.WriteValue(type_info3, value3);
					long position12 = writer.BaseStream.Position;
					long num5 = position12 - position11;
					writer.BaseStream.Position = position10;
					writer.Write((int)num5);
					writer.BaseStream.Position = position12;
				}
				else
				{
					writer.Write(4);
					writer.Write(-1);
				}
				break;
			case SerializationTypeInfo.Queue:
				if (value != null)
				{
					TypeInfo typeInfo = type_info.subTypes[0];
					ICollection collection = value as ICollection;
					long position = writer.BaseStream.Position;
					writer.Write(0);
					writer.Write(collection.Count);
					long position2 = writer.BaseStream.Position;
					if (IsPOD(typeInfo.info))
					{
						WriteListPOD(writer, typeInfo, collection);
					}
					else if (IsValueType(typeInfo.info))
					{
						SerializationTemplate serializationTemplate = Manager.GetSerializationTemplate(typeInfo.type);
						foreach (object item7 in collection)
						{
							serializationTemplate.SerializeData(item7, writer);
						}
					}
					else
					{
						foreach (object item8 in collection)
						{
							writer.WriteValue(typeInfo, item8);
						}
					}
					long position3 = writer.BaseStream.Position;
					long num = position3 - position2;
					writer.BaseStream.Position = position;
					writer.Write((int)num);
					writer.BaseStream.Position = position3;
				}
				else
				{
					writer.Write(0);
					writer.Write(-1);
				}
				break;
			default:
				throw new ArgumentException("Don't know how to serialize type: " + type_info.type.ToString());
			}
		}

		private static void WriteArrayFast(BinaryWriter writer, TypeInfo elem_type_info, Array array)
		{
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			{
				sbyte[] array5 = (sbyte[])array;
				for (int l = 0; l < array.Length; l++)
				{
					writer.Write(array5[l]);
				}
				break;
			}
			case SerializationTypeInfo.Byte:
				writer.Write((byte[])array);
				break;
			case SerializationTypeInfo.Int16:
			{
				short[] array10 = (short[])array;
				for (int num3 = 0; num3 < array.Length; num3++)
				{
					writer.Write(array10[num3]);
				}
				break;
			}
			case SerializationTypeInfo.UInt16:
			{
				ushort[] array7 = (ushort[])array;
				for (int n = 0; n < array.Length; n++)
				{
					writer.Write(array7[n]);
				}
				break;
			}
			case SerializationTypeInfo.Int32:
			{
				int[] array3 = (int[])array;
				for (int j = 0; j < array.Length; j++)
				{
					writer.Write(array3[j]);
				}
				break;
			}
			case SerializationTypeInfo.UInt32:
			{
				uint[] array9 = (uint[])array;
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					writer.Write(array9[num2]);
				}
				break;
			}
			case SerializationTypeInfo.Int64:
			{
				long[] array8 = (long[])array;
				for (int num = 0; num < array.Length; num++)
				{
					writer.Write(array8[num]);
				}
				break;
			}
			case SerializationTypeInfo.UInt64:
			{
				ulong[] array6 = (ulong[])array;
				for (int m = 0; m < array.Length; m++)
				{
					writer.Write(array6[m]);
				}
				break;
			}
			case SerializationTypeInfo.Single:
			{
				float[] array4 = (float[])array;
				for (int k = 0; k < array.Length; k++)
				{
					writer.WriteSingleFast(array4[k]);
				}
				break;
			}
			case SerializationTypeInfo.Double:
			{
				double[] array2 = (double[])array;
				for (int i = 0; i < array.Length; i++)
				{
					writer.Write(array2[i]);
				}
				break;
			}
			default:
				throw new Exception("unknown pod type");
			}
		}

		private static void WriteListPOD(BinaryWriter writer, TypeInfo elem_type_info, ICollection collection)
		{
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
				foreach (object item in collection)
				{
					writer.Write((sbyte)item);
				}
				break;
			case SerializationTypeInfo.Byte:
				foreach (object item2 in collection)
				{
					writer.Write((byte)item2);
				}
				break;
			case SerializationTypeInfo.Int16:
				foreach (object item3 in collection)
				{
					writer.Write((short)item3);
				}
				break;
			case SerializationTypeInfo.UInt16:
				foreach (object item4 in collection)
				{
					writer.Write((ushort)item4);
				}
				break;
			case SerializationTypeInfo.Int32:
				foreach (object item5 in collection)
				{
					writer.Write((int)item5);
				}
				break;
			case SerializationTypeInfo.UInt32:
				foreach (object item6 in collection)
				{
					writer.Write((uint)item6);
				}
				break;
			case SerializationTypeInfo.Int64:
				foreach (object item7 in collection)
				{
					writer.Write((long)item7);
				}
				break;
			case SerializationTypeInfo.UInt64:
				foreach (object item8 in collection)
				{
					writer.Write((ulong)item8);
				}
				break;
			case SerializationTypeInfo.Single:
				foreach (object item9 in collection)
				{
					writer.WriteSingleFast((float)item9);
				}
				break;
			case SerializationTypeInfo.Double:
				foreach (object item10 in collection)
				{
					writer.Write((double)item10);
				}
				break;
			case SerializationTypeInfo.Boolean:
				break;
			}
		}

		public static void GetSerializationMethods(this Type type, Type type_a, Type type_b, Type type_c, out MethodInfo method_a, out MethodInfo method_b, out MethodInfo method_c)
		{
			method_a = null;
			method_b = null;
			method_c = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				object[] customAttributes = methodInfo.GetCustomAttributes(inherit: false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					if (customAttributes[j].GetType() == type_a)
					{
						method_a = methodInfo;
					}
					else if (customAttributes[j].GetType() == type_b)
					{
						method_b = methodInfo;
					}
					else if (customAttributes[j].GetType() == type_c)
					{
						method_c = methodInfo;
					}
				}
			}
		}

		public static bool IsPOD(SerializationTypeInfo info)
		{
			SerializationTypeInfo serializationTypeInfo = info;
			if (serializationTypeInfo - 1 <= SerializationTypeInfo.SByte || serializationTypeInfo - 4 <= SerializationTypeInfo.UInt32)
			{
				return true;
			}
			return false;
		}

		public static bool IsPOD(Type type)
		{
			if (type == typeof(int) || type == typeof(uint) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(float) || type == typeof(double) || type == typeof(short) || type == typeof(ushort) || type == typeof(long) || type == typeof(ulong))
			{
				return true;
			}
			return false;
		}

		public static string GetKTypeString(this Type type)
		{
			return type.FullName;
		}

		public static void ClearTypeInfoMask()
		{
			TYPE_INFO_MASK = (SerializationTypeInfo)255;
		}

		public static void SetTypeInfoMask(SerializationTypeInfo mask)
		{
			TYPE_INFO_MASK = mask;
		}
	}
}
