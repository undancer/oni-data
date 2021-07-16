using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Helpers;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributeOverrides
	{
		private struct AttributeKey
		{
			public readonly Type AttributeType;

			public readonly string PropertyName;

			public AttributeKey(Type attributeType, string propertyName)
			{
				AttributeType = attributeType;
				PropertyName = propertyName;
			}

			public override bool Equals(object obj)
			{
				AttributeKey attributeKey = (AttributeKey)obj;
				if (AttributeType.Equals(attributeKey.AttributeType))
				{
					return PropertyName.Equals(attributeKey.PropertyName);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(AttributeType.GetHashCode(), PropertyName.GetHashCode());
			}
		}

		private sealed class AttributeMapping
		{
			public readonly Type RegisteredType;

			public readonly Attribute Attribute;

			public AttributeMapping(Type registeredType, Attribute attribute)
			{
				RegisteredType = registeredType;
				Attribute = attribute;
			}

			public override bool Equals(object obj)
			{
				AttributeMapping attributeMapping = obj as AttributeMapping;
				if (attributeMapping != null && RegisteredType.Equals(attributeMapping.RegisteredType))
				{
					return Attribute.Equals(attributeMapping.Attribute);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(RegisteredType.GetHashCode(), Attribute.GetHashCode());
			}

			public int Matches(Type matchType)
			{
				int num = 0;
				Type type = matchType;
				while (type != null)
				{
					num++;
					if (type == RegisteredType)
					{
						return num;
					}
					type = type.BaseType();
				}
				if (matchType.GetInterfaces().Contains(RegisteredType))
				{
					return num;
				}
				return 0;
			}
		}

		private readonly Dictionary<AttributeKey, List<AttributeMapping>> overrides = new Dictionary<AttributeKey, List<AttributeMapping>>();

		public T GetAttribute<T>(Type type, string member) where T : Attribute
		{
			if (overrides.TryGetValue(new AttributeKey(typeof(T), member), out var value))
			{
				int num = 0;
				AttributeMapping attributeMapping = null;
				foreach (AttributeMapping item in value)
				{
					int num2 = item.Matches(type);
					if (num2 > num)
					{
						num = num2;
						attributeMapping = item;
					}
				}
				if (num > 0)
				{
					return (T)attributeMapping.Attribute;
				}
			}
			return null;
		}

		public void Add(Type type, string member, Attribute attribute)
		{
			AttributeMapping item = new AttributeMapping(type, attribute);
			AttributeKey key = new AttributeKey(attribute.GetType(), member);
			if (!overrides.TryGetValue(key, out var value))
			{
				value = new List<AttributeMapping>();
				overrides.Add(key, value);
			}
			else if (value.Contains(item))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Attribute ({2}) already set for Type {0}, Member {1}", type.FullName, member, attribute));
			}
			value.Add(item);
		}

		public void Add<TClass>(Expression<Func<TClass, object>> propertyAccessor, Attribute attribute)
		{
			PropertyInfo propertyInfo = propertyAccessor.AsProperty();
			Add(typeof(TClass), propertyInfo.Name, attribute);
		}

		public YamlAttributeOverrides Clone()
		{
			YamlAttributeOverrides yamlAttributeOverrides = new YamlAttributeOverrides();
			foreach (KeyValuePair<AttributeKey, List<AttributeMapping>> @override in overrides)
			{
				foreach (AttributeMapping item in @override.Value)
				{
					yamlAttributeOverrides.Add(item.RegisteredType, @override.Key.PropertyName, item.Attribute);
				}
			}
			return yamlAttributeOverrides;
		}
	}
}
