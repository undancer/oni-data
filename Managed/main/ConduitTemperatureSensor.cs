using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitTemperatureSensor : ConduitThresholdSensor, IThresholdSwitch
{
	public float rangeMin = 0f;

	public float rangeMax = 373.15f;

	[Serialize]
	private float lastValue = 0f;

	public override float CurrentValue
	{
		get
		{
			GetContentsTemperature(out var temperature, out var hasMass);
			if (hasMass)
			{
				lastValue = temperature;
			}
			return lastValue;
		}
	}

	public float RangeMin => rangeMin;

	public float RangeMax => rangeMax;

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

	private void GetContentsTemperature(out float temperature, out bool hasMass)
	{
		int cell = Grid.PosToCell(this);
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
			temperature = contents.temperature;
			hasMass = contents.mass > 0f;
			return;
		}
		SolidConduitFlow flowManager2 = SolidConduit.GetFlowManager();
		Pickupable pickupable = flowManager2.GetPickupable(flowManager2.GetContents(cell).pickupableHandle);
		if (pickupable != null && pickupable.PrimaryElement.Mass > 0f)
		{
			temperature = pickupable.PrimaryElement.Temperature;
			hasMass = true;
		}
		else
		{
			temperature = 0f;
			hasMass = false;
		}
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
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units);
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
