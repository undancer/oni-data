using System;
using System.Globalization;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class StandardProperty<T> : DictionaryProperty<T>
	{
		private static readonly StandardType Type = ParseType(typeof(T));

		private static readonly string TypeString = TypeToGraphML(Type);

		public StandardProperty()
		{
		}

		internal StandardProperty(XElement xKey)
			: this()
		{
			XAttribute xAttribute = xKey.Attribute("attr.type");
			if (xAttribute == null || xAttribute.Value != TypeString)
			{
				throw new ArgumentException("Key not compatible with property.");
			}
			LoadFromKeyElement(xKey);
		}

		private static StandardType ParseType(Type t)
		{
			if (t == typeof(bool))
			{
				return StandardType.Bool;
			}
			if (t == typeof(double))
			{
				return StandardType.Double;
			}
			if (t == typeof(float))
			{
				return StandardType.Float;
			}
			if (t == typeof(int))
			{
				return StandardType.Int;
			}
			if (t == typeof(long))
			{
				return StandardType.Long;
			}
			if (t == typeof(string))
			{
				return StandardType.String;
			}
			throw new ArgumentException("Invalid type for a standard GraphML property.");
		}

		private static string TypeToGraphML(StandardType type)
		{
			return type switch
			{
				StandardType.Bool => "boolean", 
				StandardType.Double => "double", 
				StandardType.Float => "float", 
				StandardType.Int => "int", 
				StandardType.Long => "long", 
				_ => "string", 
			};
		}

		private static object ParseValue(string value)
		{
			return Type switch
			{
				StandardType.Bool => value == "true", 
				StandardType.Double => double.Parse(value, CultureInfo.InvariantCulture), 
				StandardType.Float => float.Parse(value, CultureInfo.InvariantCulture), 
				StandardType.Int => int.Parse(value, CultureInfo.InvariantCulture), 
				StandardType.Long => long.Parse(value, CultureInfo.InvariantCulture), 
				_ => value, 
			};
		}

		public override XElement GetKeyElement()
		{
			XElement keyElement = base.GetKeyElement();
			keyElement.SetAttributeValue("attr.type", TypeString);
			return keyElement;
		}

		protected override T ReadValue(XElement x)
		{
			return (T)ParseValue(x.Value);
		}

		protected override XElement WriteValue(T value)
		{
			return new XElement("dummy", value.ToString());
		}
	}
}
