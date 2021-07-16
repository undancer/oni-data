using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class KMonoBehaviourExtensions
{
	public static int Subscribe(this GameObject go, int hash, Action<object> handler)
	{
		return go.GetComponent<KMonoBehaviour>().Subscribe(hash, handler);
	}

	public static void Subscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
	{
		go.GetComponent<KMonoBehaviour>().Subscribe(target, hash, handler);
	}

	public static void Unsubscribe(this GameObject go, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Unsubscribe(hash, handler);
		}
	}

	public static void Unsubscribe(this GameObject go, int id)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Unsubscribe(id);
		}
	}

	public static void Unsubscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Unsubscribe(target, hash, handler);
		}
	}

	public static T GetComponentInChildrenOnly<T>(this GameObject go) where T : Component
	{
		T[] componentsInChildren = go.GetComponentsInChildren<T>();
		foreach (T val in componentsInChildren)
		{
			if (val.gameObject != go)
			{
				return val;
			}
		}
		return null;
	}

	public static T[] GetComponentsInChildrenOnly<T>(this GameObject go) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(go.GetComponentsInChildren<T>());
		list.RemoveAll((T t) => t.gameObject == go);
		return list.ToArray();
	}

	public static void SetAlpha(this Image img, float alpha)
	{
		Color color = img.color;
		color.a = alpha;
		img.color = color;
	}

	public static void SetAlpha(this Text txt, float alpha)
	{
		Color color = txt.color;
		color.a = alpha;
		txt.color = color;
	}
}
