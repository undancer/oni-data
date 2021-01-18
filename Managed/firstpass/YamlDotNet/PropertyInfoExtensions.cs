using System.Reflection;

namespace YamlDotNet
{
	internal static class PropertyInfoExtensions
	{
		public static object ReadValue(this PropertyInfo property, object target)
		{
			return property.GetGetMethod().Invoke(target, null);
		}
	}
}
