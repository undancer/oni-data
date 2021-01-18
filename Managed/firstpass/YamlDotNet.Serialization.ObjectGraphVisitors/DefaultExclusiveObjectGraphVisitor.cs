using System;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private static readonly IEqualityComparer<object> _objectComparer = EqualityComparer<object>.Default;

		public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
			: base(nextVisitor)
		{
		}

		private static object GetDefault(Type type)
		{
			if (!type.IsValueType())
			{
				return null;
			}
			return Activator.CreateInstance(type);
		}

		public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			if (!_objectComparer.Equals(value, GetDefault(value.Type)))
			{
				return base.EnterMapping(key, value, context);
			}
			return false;
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
			object y = ((customAttribute != null) ? customAttribute.Value : GetDefault(key.Type));
			if (!_objectComparer.Equals(value.Value, y))
			{
				return base.EnterMapping(key, value, context);
			}
			return false;
		}
	}
}
