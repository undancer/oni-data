using System;
using System.Reflection;
using UnityEngine;

public class CmpFns
{
	public Func<KMonoBehaviour, Component> mFindOrAddFn;

	public Func<KMonoBehaviour, Component> mFindFn;

	public Func<KMonoBehaviour, Component> mRequireFn;

	public static Component FindComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.FindComponent<T>();
	}

	public static Component RequireComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.RequireComponent<T>();
	}

	public static Component FindOrAddComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.FindOrAddComponent<T>();
	}

	public CmpFns(Type type)
	{
		Type[] type_array = new Type[1]
		{
			type
		};
		mFindOrAddFn = GetMethod("FindOrAddComponent", type_array);
		mFindFn = GetMethod("FindComponent", type_array);
		mRequireFn = GetMethod("RequireComponent", type_array);
	}

	private Func<KMonoBehaviour, Component> GetMethod(string name, Type[] type_array)
	{
		MethodInfo method = typeof(CmpFns).GetMethod(name);
		MethodInfo method2 = null;
		try
		{
			method2 = method.MakeGenericMethod(type_array);
		}
		catch (Exception obj)
		{
			Debug.LogError(obj);
			for (int i = 0; i < type_array.Length; i++)
			{
				Debug.Log(type_array[i]);
			}
		}
		return (Func<KMonoBehaviour, Component>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, Component>), method2);
	}
}
