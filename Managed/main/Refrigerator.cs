using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Refrigerator")]
public class Refrigerator : KMonoBehaviour, IUserControlledCapacity, IGameObjectEffectDescriptor
{
	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private LogicPorts ports;

	[SerializeField]
	public float simulatedInternalTemperature = 277.15f;

	[SerializeField]
	public float simulatedInternalHeatCapacity = 400f;

	[SerializeField]
	public float simulatedThermalConductivity = 1000f;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private FilteredStorage filteredStorage;

	private SimulatedTemperatureAdjuster temperatureAdjuster;

	private static readonly EventSystem.IntraObjectHandler<Refrigerator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Refrigerator>(delegate(Refrigerator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Refrigerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Refrigerator>(delegate(Refrigerator component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Refrigerator> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<Refrigerator>(delegate(Refrigerator component, object data)
	{
		component.UpdateLogicCircuitCB(data);
	});

	public float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, storage.capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			filteredStorage.FilterChanged();
			UpdateLogicCircuit();
		}
	}

	public float AmountStored => storage.MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => storage.capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	protected override void OnPrefabInit()
	{
		filteredStorage = new FilteredStorage(this, null, new Tag[1]
		{
			GameTags.Compostable
		}, this, use_logic_meter: true, Db.Get().ChoreTypes.FoodFetch);
	}

	protected override void OnSpawn()
	{
		operational.SetActive(operational.IsOperational);
		GetComponent<KAnimControllerBase>().Play("off");
		filteredStorage.FilterChanged();
		temperatureAdjuster = new SimulatedTemperatureAdjuster(simulatedInternalTemperature, simulatedInternalHeatCapacity, simulatedThermalConductivity, GetComponent<Storage>());
		UpdateLogicCircuit();
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(-1697596308, UpdateLogicCircuitCBDelegate);
		Subscribe(-592767678, UpdateLogicCircuitCBDelegate);
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
		temperatureAdjuster.CleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		bool isOperational = operational.IsOperational;
		operational.SetActive(isOperational);
	}

	public bool IsActive()
	{
		return operational.IsActive;
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!(gameObject == null))
		{
			Refrigerator component = gameObject.GetComponent<Refrigerator>();
			if (!(component == null))
			{
				UserMaxCapacity = component.UserMaxCapacity;
			}
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return SimulatedTemperatureAdjuster.GetDescriptors(simulatedInternalTemperature);
	}

	private void UpdateLogicCircuitCB(object data)
	{
		UpdateLogicCircuit();
	}

	private void UpdateLogicCircuit()
	{
		bool flag = filteredStorage.IsFull();
		bool isOperational = operational.IsOperational;
		bool flag2 = flag && isOperational;
		ports.SendSignal(FilteredStorage.FULL_PORT_ID, flag2 ? 1 : 0);
		filteredStorage.SetLogicMeter(flag2);
	}
}
