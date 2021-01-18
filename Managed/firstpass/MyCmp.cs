using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MyCmp : MyAttributeManager<Component>
{
	public static void Init()
	{
		Dictionary<Type, MethodInfo> dictionary = new Dictionary<Type, MethodInfo>();
		dictionary.Add(typeof(MyCmpAdd), typeof(MyCmp).GetMethod("FindOrAddComponent"));
		dictionary.Add(typeof(MyCmpGet), typeof(MyCmp).GetMethod("FindComponent"));
		dictionary.Add(typeof(MyCmpReq), typeof(MyCmp).GetMethod("RequireComponent"));
		Dictionary<Type, MethodInfo> attributeMap = dictionary;
		MyAttributes.Register(new MyCmp(attributeMap, Util.SpawnComponent));
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
