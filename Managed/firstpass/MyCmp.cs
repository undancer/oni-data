using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MyCmp : MyAttributeManager<Component>
{
	public static void Init()
	{
		MyAttributes.Register(new MyCmp(new Dictionary<Type, MethodInfo>
		{
			{
				typeof(MyCmpAdd),
				typeof(MyCmp).GetMethod("FindOrAddComponent")
			},
			{
				typeof(MyCmpGet),
				typeof(MyCmp).GetMethod("FindComponent")
			},
			{
				typeof(MyCmpReq),
				typeof(MyCmp).GetMethod("RequireComponent")
			}
		}, Util.SpawnComponent));
	}

	public MyCmp(Dictionary<Type, MethodInfo> attributeMap, Action<Component> spawnFunc)
		: base(attributeMap, spawnFunc)
	{
	}

	public static Component FindComponent<T>(KMonoBehaviour c, bool isStart) where T : Component
	{
		return c.FindComponent<T>();
	}

	public static Component RequireComponent<T>(KMonoBehaviour c, bool isStart) where T : Component
	{
		if (isStart)
		{
			return c.RequireComponent<T>();
		}
		return c.FindComponent<T>();
	}

	public static Component FindOrAddComponent<T>(KMonoBehaviour c, bool isStart) where T : Component
	{
		return c.FindOrAddComponent<T>();
	}
}
