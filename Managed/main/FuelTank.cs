using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class FuelTank : KMonoBehaviour, IUserControlledCapacity, IFuelTank
{
	public Storage storage;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = -1f;

	[SerializeField]
	public float physicalFuelCapacity;

	public bool consumeFuelOnLand;

	[SerializeField]
	private Tag fuelType;

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnRocketLandedDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnRocketLanded(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnStorageChange(data);
	});

	public IStorage Storage => storage;

	public bool ConsumeFuelOnLand => consumeFuelOnLand;

	public float UserMaxCapacity
	{
		get
		{
			return targetFillMass;
		}
		set
		{
			targetFillMass = value;
			storage.capacityKg = targetFillMass;
			ConduitConsumer component = GetComponent<ConduitConsumer>();
			if (component != null)
			{
				component.capacityKG = targetFillMass;
			}
			ManualDeliveryKG component2 = GetComponent<ManualDeliveryKG>();
			if (component2 != null)
			{
				component2.capacity = (component2.refillMass = targetFillMass);
			}
			Trigger(-945020481, this);
		}
	}

	public float MinCapacity => 0f;

	public float MaxCapacity => physicalFuelCapacity;

	public float AmountStored => storage.MassStored();

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	public Tag FuelType
	{
		get
		{
			return fuelType;
		}
		set
		{
			fuelType = value;
			if (storage.storageFilters == null)
			{
				storage.storageFilters = new List<Tag>();
			}
			storage.storageFilters.Add(fuelType);
			ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
			if (component != null)
			{
				component.requestedItemTag = fuelType;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (targetFillMass == -1f)
		{
			targetFillMass = physicalFuelCapacity;
		}
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionProperlyFueled(this));
		}
		Subscribe(-887025858, OnRocketLandedDelegate);
		UserMaxCapacity = UserMaxCapacity;
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		OnStorageChange(null);
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
	}

	private void OnRocketLanded(object data)
	{
		if (ConsumeFuelOnLand)
		{
			storage.ConsumeAllIgnoringDisease();
		}
	}

	private void OnCopySettings(object data)
	{
		FuelTank component = ((GameObject)data).GetComponent<FuelTank>();
		if (component != null)
		{
			UserMaxCapacity = component.UserMaxCapacity;
		}
	}

	public void DEBUG_FillTank()
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			RocketEngineCluster rocketEngineCluster = null;
			foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
			{
				rocketEngineCluster = item.GetComponent<RocketEngineCluster>();
				if (rocketEngineCluster != null && rocketEngineCluster.mainEngine)
				{
					break;
				}
			}
			if (rocketEngineCluster != null)
			{
				Element element = ElementLoader.GetElement(rocketEngineCluster.fuelTag);
				if (element.IsLiquid)
				{
					storage.AddLiquid(element.id, targetFillMass - storage.MassStored(), element.defaultValues.temperature, 0, 0);
				}
				else if (element.IsGas)
				{
					storage.AddGasChunk(element.id, targetFillMass - storage.MassStored(), element.defaultValues.temperature, 0, 0, keep_zero_mass: false);
				}
				else if (element.IsSolid)
				{
					storage.AddOre(element.id, targetFillMass - storage.MassStored(), element.defaultValues.temperature, 0, 0);
				}
				rocketEngineCluster.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().UpdateStatusItem();
			}
			else
			{
				Debug.LogWarning("Fuel tank couldn't find rocket engine");
			}
			return;
		}
		RocketEngine rocketEngine = null;
		foreach (GameObject item2 in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			rocketEngine = item2.GetComponent<RocketEngine>();
			if (rocketEngine != null && rocketEngine.mainEngine)
			{
				break;
			}
		}
		if (rocketEngine != null)
		{
			Element element2 = ElementLoader.GetElement(rocketEngine.fuelTag);
			if (element2.IsLiquid)
			{
				storage.AddLiquid(element2.id, targetFillMass - storage.MassStored(), element2.defaultValues.temperature, 0, 0);
			}
			else if (element2.IsGas)
			{
				storage.AddGasChunk(element2.id, targetFillMass - storage.MassStored(), element2.defaultValues.temperature, 0, 0, keep_zero_mass: false);
			}
			else if (element2.IsSolid)
			{
				storage.AddOre(element2.id, targetFillMass - storage.MassStored(), element2.defaultValues.temperature, 0, 0);
			}
		}
		else
		{
			Debug.LogWarning("Fuel tank couldn't find rocket engine");
		}
	}
}
