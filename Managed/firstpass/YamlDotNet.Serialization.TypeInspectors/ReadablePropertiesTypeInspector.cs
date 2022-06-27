using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class ReadablePropertiesTypeInspector : TypeInspectorSkeleton
	{
		private sealed class ReflectionPropertyDescriptor : IPropertyDescriptor
		{
			private readonly PropertyInfo _propertyInfo;

			private readonly ITypeResolver _typeResolver;

			public string Name => _propertyInfo.Name;

			public Type Type => _propertyInfo.PropertyType;

			public Type TypeOverride { get; set; }

			public int Order { get; set; }

			public bool CanWrite => _propertyInfo.CanWrite;

			public ScalarStyle ScalarStyle { get; set; }

			public ReflectionPropertyDescriptor(PropertyInfo propertyInfo, ITypeResolver typeResolver)
			{
				_propertyInfo = propertyInfo;
				_typeResolver = typeResolver;
				ScalarStyle = ScalarStyle.Any;
			}

			public void Write(object target, object value)
			{
				_propertyInfo.SetValue(target, value, null);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				return (T)_propertyInfo.GetCustomAttributes(typeof(T), inherit: true).FirstOrDefault();
			}

			public IObjectDescriptor Read(object target)
			{
				object obj = _propertyInfo.ReadValue(target);
				Type type = TypeOverride ?? _typeResolver.Resolve(Type, obj);
				return new ObjectDescriptor(obj, type, Type, ScalarStyle);
			}
		}

		private readonly ITypeResolver _typeResolver;

		public ReadablePropertiesTypeInspector(ITypeResolver typeResolver)
		{
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			_typeResolver = typeResolver;
		}

		private static bool IsValidProperty(PropertyInfo property)
		{
			if (property.CanRead)
			{
				return property.GetGetMethod().GetParameters().Length == 0;
			}
			return false;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return type.GetPublicProperties().Where(IsValidProperty).Select((Func<PropertyInfo, IPropertyDescriptor>)((PropertyInfo p) => new ReflectionPropertyDescriptor(p, _typeResolver)));
		}
	}
}
