using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	private HandleVector<int>.Handle structureTemperature;

	private int simUpdateCounter = 0;

	[Serialize]
	public float thresholdTemperature = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	[Serialize]
	private bool dirty = true;

	public float minTemp = 0f;

	public float maxTemp = 373.15f;

	private const int NumFrameDelay = 8;

	private float[] temperatures = new float[8];

	private float averageTemp;

	private bool wasOn = false;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicTemperatureSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicTemperatureSensor>(delegate(LogicTemperatureSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float StructureTemperature => GameComps.StructureTemperatures.GetPayload(structureTemperature).Temperature;

	public float Threshold
	{
		get
		{
			return thresholdTemperature;
		}
		set
		{
			thresholdTemperature = value;
			dirty = true;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnWarmerThan;
		}
		set
		{
			activateOnWarmerThan = value;
			dirty = true;
		}
	}

	public float CurrentValue => GetTemperature();

	public float RangeMin => minTemp;

	public float RangeMax => maxTemp;

	public LocString Title => UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
	{
		new NonLinearSlider.Range(25f, 260f),
		new NonLinearSlider.Range(50f, 400f),
		new NonLinearSlider.Range(12f, 1500f),
		new NonLinearSlider.Range(13f, 10000f)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicTemperatureSensor component = gameObject.GetComponent<LogicTemperatureSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		base.OnToggle += OnSwitchToggled;
		UpdateVisualState(force: true);
		UpdateLogicCircuit();
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		if (simUpdateCounter < 8 && !dirty)
		{
			int i = Grid.PosToCell(this);
			if (Grid.Mass[i] > 0f)
			{
				temperatures[simUpdateCounter] = Grid.Temperature[i];
				simUpdateCounter++;
			}
			return;
		}
		simUpdateCounter = 0;
		dirty = false;
		averageTemp = 0f;
		for (int j = 0; j < 8; j++)
		{
			averageTemp += temperatures[j];
		}
		averageTemp /= 8f;
		if (activateOnWarmerThan)
		{
			if ((averageTemp > thresholdTemperature && !base.IsSwitchedOn) || (averageTemp <= thresholdTemperature && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((averageTemp >= thresholdTemperature && base.IsSwitchedOn) || (averageTemp < thresholdTemperature && !base.IsSwitchedOn))
		{
			Toggle();
		}
	}

	public float GetTemperature()
	{
		return averageTemp;
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
		return GameUtil.GetConvertedTemperature(RangeMin);
	}

	public float GetRangeMaxInputField()
	{
		return GameUtil.GetConvertedTemperature(RangeMax);
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, roundInDestinationFormat: true);
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return GameUtil.GetTemperatureConvertedToKelvin(input);
	}

	public LocString ThresholdValueUnits()
	{
		LocString result = null;
		switch (GameUtil.temperatureUnit)
		{
		case GameUtil.TemperatureUnit.Celsius:
			result = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
			break;
		case GameUtil.TemperatureUnit.Fahrenheit:
			result = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
			break;
		case GameUtil.TemperatureUnit.Kelvin:
			result = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
			break;
		}
		return result;
	}
}
