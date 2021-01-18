using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicWattageSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	[Serialize]
	public float thresholdWattage = 0f;

	[Serialize]
	public bool activateOnHigherThan;

	[Serialize]
	public bool dirty = true;

	private readonly float minWattage = 0f;

	private readonly float maxWattage = 1.5f * Wire.GetMaxWattageAsFloat(Wire.WattageRating.Max50000);

	private float currentWattage = 0f;

	private bool wasOn = false;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicWattageSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicWattageSensor>(delegate(LogicWattageSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float Threshold
	{
		get
		{
			return thresholdWattage;
		}
		set
		{
			thresholdWattage = value;
			dirty = true;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnHigherThan;
		}
		set
		{
			activateOnHigherThan = value;
			dirty = true;
		}
	}

	public float CurrentValue => GetWattageUsed();

	public float RangeMin => minWattage;

	public float RangeMax => maxWattage;

	public LocString Title => UI.UISIDESCREENS.WATTAGESWITCHSIDESCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
	{
		new NonLinearSlider.Range(5f, 5f),
		new NonLinearSlider.Range(35f, 1000f),
		new NonLinearSlider.Range(50f, 3000f),
		new NonLinearSlider.Range(10f, maxWattage)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicWattageSensor component = gameObject.GetComponent<LogicWattageSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateVisualState(force: true);
		UpdateLogicCircuit();
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		currentWattage = Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell(this)));
		currentWattage = Mathf.Max(0f, currentWattage);
		if (activateOnHigherThan)
		{
			if ((currentWattage > thresholdWattage && !base.IsSwitchedOn) || (currentWattage <= thresholdWattage && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((currentWattage >= thresholdWattage && base.IsSwitchedOn) || (currentWattage < thresholdWattage && !base.IsSwitchedOn))
		{
			Toggle();
		}
	}

	public float GetWattageUsed()
	{
		return currentWattage;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateVisualState();
		UpdateLogicCircuit();
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play(switchedOn ? "on_pre" : "on_pst");
			component.Queue(switchedOn ? "on" : "off");
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}

	public float GetRangeMinInputField()
	{
		return minWattage;
	}

	public float GetRangeMaxInputField()
	{
		return maxWattage;
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedWattage(value, GameUtil.WattageFormatterUnit.Watts, units);
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return UI.UNITSUFFIXES.ELECTRICAL.WATT;
	}
}
