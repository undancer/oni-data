using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Bed")]
public class Bed : Workable, IGameObjectEffectDescriptor, IBasicBuilding
{
	[MyCmpReq]
	private Sleepable sleepable;

	private Worker targetWorker;

	public string[] effects;

	private static Dictionary<string, string> roomSleepingEffects = new Dictionary<string, string>
	{
		{ "Barracks", "BarracksStamina" },
		{ "Bedroom", "BedroomStamina" }
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.BasicBuildings.Add(this);
		sleepable = GetComponent<Sleepable>();
		Sleepable obj = sleepable;
		obj.OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(obj.OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
	}

	private void OnWorkableEvent(WorkableEvent workable_event)
	{
		switch (workable_event)
		{
		case WorkableEvent.WorkStarted:
			AddEffects();
			break;
		case WorkableEvent.WorkStopped:
			RemoveEffects();
			break;
		}
	}

	private void AddEffects()
	{
		targetWorker = sleepable.worker;
		if (effects != null)
		{
			string[] array = effects;
			foreach (string effect_id in array)
			{
				targetWorker.GetComponent<Effects>().Add(effect_id, should_save: false);
			}
		}
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject == null)
		{
			return;
		}
		RoomType roomType = roomOfGameObject.roomType;
		foreach (KeyValuePair<string, string> roomSleepingEffect in roomSleepingEffects)
		{
			if (roomSleepingEffect.Key == roomType.Id)
			{
				targetWorker.GetComponent<Effects>().Add(roomSleepingEffect.Value, should_save: false);
			}
		}
		roomType.TriggerRoomEffects(GetComponent<KPrefabID>(), targetWorker.GetComponent<Effects>());
	}

	private void RemoveEffects()
	{
		if (targetWorker == null)
		{
			return;
		}
		if (effects != null)
		{
			string[] array = effects;
			foreach (string effect_id in array)
			{
				targetWorker.GetComponent<Effects>().Remove(effect_id);
			}
		}
		foreach (KeyValuePair<string, string> roomSleepingEffect in roomSleepingEffects)
		{
			targetWorker.GetComponent<Effects>().Remove(roomSleepingEffect.Value);
		}
		targetWorker = null;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (effects != null)
		{
			string[] array = effects;
			foreach (string text in array)
			{
				if (text != null && text != "")
				{
					Effect.AddModifierDescriptions(base.gameObject, list, text);
				}
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.BasicBuildings.Remove(this);
		if (sleepable != null)
		{
			Sleepable obj = sleepable;
			obj.OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Remove(obj.OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
		}
	}
}
