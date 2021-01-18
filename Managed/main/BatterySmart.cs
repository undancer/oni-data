using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
public class BatterySmart : Battery, IActivationRangeTarget
{
	public static readonly HashedString PORT_ID = "BatterySmartLogicPort";

	[Serialize]
	private int activateValue = 0;

	[Serialize]
	private int deactivateValue = 100;

	[Serialize]
	private bool activated = false;

	[MyCmpGet]
	private LogicPorts logicPorts;

	private MeterController logicMeter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<BatterySmart> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<BatterySmart>(delegate(BatterySmart component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BatterySmart> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<BatterySmart>(delegate(BatterySmart component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BatterySmart> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<BatterySmart>(delegate(BatterySmart component, object data)
	{
		component.UpdateLogicCircuit(data);
	});

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

	public string ActivateTooltip => BUILDINGS.PREFABS.BATTERYSMART.DEACTIVATE_TOOLTIP;

	public string DeactivateTooltip => BUILDINGS.PREFABS.BATTERYSMART.ACTIVATE_TOOLTIP;

	public string ActivationRangeTitleText => BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;

	public string ActivateSliderLabelText => BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_DEACTIVATE;

	public string DeactivateSliderLabelText => BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_ACTIVATE;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		BatterySmart component = gameObject.GetComponent<BatterySmart>();
		if (component != null)
		{
			ActivateValue = component.ActivateValue;
			DeactivateValue = component.DeactivateValue;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CreateLogicMeter();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, UpdateLogicCircuitDelegate);
	}

	private void CreateLogicMeter()
	{
		logicMeter = new MeterController(GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		UpdateLogicCircuit(null);
	}

	private void UpdateLogicCircuit(object data)
	{
		float num = Mathf.RoundToInt(base.PercentFull * 100f);
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
		bool isOperational = operational.IsOperational;
		bool flag = activated && isOperational;
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

	public void SetLogicMeter(bool on)
	{
		if (logicMeter != null)
		{
			logicMeter.SetPositionPercent(on ? 1f : 0f);
		}
	}
}
