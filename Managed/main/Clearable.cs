using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Clearable")]
public class Clearable : Workable, ISaveLoadable, IRender1000ms
{
	[MyCmpReq]
	private Pickupable pickupable;

	[MyCmpReq]
	private KSelectable selectable;

	[Serialize]
	private bool isMarkedForClear;

	private HandleVector<int>.Handle clearHandle;

	public bool isClearable = true;

	private Guid pendingClearGuid;

	private Guid pendingClearNoStorageGuid;

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnEquippedDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnEquipped(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(856640610, OnStoreDelegate);
		Subscribe(-2064133523, OnAbsorbDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1617557748, OnEquippedDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Clearing;
		simRenderLoadBalance = true;
		autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForClear)
		{
			if (this.HasTag(GameTags.Stored))
			{
				if (!base.transform.parent.GetComponent<Storage>().allowClearable)
				{
					isMarkedForClear = false;
				}
				else
				{
					MarkForClear(restoringFromSave: true, allowWhenStored: true);
				}
			}
			else
			{
				MarkForClear(restoringFromSave: true);
			}
		}
		RefreshClearableStatus(force_update: true);
	}

	private void OnStore(object data)
	{
		CancelClearing();
	}

	private void OnCancel(object data)
	{
		for (ObjectLayerListItem objectLayerListItem = pickupable.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
		{
			if (objectLayerListItem.gameObject != null)
			{
				objectLayerListItem.gameObject.GetComponent<Clearable>().CancelClearing();
			}
		}
	}

	public void CancelClearing()
	{
		if (isMarkedForClear)
		{
			isMarkedForClear = false;
			GetComponent<KPrefabID>().RemoveTag(GameTags.Garbage);
			Prioritizable.RemoveRef(base.gameObject);
			if (clearHandle.IsValid())
			{
				GlobalChoreProvider.Instance.UnregisterClearable(clearHandle);
				clearHandle.Clear();
			}
			RefreshClearableStatus(force_update: true);
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void MarkForClear(bool restoringFromSave = false, bool allowWhenStored = false)
	{
		if (isClearable && (!isMarkedForClear || restoringFromSave) && !pickupable.IsEntombed && !clearHandle.IsValid() && (!this.HasTag(GameTags.Stored) || allowWhenStored))
		{
			Prioritizable.AddRef(base.gameObject);
			GetComponent<KPrefabID>().AddTag(GameTags.Garbage);
			isMarkedForClear = true;
			clearHandle = GlobalChoreProvider.Instance.RegisterClearable(this);
			RefreshClearableStatus(force_update: true);
			SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
		}
	}

	private void OnClickClear()
	{
		MarkForClear();
	}

	private void OnClickCancel()
	{
		CancelClearing();
	}

	private void OnEquipped(object data)
	{
		CancelClearing();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (clearHandle.IsValid())
		{
			GlobalChoreProvider.Instance.UnregisterClearable(clearHandle);
			clearHandle.Clear();
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (isClearable && !(GetComponent<Health>() != null) && !this.HasTag(GameTags.Stored))
		{
			KIconButtonMenu.ButtonInfo button = (isMarkedForClear ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME_OFF, OnClickCancel, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME, OnClickClear, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEAR.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Clearable component = pickupable.GetComponent<Clearable>();
			if (component != null && component.isMarkedForClear)
			{
				MarkForClear();
			}
		}
	}

	public void Render1000ms(float dt)
	{
		RefreshClearableStatus(force_update: false);
	}

	public void RefreshClearableStatus(bool force_update)
	{
		if (force_update || isMarkedForClear)
		{
			bool show = false;
			bool show2 = false;
			if (isMarkedForClear)
			{
				show2 = !(show = GlobalChoreProvider.Instance.ClearableHasDestination(pickupable));
			}
			pendingClearGuid = selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, pendingClearGuid, show, this);
			pendingClearNoStorageGuid = selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, pendingClearNoStorageGuid, show2, this);
		}
	}
}
