using KSerialization;
using STRINGS;
using UnityEngine;

public class CargoBayCluster : KMonoBehaviour, IUserControlledCapacity
{
	private MeterController meter;

	[SerializeField]
	public Storage storage;

	[SerializeField]
	public CargoBay.CargoType storageType;

	[Serialize]
	private float userMaxCapacity;

	private static readonly EventSystem.IntraObjectHandler<CargoBayCluster> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CargoBayCluster>(delegate(CargoBayCluster component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBayCluster> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CargoBayCluster>(delegate(CargoBayCluster component, object data)
	{
		component.OnStorageChange(data);
	});

	public float UserMaxCapacity
	{
		get
		{
			return userMaxCapacity;
		}
		set
		{
			userMaxCapacity = value;
			Trigger(-945020481, this);
		}
	}

	public float MinCapacity => 0f;

	public float MaxCapacity => storage.capacityKg;

	public float AmountStored => storage.MassStored();

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	public float RemainingCapacity => userMaxCapacity - storage.MassStored();

	protected override void OnPrefabInit()
	{
		userMaxCapacity = storage.capacityKg;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		OnStorageChange(null);
		Subscribe(-1697596308, OnStorageChangeDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, delegate
		{
			storage.DropAll();
		}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP);
		Game.Instance.userMenu.AddButton(base.gameObject, button);
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(storage.MassStored() / storage.Capacity());
		UpdateCargoStatusItem();
	}

	private void UpdateCargoStatusItem()
	{
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		if (component == null)
		{
			return;
		}
		CraftModuleInterface craftInterface = component.CraftInterface;
		if (!(craftInterface == null))
		{
			Clustercraft component2 = craftInterface.GetComponent<Clustercraft>();
			if (!(component2 == null))
			{
				component2.UpdateStatusItem();
			}
		}
	}
}
