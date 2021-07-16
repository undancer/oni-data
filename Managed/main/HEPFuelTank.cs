using STRINGS;
using UnityEngine;

public class HEPFuelTank : KMonoBehaviour, IFuelTank, IUserControlledCapacity
{
	[MyCmpReq]
	public HighEnergyParticleStorage hepStorage;

	public float physicalFuelCapacity;

	private MeterController m_meter;

	public bool consumeFuelOnLand;

	private static readonly EventSystem.IntraObjectHandler<HEPFuelTank> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<HEPFuelTank>(delegate(HEPFuelTank component, object data)
	{
		component.OnStorageChange(data);
	});

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<HEPFuelTank> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HEPFuelTank>(delegate(HEPFuelTank component, object data)
	{
		component.OnCopySettings(data);
	});

	public IStorage Storage => hepStorage;

	public bool ConsumeFuelOnLand => consumeFuelOnLand;

	public float UserMaxCapacity
	{
		get
		{
			return hepStorage.capacity;
		}
		set
		{
			hepStorage.capacity = value;
			Trigger(-795826715, this);
		}
	}

	public float MinCapacity => 0f;

	public float MaxCapacity => physicalFuelCapacity;

	public float AmountStored => hepStorage.Particles;

	public bool WholeValues => false;

	public LocString CapacityUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

	public void DEBUG_FillTank()
	{
		hepStorage.Store(hepStorage.RemainingCapacity());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionProperlyFueled(this));
		m_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		m_meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		OnStorageChange(null);
		Subscribe(-795826715, OnStorageChangedDelegate);
		Subscribe(-1837862626, OnStorageChangedDelegate);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnStorageChange(object data)
	{
		m_meter.SetPositionPercent(hepStorage.Particles / Mathf.Max(1f, hepStorage.capacity));
	}

	private void OnCopySettings(object data)
	{
		HEPFuelTank component = ((GameObject)data).GetComponent<HEPFuelTank>();
		if (component != null)
		{
			UserMaxCapacity = component.UserMaxCapacity;
		}
	}
}
