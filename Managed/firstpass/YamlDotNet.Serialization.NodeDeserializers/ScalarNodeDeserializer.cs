using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ScalarNodeDeserializer : INodeDeserializer
	{
		private const string BooleanTruePattern = "^(true|y|yes|on)$";

		private const string BooleanFalsePattern = "^(false|n|no|off)$";

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			Scalar scalar = parser.Allow<Scalar>();
			if (scalar == null)
			{
				value = null;
				return false;
			}
			if (expectedType.IsEnum())
			{
				value = Enum.Parse(expectedType, scalar.Value, ignoreCase: true);
			}
			else
			{
				TypeCode typeCode = expectedType.GetTypeCode();
				switch (typeCode)
				{
				case TypeCode.Boolean:
					value = DeserializeBooleanHelper(scalar.Value);
					break;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					value = DeserializeIntegerHelper(typeCode, scalar.Value);
					break;
				case TypeCode.Single:
					value = float.Parse(scalar.Value, YamlFormatter.NumberFormat);
					break;
				case TypeCode.Double:
					value = double.Parse(scalar.Value, YamlFormatter.NumberFormat);
					break;
				case TypeCode.Decimal:
					value = decimal.Parse(scalar.Value, YamlFormatter.NumberFormat);
					break;
				case TypeCode.String:
					value = scalar.Value;
					break;
				case TypeCode.Char:
					value = scalar.Value[0];
					break;
				case TypeCode.DateTime:
					value = DateTime.Parse(scalar.Value, CultureInfo.InvariantCulture);
					break;
				default:
					if (expectedType == typeof(object))
					{
						value = scalar.Value;
					}
					else
					{
						value = TypeConverter.ChangeType(scalar.Value, expectedType);
					}
					break;
				}
			}
			return true;
		}

		private object DeserializeBooleanHelper(string value)
		{
			bool flag;
			if (Regex.IsMatch(value, "^(true|y|yes|on)$", RegexOptions.IgnoreCase))
			{
				flag = true;
			}
			else
			{
				if (!Regex.IsMatch(value, "^(false|n|no|off)$", RegexOptions.IgnoreCase))
				{
					throw new FormatException($"The value \"{value}\" is not a valid YAML Boolean");
				}
				flag = false;
			}
			return flag;
		}

		private object DeserializeIntegerHelper(TypeCode typeCode, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			bool flag = false;
			int num = 0;
			ulong num2 = 0uL;
			if (value[0] == '-')
			{
				i++;
				flag = true;
			}
			else if (value[0] == '+')
			{
				i++;
			}
			if (value[i] == '0')
			{
				if (i == value.Length - 1)
				{
					num = 10;
					num2 = 0uL;
				}
				else
				{
					i++;
					if (value[i] == 'b')
					{
						num = 2;
						i++;
					}
					else if (value[i] == 'x')
					{
						num = 16;
						i++;
					}
					else
					{
						num = 8;
					}
				}
				for (; i < value.Length; i++)
				{
					if (value[i] != '_')
					{
						stringBuilder.Append(value[i]);
					}
				}
				switch (num)
				{
				case 2:
				case 8:
					num2 = Convert.ToUInt64(stringBuilder.ToString(), num);
					break;
				case 16:
					num2 = ulong.Parse(stringBuilder.ToString(), NumberStyles.HexNumber, YamlFormatter.NumberFormat);
					break;
				}
			}
			else
			{
				string[] array = value.Substring(i).Split(':');
				num2 = 0uL;
				for (int j = 0; j < array.Length; j++)
				{
					num2 *= 60;
					num2 += ulong.Parse(array[j].Replace("_", ""));
				}
			}
			if (flag)
			{
				return CastInteger(checked(-(long)num2), typeCode);
			}
			return CastInteger(num2, typeCode);
		}

		private static object CastInteger(long number, TypeCode typeCode)
		{
			return checked(typeCode switch
			{
				TypeCode.Byte => (byte)number, 
				TypeCode.Int16 => (short)number, 
				TypeCode.Int32 => (int)number, 
				TypeCode.Int64 => number, 
				TypeCode.SByte => (sbyte)number, 
				TypeCode.UInt16 => (ushort)number, 
				TypeCode.UInt32 => (uint)number, 
				TypeCode.UInt64 => (ulong)number, 
				_ => number, 
			});
		}

		private static object CastInteger(ulong number, TypeCode typeCode)
		{
			return checked(typeCode switch
			{
				TypeCode.Byte => (byte)number, 
				TypeCode.Int16 => (short)number, 
				TypeCode.Int32 => (int)number, 
				TypeCode.Int64 => (long)number, 
				TypeCode.SByte => (sbyte)number, 
				TypeCode.UInt16 => (ushort)number, 
				TypeCode.UInt32 => (uint)number, 
				TypeCode.UInt64 => number, 
				_ => number, 
			});
		}
	}
}
