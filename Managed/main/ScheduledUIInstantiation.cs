using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduledUIInstantiation")]
public class ScheduledUIInstantiation : KMonoBehaviour
{
	[Serializable]
	public struct Instantiation
	{
		public string Name;

		public string Comment;

		public GameObject[] prefabs;

		public Transform parent;
	}

	public Instantiation[] UIElements;

	public bool InstantiateOnAwake;

	public GameHashes InstantiationEvent = GameHashes.StartGameUser;

	private bool completed;

	private List<GameObject> instantiatedObjects = new List<GameObject>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (InstantiateOnAwake)
		{
			InstantiateElements(null);
		}
		else
		{
			Game.Instance.Subscribe((int)InstantiationEvent, InstantiateElements);
		}
	}

	public void InstantiateElements(object data)
	{
		if (completed)
		{
			return;
		}
		completed = true;
		Instantiation[] uIElements = UIElements;
		for (int i = 0; i < uIElements.Length; i++)
		{
			Instantiation instantiation = uIElements[i];
			GameObject[] prefabs = instantiation.prefabs;
			foreach (GameObject obj in prefabs)
			{
				Vector3 v = obj.rectTransform().anchoredPosition;
				GameObject gameObject = Util.KInstantiateUI(obj, instantiation.parent.gameObject);
				gameObject.rectTransform().anchoredPosition = v;
				gameObject.rectTransform().localScale = Vector3.one;
				instantiatedObjects.Add(gameObject);
			}
		}
		if (!InstantiateOnAwake)
		{
			Unsubscribe((int)InstantiationEvent, InstantiateElements);
		}
	}

	public T GetInstantiatedObject<T>() where T : Component
	{
		for (int i = 0; i < instantiatedObjects.Count; i++)
		{
			if (instantiatedObjects[i].GetComponent(typeof(T)) != null)
			{
				return instantiatedObjects[i].GetComponent(typeof(T)) as T;
			}
		}
		return null;
	}
}
