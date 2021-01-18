using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/KBatchedAnimEventToggler")]
public class KBatchedAnimEventToggler : KMonoBehaviour
{
	[Serializable]
	public struct Entry
	{
		public string anim;

		public HashedString context;

		public KBatchedAnimController controller;
	}

	[SerializeField]
	public GameObject eventSource;

	[SerializeField]
	public string enableEvent;

	[SerializeField]
	public string disableEvent;

	[SerializeField]
	public List<Entry> entries;

	private AnimEventHandler animEventHandler;

	protected override void OnPrefabInit()
	{
		Vector3 position = eventSource.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		int layer = LayerMask.NameToLayer("Default");
		foreach (Entry entry in entries)
		{
			entry.controller.transform.SetPosition(position);
			entry.controller.SetLayer(layer);
			entry.controller.gameObject.SetActive(value: false);
		}
		int hash = Hash.SDBMLower(enableEvent);
		int hash2 = Hash.SDBMLower(disableEvent);
		Subscribe(eventSource, hash, Enable);
		Subscribe(eventSource, hash2, Disable);
	}

	protected override void OnSpawn()
	{
		animEventHandler = GetComponentInParent<AnimEventHandler>();
	}

	private void Enable(object data)
	{
		StopAll();
		HashedString context = animEventHandler.GetContext();
		if (!context.IsValid)
		{
			return;
		}
		foreach (Entry entry in entries)
		{
			if (entry.context == context)
			{
				entry.controller.gameObject.SetActive(value: true);
				entry.controller.Play(entry.anim, KAnim.PlayMode.Loop);
			}
		}
	}

	private void Disable(object data)
	{
		StopAll();
	}

	private void StopAll()
	{
		foreach (Entry entry in entries)
		{
			entry.controller.StopAndClear();
			entry.controller.gameObject.SetActive(value: false);
		}
	}
}
