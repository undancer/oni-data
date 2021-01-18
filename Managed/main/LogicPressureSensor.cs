using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicPressureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	private bool wasOn;

	public float rangeMin;

	public float rangeMax = 1f;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private float[] samples = new float[8];

	private int sampleIdx;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicPressureSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicPressureSensor>(delegate(LogicPressureSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateAboveThreshold;
		}
		set
		{
			activateAboveThreshold = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 8; i++)
			{
				num += samples[i];
			}
			return num / 8f;
		}
	}

	public float RangeMin => rangeMin;

	public float RangeMax => rangeMax;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicPressureSensor component = ((GameObject)data).GetComponent<LogicPressureSensor>();
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
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(this);
		if (sampleIdx < 8)
		{
			float num2 = (Grid.Element[num].IsState(desiredState) ? Grid.Mass[num] : 0f);
			samples[sampleIdx] = num2;
			sampleIdx++;
			return;
		}
		sampleIdx = 0;
		float currentValue = CurrentValue;
		if (activateAboveThreshold)
		{
			if ((currentValue > threshold && !base.IsSwitchedOn) || (currentValue <= threshold && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((currentValue > threshold && base.IsSwitchedOn) || (currentValue <= threshold && !base.IsSwitchedOn))
		{
			Toggle();
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	public float GetRangeMinInputField()
	{
		if (desiredState != Element.State.Gas)
		{
			return rangeMin;
		}
		return rangeMin * 1000f;
	}

	public float GetRangeMaxInputField()
	{
		if (desiredState != Element.State.Gas)
		{
			return rangeMax;
		}
		return rangeMax * 1000f;
	}

	public string Format(float value, bool units)
	{
		GameUtil.MetricMassFormat massFormat = ((desiredState != Element.State.Gas) ? GameUtil.MetricMassFormat.Kilogram : GameUtil.MetricMassFormat.Gram);
		bool includeSuffix = units;
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, includeSuffix);
	}

	public float ProcessedSliderValue(float input)
	{
		input = ((desiredState != Element.State.Gas) ? Mathf.Round(input) : (Mathf.Round(input * 1000f) / 1000f));
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		if (desiredState == Element.State.Gas)
		{
			input /= 1000f;
		}
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return GameUtil.GetCurrentMassUnit(desiredState == Element.State.Gas);
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
}
