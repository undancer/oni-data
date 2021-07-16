using System;

namespace YamlDotNet.Serialization.TypeResolvers
{
	public sealed class DynamicTypeResolver : ITypeResolver
	{
		public Type Resolve(Type staticType, object actualValue)
		{
			if (actualValue == null)
			{
				return staticType;
			}
			return actualValue.GetType();
		}
	}
}
