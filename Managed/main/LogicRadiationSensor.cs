using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicRadiationSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	private int simUpdateCounter = 0;

	[Serialize]
	public float thresholdRads = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	[Serialize]
	private bool dirty = true;

	public float minRads = 0f;

	public float maxRads = 5000f;

	private const int NumFrameDelay = 8;

	private float[] radHistory = new float[8];

	private float averageRads;

	private bool wasOn = false;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicRadiationSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicRadiationSensor>(delegate(LogicRadiationSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float Threshold
	{
		get
		{
			return thresholdRads;
		}
		set
		{
			thresholdRads = value;
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

	public float CurrentValue => GetAverageRads();

	public float RangeMin => minRads;

	public float RangeMax => maxRads;

	public LocString Title => UI.UISIDESCREENS.RADIATIONSWITCHSIDESCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.RADIATION;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.RADIATION_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.RADIATION_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[3]
	{
		new NonLinearSlider.Range(50f, 200f),
		new NonLinearSlider.Range(25f, 1000f),
		new NonLinearSlider.Range(25f, 5000f)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicRadiationSensor component = gameObject.GetComponent<LogicRadiationSensor>();
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
		if (simUpdateCounter < 8 && !dirty)
		{
			int i = Grid.PosToCell(this);
			radHistory[simUpdateCounter] = Grid.Radiation[i];
			simUpdateCounter++;
			return;
		}
		simUpdateCounter = 0;
		dirty = false;
		averageRads = 0f;
		for (int j = 0; j < 8; j++)
		{
			averageRads += radHistory[j];
		}
		averageRads /= 8f;
		if (activateOnWarmerThan)
		{
			if ((averageRads > thresholdRads && !base.IsSwitchedOn) || (averageRads <= thresholdRads && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((averageRads >= thresholdRads && base.IsSwitchedOn) || (averageRads < thresholdRads && !base.IsSwitchedOn))
		{
			Toggle();
		}
	}

	public float GetAverageRads()
	{
		return averageRads;
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
		return GameUtil.GetFormattedRads(value);
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
		return "";
	}
}
