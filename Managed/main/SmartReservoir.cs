using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SmartReservoir")]
public class SmartReservoir : KMonoBehaviour, IActivationRangeTarget, ISim200ms
{
	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[Serialize]
	private int activateValue;

	[Serialize]
	private int deactivateValue = 100;

	[Serialize]
	private bool activated;

	[MyCmpGet]
	private LogicPorts logicPorts;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private MeterController logicMeter;

	public static readonly HashedString PORT_ID = "SmartReservoirLogicPort";

	private static readonly EventSystem.IntraObjectHandler<SmartReservoir> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>(delegate(SmartReservoir component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SmartReservoir> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>(delegate(SmartReservoir component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SmartReservoir> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<SmartReservoir>(delegate(SmartReservoir component, object data)
	{
		component.UpdateLogicCircuit(data);
	});

	public float PercentFull => storage.MassStored() / storage.Capacity();

	public float ActivateValue
	{
		get
		{
			return deactivateValue;
		}
		set
		{
			deactivateValue = (int)value;
			UpdateLogicCircuit(null);
		}
	}

	public float DeactivateValue
	{
		get
		{
			return activateValue;
		}
		set
		{
			activateValue = (int)value;
			UpdateLogicCircuit(null);
		}
	}

	public float MinValue => 0f;

	public float MaxValue => 100f;

	public bool UseWholeNumbers => true;

	public string ActivateTooltip => BUILDINGS.PREFABS.SMARTRESERVOIR.DEACTIVATE_TOOLTIP;

	public string DeactivateTooltip => BUILDINGS.PREFABS.SMARTRESERVOIR.ACTIVATE_TOOLTIP;

	public string ActivationRangeTitleText => BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_TITLE;

	public string ActivateSliderLabelText => BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_DEACTIVATE;

	public string DeactivateSliderLabelText => BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_ACTIVATE;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, UpdateLogicCircuitDelegate);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	public void Sim200ms(float dt)
	{
		UpdateLogicCircuit(null);
	}

	private void UpdateLogicCircuit(object data)
	{
		float num = PercentFull * 100f;
		if (activated)
		{
			if (num >= (float)deactivateValue)
			{
				activated = false;
			}
		}
		else if (num <= (float)activateValue)
		{
			activated = true;
		}
		bool flag = activated;
		logicPorts.SendSignal(PORT_ID, flag ? 1 : 0);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			SetLogicMeter(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
		}
	}

	private void OnCopySettings(object data)
	{
		SmartReservoir component = ((GameObject)data).GetComponent<SmartReservoir>();
		if (component != null)
		{
			ActivateValue = component.ActivateValue;
			DeactivateValue = component.DeactivateValue;
		}
	}

	public void SetLogicMeter(bool on)
	{
		if (logicMeter != null)
		{
			logicMeter.SetPositionPercent(on ? 1f : 0f);
		}
	}
}
