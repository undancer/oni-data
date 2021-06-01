using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DropAllWorkable")]
public class DropAllWorkable : Workable
{
	[Serialize]
	private bool markedForDrop;

	private Chore _chore;

	private bool showCmd;

	private Storage[] storages;

	public float dropWorkTime = 0.1f;

	public string choreTypeID;

	[MyCmpAdd]
	private Prioritizable _prioritizable;

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnStorageChange(data);
	});

	private Guid statusItem;

	private Chore Chore
	{
		get
		{
			return _chore;
		}
		set
		{
			_chore = value;
			markedForDrop = _chore != null;
		}
	}

	protected DropAllWorkable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		synchronizeAnims = false;
		SetWorkTime(dropWorkTime);
		Prioritizable.AddRef(base.gameObject);
	}

	private Storage[] GetStorages()
	{
		if (storages == null)
		{
			storages = GetComponents<Storage>();
		}
		return storages;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		showCmd = GetNewShowCmd();
		if (markedForDrop)
		{
			DropAll();
		}
	}

	public void DropAll()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
		}
		else if (Chore == null)
		{
			ChoreType chore_type = ((!string.IsNullOrEmpty(choreTypeID)) ? Db.Get().ChoreTypes.Get(choreTypeID) : Db.Get().ChoreTypes.EmptyStorage);
			Chore = new WorkChore<DropAllWorkable>(chore_type, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
		}
		else
		{
			Chore.Cancel("Cancelled emptying");
			Chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(workerStatusItem);
			ShowProgressBar(show: false);
		}
		RefreshStatusItem();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage[] array = GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			List<GameObject> list = new List<GameObject>(array[i].items);
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = array[i].Drop(list[j]);
				if (gameObject != null)
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if (component != null)
					{
						component.TryToOffsetIfBuried();
					}
				}
			}
		}
		Chore = null;
		RefreshStatusItem();
		Trigger(-1957399615);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (showCmd)
		{
			KIconButtonMenu.ButtonInfo button = ((Chore == null) ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, DropAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF, DropAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	private bool GetNewShowCmd()
	{
		bool flag = false;
		Storage[] array = GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			flag = flag || !array[i].IsEmpty();
		}
		return flag;
	}

	private void OnStorageChange(object data)
	{
		bool newShowCmd = GetNewShowCmd();
		if (newShowCmd != showCmd)
		{
			showCmd = newShowCmd;
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}

	private void RefreshStatusItem()
	{
		if (Chore != null && statusItem == Guid.Empty)
		{
			KSelectable component = GetComponent<KSelectable>();
			statusItem = component.AddStatusItem(Db.Get().BuildingStatusItems.AwaitingEmptyBuilding);
		}
		else if (Chore == null && statusItem != Guid.Empty)
		{
			KSelectable component2 = GetComponent<KSelectable>();
			statusItem = component2.RemoveStatusItem(statusItem);
		}
	}
}
