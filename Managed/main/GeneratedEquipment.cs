using System;
using System.Collections.Generic;

public class GeneratedEquipment
{
	public static void LoadGeneratedEquipment(List<Type> types)
	{
		Type typeFromHandle = typeof(IEquipmentConfig);
		List<Type> list = new List<Type>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				list.Add(type);
			}
		}
		foreach (Type item in list)
		{
			object obj = Activator.CreateInstance(item);
			try
			{
				EquipmentConfigManager.Instance.RegisterEquipment(obj as IEquipmentConfig);
			}
			catch (Exception e)
			{
				DebugUtil.LogException(null, "Exception in RegisterEquipment for type " + item.FullName + " from " + item.Assembly.GetName().Name, e);
			}
		}
	}
}
