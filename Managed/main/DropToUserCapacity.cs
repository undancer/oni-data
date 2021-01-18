using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DropToUserCapacity")]
public class DropToUserCapacity : Workable
{
	private Chore chore;

	private bool showCmd;

	private Storage[] storages;

	private static readonly EventSystem.IntraObjectHandler<DropToUserCapacity> OnStorageCapacityChangedHandler = new EventSystem.IntraObjectHandler<DropToUserCapacity>(delegate(DropToUserCapacity component, object data)
	{
		component.OnStorageChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DropToUserCapacity> OnStorageChangedHandler = new EventSystem.IntraObjectHandler<DropToUserCapacity>(delegate(DropToUserCapacity component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected DropToUserCapacity()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		Subscribe(-945020481, OnStorageCapacityChangedHandler);
		Subscribe(-1697596308, OnStorageChangedHandler);
		synchronizeAnims = false;
		SetWorkTime(0.1f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateChore();
	}

	private Storage[] GetStorages()
	{
		if (storages == null)
		{
			storages = GetComponents<Storage>();
		}
		return storages;
	}

	private void OnStorageChanged(object data)
	{
		UpdateChore();
	}

	public void UpdateChore()
	{
		IUserControlledCapacity component = GetComponent<IUserControlledCapacity>();
		if (component != null && component.AmountStored > component.UserMaxCapacity)
		{
			if (chore == null)
			{
				chore = new WorkChore<DropToUserCapacity>(Db.Get().ChoreTypes.EmptyStorage, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("Cancelled emptying");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(workerStatusItem);
			ShowProgressBar(show: false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = GetComponent<Storage>();
		IUserControlledCapacity component2 = GetComponent<IUserControlledCapacity>();
		float num = Mathf.Max(0f, component2.AmountStored - component2.UserMaxCapacity);
		List<GameObject> list = new List<GameObject>(component.items);
		for (int i = 0; i < list.Count; i++)
		{
			Pickupable component3 = list[i].GetComponent<Pickupable>();
			if (component3.PrimaryElement.Mass <= num)
			{
				num -= component3.PrimaryElement.Mass;
				component.Drop(component3.gameObject);
				continue;
			}
			Pickupable pickupable = component3.Take(num);
			pickupable.transform.SetPosition(base.transform.GetPosition());
			return;
		}
		chore = null;
	}
}
