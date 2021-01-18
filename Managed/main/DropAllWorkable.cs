using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DropAllWorkable")]
public class DropAllWorkable : Workable
{
	private Chore chore;

	private bool showCmd;

	private Storage[] storages;

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
		SetWorkTime(0.1f);
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
	}

	public void DropAll()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
			return;
		}
		if (chore == null)
		{
			chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
			return;
		}
		chore.Cancel("Cancelled emptying");
		chore = null;
		GetComponent<KSelectable>().RemoveStatusItem(workerStatusItem);
		ShowProgressBar(show: false);
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
		chore = null;
		Trigger(-1957399615);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (showCmd)
		{
			KIconButtonMenu.ButtonInfo button = ((chore == null) ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, DropAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF, DropAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF));
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
}
