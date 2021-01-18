using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class CachedTypeInspector : TypeInspectorSkeleton
	{
		private readonly ITypeInspector innerTypeDescriptor;

		private readonly Dictionary<Type, List<IPropertyDescriptor>> cache = new Dictionary<Type, List<IPropertyDescriptor>>();

		public CachedTypeInspector(ITypeInspector innerTypeDescriptor)
		{
			if (innerTypeDescriptor == null)
			{
				throw new ArgumentNullException("innerTypeDescriptor");
			}
			this.innerTypeDescriptor = innerTypeDescriptor;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			if (!cache.TryGetValue(type, out var value))
			{
				return new List<IPropertyDescriptor>(innerTypeDescriptor.GetProperties(type, container));
			}
			return value;
		}
	}
}
