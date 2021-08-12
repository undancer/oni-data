using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RoomTracker")]
public class RoomTracker : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public enum Requirement
	{
		TrackingOnly,
		Recommended,
		Required,
		CustomRecommended,
		CustomRequired
	}

	public Requirement requirement;

	public string requiredRoomType;

	public string customStatusItemID;

	private Guid statusItemGuid;

	private static readonly EventSystem.IntraObjectHandler<RoomTracker> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<RoomTracker>(delegate(RoomTracker component, object data)
	{
		component.OnUpdateRoom(data);
	});

	public Room room { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Debug.Assert(!string.IsNullOrEmpty(requiredRoomType) && requiredRoomType != Db.Get().RoomTypes.Neutral.Id, "RoomTracker must have a requiredRoomType!");
		Subscribe(144050788, OnUpdateRoomDelegate);
		FindAndSetRoom();
	}

	public void FindAndSetRoom()
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(base.gameObject));
		if (cavityForCell != null && cavityForCell.room != null)
		{
			OnUpdateRoom(cavityForCell.room);
		}
		else
		{
			OnUpdateRoom(null);
		}
	}

	public bool IsInCorrectRoom()
	{
		if (room != null)
		{
			return room.roomType.Id == requiredRoomType;
		}
		return false;
	}

	public bool SufficientBuildLocation(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if ((requirement == Requirement.Required || requirement == Requirement.CustomRequired) && Game.Instance.roomProber.GetCavityForCell(cell)?.room == null)
		{
			return false;
		}
		return true;
	}

	private void OnUpdateRoom(object data)
	{
		room = (Room)data;
		if (room == null || room.roomType.Id != requiredRoomType)
		{
			switch (requirement)
			{
			case Requirement.Recommended:
				statusItemGuid = GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.RequiredRoom, Db.Get().BuildingStatusItems.NotInRecommendedRoom, requiredRoomType);
				break;
			case Requirement.Required:
				statusItemGuid = GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.RequiredRoom, Db.Get().BuildingStatusItems.NotInRequiredRoom, requiredRoomType);
				break;
			case Requirement.CustomRecommended:
			case Requirement.CustomRequired:
				statusItemGuid = GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.RequiredRoom, Db.Get().BuildingStatusItems.Get(customStatusItemID), requiredRoomType);
				break;
			case Requirement.TrackingOnly:
				statusItemGuid = GetComponent<KSelectable>().RemoveStatusItem(statusItemGuid);
				break;
			}
		}
		else
		{
			statusItemGuid = GetComponent<KSelectable>().RemoveStatusItem(statusItemGuid);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (!string.IsNullOrEmpty(requiredRoomType))
		{
			string arg = Db.Get().RoomTypes.Get(requiredRoomType).Name;
			switch (requirement)
			{
			case Requirement.Recommended:
			case Requirement.CustomRecommended:
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.PREFERS_ROOM, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PREFERS_ROOM, arg), Descriptor.DescriptorType.Requirement));
				break;
			case Requirement.Required:
			case Requirement.CustomRequired:
				list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESROOM, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESROOM, arg), Descriptor.DescriptorType.Requirement));
				break;
			}
		}
		return list;
	}
}
