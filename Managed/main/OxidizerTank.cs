using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OxidizerTank")]
public class OxidizerTank : KMonoBehaviour, IUserControlledCapacity
{
	public Storage storage;

	private MeterController meter;

	private bool isSuspended = false;

	public bool consumeOnLand = true;

	[Serialize]
	public float targetFillMass = 2700f;

	[SerializeField]
	private Tag[] oxidizerTypes = new Tag[3]
	{
		SimHashes.OxyRock.CreateTag(),
		SimHashes.LiquidOxygen.CreateTag(),
		SimHashes.Fertilizer.CreateTag()
	};

	private FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<OxidizerTank> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OxidizerTank>(delegate(OxidizerTank component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<OxidizerTank> OnRocketLandedDelegate = new EventSystem.IntraObjectHandler<OxidizerTank>(delegate(OxidizerTank component, object data)
	{
		component.OnRocketLanded(data);
	});

	private static readonly EventSystem.IntraObjectHandler<OxidizerTank> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<OxidizerTank>(delegate(OxidizerTank component, object data)
	{
		component.OnStorageChange(data);
	});

	public bool IsSuspended => isSuspended;

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

	public float MaxCapacity => 2700f;

	public float AmountStored => storage.MassStored();

	public float TotalOxidizerPower
	{
		get
		{
			float num = 0f;
			foreach (GameObject item in storage.items)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				num += component.Mass * RocketStats.oxidizerEfficiencies[component.ElementID.CreateTag()];
			}
			return num;
		}
	}

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
		filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter: true, Db.Get().ChoreTypes.Fetch);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop);
		UserMaxCapacity = UserMaxCapacity;
		Subscribe(-887025858, OnRocketLandedDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
	}

	public float GetTotalOxidizerAvailable()
	{
		float num = 0f;
		Tag[] array = oxidizerTypes;
		foreach (Tag tag in array)
		{
			num += storage.GetAmountAvailable(tag);
		}
		return num;
	}

	public Dictionary<Tag, float> GetOxidizersAvailable()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		Tag[] array = oxidizerTypes;
		foreach (Tag tag in array)
		{
			dictionary[tag] = storage.GetAmountAvailable(tag);
		}
		return dictionary;
	}

	private void OnStorageChange(object data)
	{
		meter.SetPositionPercent(storage.MassStored() / storage.capacityKg);
	}

	private void OnRocketLanded(object data)
	{
		if (consumeOnLand)
		{
			storage.ConsumeAllIgnoringDisease();
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		OxidizerTank component = gameObject.GetComponent<OxidizerTank>();
		if (component != null)
		{
			UserMaxCapacity = component.UserMaxCapacity;
		}
	}

	[ContextMenu("Fill Tank")]
	public void DEBUG_FillTank(SimHashes element)
	{
		if (ElementLoader.FindElementByHash(element).IsLiquid)
		{
			storage.AddLiquid(element, targetFillMass, ElementLoader.FindElementByHash(element).defaultValues.temperature, 0, 0);
		}
		else if (ElementLoader.FindElementByHash(element).IsSolid)
		{
			GameObject go = ElementLoader.FindElementByHash(element).substance.SpawnResource(base.gameObject.transform.GetPosition(), targetFillMass, 300f, byte.MaxValue, 0);
			storage.Store(go);
		}
	}
}
