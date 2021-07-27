using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class ReflectionUtility
	{
		public static Type GetImplementedGenericInterface(Type type, Type genericInterfaceType)
		{
			foreach (Type implementedInterface in GetImplementedInterfaces(type))
			{
				if (implementedInterface.IsGenericType() && implementedInterface.GetGenericTypeDefinition() == genericInterfaceType)
				{
					return implementedInterface;
				}
			}
			return null;
		}

		public static IEnumerable<Type> GetImplementedInterfaces(Type type)
		{
			if (type.IsInterface())
			{
				yield return type;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				yield return interfaces[i];
			}
		}
	}
}
