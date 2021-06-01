using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class GameUtil
{
	public enum UnitClass
	{
		SimpleFloat,
		SimpleInteger,
		Temperature,
		Mass,
		Calories,
		Percent,
		Distance,
		Disease,
		Radiation,
		Energy,
		Power,
		Lux,
		Time,
		Seconds,
		Cycles
	}

	public enum TemperatureUnit
	{
		Celsius,
		Fahrenheit,
		Kelvin
	}

	public enum MassUnit
	{
		Kilograms,
		Pounds
	}

	public enum MetricMassFormat
	{
		UseThreshold,
		Kilogram,
		Gram,
		Tonne
	}

	public enum TemperatureInterpretation
	{
		Absolute,
		Relative
	}

	public enum TimeSlice
	{
		None,
		ModifyOnly,
		PerSecond,
		PerCycle
	}

	public enum MeasureUnit
	{
		mass,
		kcal,
		quantity
	}

	public enum WattageFormatterUnit
	{
		Watts,
		Kilowatts,
		Automatic
	}

	public enum HeatEnergyFormatterUnit
	{
		DTU_S,
		KDTU_S,
		Automatic
	}

	public struct FloodFillInfo
	{
		public int cell;

		public int depth;
	}

	public static class Hardness
	{
		public const int VERY_SOFT = 0;

		public const int SOFT = 10;

		public const int FIRM = 25;

		public const int VERY_FIRM = 50;

		public const int NEARLY_IMPENETRABLE = 150;

		public const int SUPER_DUPER_HARD = 200;

		public const int RADIOACTIVE_MATERIALS = 251;

		public const int IMPENETRABLE = 255;

		public static Color ImpenetrableColor = new Color(212f / 255f, 73f / 255f, 24f / 85f);

		public static Color nearlyImpenetrableColor = new Color(63f / 85f, 89f / 255f, 127f / 255f);

		public static Color veryFirmColor = new Color(163f / 255f, 20f / 51f, 154f / 255f);

		public static Color firmColor = new Color(134f / 255f, 107f / 255f, 0.64705884f);

		public static Color softColor = new Color(109f / 255f, 41f / 85f, 193f / 255f);

		public static Color verySoftColor = new Color(113f / 255f, 57f / 85f, 69f / 85f);
	}

	public static class GermResistanceValues
	{
		public const float MEDIUM = 2f;

		public const float LARGE = 5f;

		public static Color NegativeLargeColor = new Color(212f / 255f, 73f / 255f, 24f / 85f);

		public static Color NegativeMediumColor = new Color(63f / 85f, 89f / 255f, 127f / 255f);

		public static Color NegativeSmallColor = new Color(163f / 255f, 20f / 51f, 154f / 255f);

		public static Color PositiveSmallColor = new Color(134f / 255f, 107f / 255f, 0.64705884f);

		public static Color PositiveMediumColor = new Color(109f / 255f, 41f / 85f, 193f / 255f);

		public static Color PositiveLargeColor = new Color(113f / 255f, 57f / 85f, 69f / 85f);
	}

	public static class ThermalConductivityValues
	{
		public const float VERY_HIGH = 50f;

		public const float HIGH = 10f;

		public const float MEDIUM = 2f;

		public const float LOW = 1f;

		public static Color veryLowConductivityColor = new Color(212f / 255f, 73f / 255f, 24f / 85f);

		public static Color lowConductivityColor = new Color(63f / 85f, 89f / 255f, 127f / 255f);

		public static Color mediumConductivityColor = new Color(163f / 255f, 20f / 51f, 154f / 255f);

		public static Color highConductivityColor = new Color(134f / 255f, 107f / 255f, 0.64705884f);

		public static Color veryHighConductivityColor = new Color(109f / 255f, 41f / 85f, 193f / 255f);
	}

	public static class BreathableValues
	{
		public static Color positiveColor = new Color(113f / 255f, 57f / 85f, 69f / 85f);

		public static Color warningColor = new Color(163f / 255f, 20f / 51f, 154f / 255f);

		public static Color negativeColor = new Color(212f / 255f, 73f / 255f, 24f / 85f);
	}

	public static class WireLoadValues
	{
		public static Color warningColor = new Color(251f / 255f, 176f / 255f, 59f / 255f);

		public static Color negativeColor = new Color(1f, 49f / 255f, 49f / 255f);
	}

	public static TemperatureUnit temperatureUnit;

	public static MassUnit massUnit;

	private static string[] adjectives;

	[ThreadStatic]
	public static Queue<FloodFillInfo> FloodFillNext = new Queue<FloodFillInfo>();

	[ThreadStatic]
	public static HashSet<int> FloodFillVisited = new HashSet<int>();

	public static TagSet foodTags = new TagSet("BasicPlantFood", "MushBar", "ColdWheatSeed", "ColdWheatSeed", "SpiceNut", "PrickleFruit", "Meat", "Mushroom", "ColdWheat", GameTags.Compostable.Name);

	public static TagSet solidTags = new TagSet("Filter", "Coal", "BasicFabric", "SwampLilyFlower", "RefinedMetal");

	public static string GetTemperatureUnitSuffix()
	{
		return temperatureUnit switch
		{
			TemperatureUnit.Celsius => UI.UNITSUFFIXES.TEMPERATURE.CELSIUS, 
			TemperatureUnit.Fahrenheit => UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT, 
			_ => UI.UNITSUFFIXES.TEMPERATURE.KELVIN, 
		};
	}

	private static string AddTemperatureUnitSuffix(string text)
	{
		return text + GetTemperatureUnitSuffix();
	}

	public static float GetTemperatureConvertedFromKelvin(float temperature, TemperatureUnit targetUnit)
	{
		return targetUnit switch
		{
			TemperatureUnit.Celsius => temperature - 273.15f, 
			TemperatureUnit.Fahrenheit => temperature * 1.8f - 459.67f, 
			_ => temperature, 
		};
	}

	public static float GetConvertedTemperature(float temperature, bool roundOutput = false)
	{
		float num = 0f;
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			num = temperature - 273.15f;
			return roundOutput ? Mathf.Round(num) : num;
		case TemperatureUnit.Fahrenheit:
			num = temperature * 1.8f - 459.67f;
			return roundOutput ? Mathf.Round(num) : num;
		default:
			return roundOutput ? Mathf.Round(temperature) : temperature;
		}
	}

	public static float GetTemperatureConvertedToKelvin(float temperature, TemperatureUnit fromUnit)
	{
		return fromUnit switch
		{
			TemperatureUnit.Celsius => temperature + 273.15f, 
			TemperatureUnit.Fahrenheit => (temperature + 459.67f) * 5f / 9f, 
			_ => temperature, 
		};
	}

	public static float GetTemperatureConvertedToKelvin(float temperature)
	{
		return temperatureUnit switch
		{
			TemperatureUnit.Celsius => temperature + 273.15f, 
			TemperatureUnit.Fahrenheit => (temperature + 459.67f) * 5f / 9f, 
			_ => temperature, 
		};
	}

	private static float GetConvertedTemperatureDelta(float kelvin_delta)
	{
		return temperatureUnit switch
		{
			TemperatureUnit.Celsius => kelvin_delta, 
			TemperatureUnit.Fahrenheit => kelvin_delta * 1.8f, 
			TemperatureUnit.Kelvin => kelvin_delta, 
			_ => kelvin_delta, 
		};
	}

	public static float ApplyTimeSlice(float val, TimeSlice timeSlice)
	{
		if (timeSlice == TimeSlice.PerCycle)
		{
			return val * 600f;
		}
		return val;
	}

	public static float ApplyTimeSlice(int val, TimeSlice timeSlice)
	{
		if (timeSlice == TimeSlice.PerCycle)
		{
			return (float)val * 600f;
		}
		return val;
	}

	public static string AddTimeSliceText(string text, TimeSlice timeSlice)
	{
		return timeSlice switch
		{
			TimeSlice.PerSecond => text + UI.UNITSUFFIXES.PERSECOND, 
			TimeSlice.PerCycle => text + UI.UNITSUFFIXES.PERCYCLE, 
			_ => text, 
		};
	}

	public static string AddPositiveSign(string text, bool positive)
	{
		if (positive)
		{
			return string.Format(UI.POSITIVE_FORMAT, text);
		}
		return text;
	}

	public static float AttributeSkillToAlpha(AttributeInstance attributeInstance)
	{
		return Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);
	}

	public static float AttributeSkillToAlpha(float attributeSkill)
	{
		return Mathf.Min(attributeSkill / 10f, 1f);
	}

	public static float AptitudeToAlpha(float aptitude)
	{
		return Mathf.Min(aptitude / 10f, 1f);
	}

	public static float GetThermalEnergy(PrimaryElement pe)
	{
		return pe.Temperature * pe.Mass * pe.Element.specificHeatCapacity;
	}

	public static float CalculateTemperatureChange(float shc, float mass, float kilowatts)
	{
		return kilowatts / (shc * mass);
	}

	public static void DeltaThermalEnergy(PrimaryElement pe, float kilowatts, float targetTemperature)
	{
		float num = CalculateTemperatureChange(pe.Element.specificHeatCapacity, pe.Mass, kilowatts);
		float value = pe.Temperature + num;
		value = (pe.Temperature = ((!(targetTemperature > pe.Temperature)) ? Mathf.Clamp(value, targetTemperature, pe.Temperature) : Mathf.Clamp(value, pe.Temperature, targetTemperature)));
	}

	public static BindingEntry ActionToBinding(Action action)
	{
		BindingEntry[] keyBindings = GameInputMapping.KeyBindings;
		for (int i = 0; i < keyBindings.Length; i++)
		{
			BindingEntry result = keyBindings[i];
			if (result.mAction == action)
			{
				return result;
			}
		}
		throw new ArgumentException(action.ToString() + " is not bound in GameInputBindings");
	}

	public static string GetIdentityDescriptor(GameObject go)
	{
		if ((bool)go.GetComponent<MinionIdentity>())
		{
			return DUPLICANTS.STATS.SUBJECTS.DUPLICANT;
		}
		if ((bool)go.GetComponent<CreatureBrain>())
		{
			return DUPLICANTS.STATS.SUBJECTS.CREATURE;
		}
		return DUPLICANTS.STATS.SUBJECTS.PLANT;
	}

	public static float GetEnergyInPrimaryElement(PrimaryElement element)
	{
		return 0.001f * (element.Temperature * (element.Mass * 1000f * element.Element.specificHeatCapacity));
	}

	public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
	{
		Debug.Assert(element.Mass > 0f);
		float energyInPrimaryElement = GetEnergyInPrimaryElement(element);
		float num = Mathf.Max(energyInPrimaryElement - kilojoules, 1f);
		float temperature = element.Temperature;
		float num2 = num / (0.001f * (element.Mass * (element.Element.specificHeatCapacity * 1000f)));
		return num2 - temperature;
	}

	public static float CalculateEnergyDeltaForElement(PrimaryElement element, float startTemp, float endTemp)
	{
		return CalculateEnergyDeltaForElementChange(element.Mass, element.Element.specificHeatCapacity, startTemp, endTemp);
	}

	public static float CalculateEnergyDeltaForElementChange(float mass, float shc, float startTemp, float endTemp)
	{
		float num = endTemp - startTemp;
		return num * mass * shc;
	}

	public static float GetFinalTemperature(float t1, float m1, float t2, float m2)
	{
		float num = m1 + m2;
		float num2 = t1 * m1 + t2 * m2;
		float value = num2 / num;
		float num3 = Mathf.Min(t1, t2);
		float num4 = Mathf.Max(t1, t2);
		value = Mathf.Clamp(value, num3, num4);
		if (float.IsNaN(value) || float.IsInfinity(value))
		{
			Debug.LogError($"Calculated an invalid temperature: t1={t1}, m1={m1}, t2={t2}, m2={m2}, min_temp={num3}, max_temp={num4}");
		}
		return value;
	}

	public static void ForceConduction(PrimaryElement a, PrimaryElement b, float dt)
	{
		float num = a.Temperature * a.Element.specificHeatCapacity * a.Mass;
		float num2 = b.Temperature * b.Element.specificHeatCapacity * b.Mass;
		float num3 = Math.Min(a.Element.thermalConductivity, b.Element.thermalConductivity);
		float num4 = Math.Min(a.Mass, b.Mass);
		float val = (b.Temperature - a.Temperature) * (num3 * num4) * dt;
		float num5 = (num + num2) / (a.Element.specificHeatCapacity * a.Mass + b.Element.specificHeatCapacity * b.Mass);
		float val2 = Math.Abs((num5 - a.Temperature) * a.Element.specificHeatCapacity * a.Mass);
		float val3 = Math.Abs((num5 - b.Temperature) * b.Element.specificHeatCapacity * b.Mass);
		float num6 = Math.Min(val2, val3);
		val = Math.Min(val, num6);
		val = Math.Max(val, 0f - num6);
		a.Temperature = (num + val) / a.Element.specificHeatCapacity / a.Mass;
		b.Temperature = (num2 - val) / b.Element.specificHeatCapacity / b.Mass;
	}

	public static string FloatToString(float f, string format = null)
	{
		if (float.IsPositiveInfinity(f))
		{
			return UI.POS_INFINITY;
		}
		if (float.IsNegativeInfinity(f))
		{
			return UI.NEG_INFINITY;
		}
		return f.ToString(format);
	}

	public static string GetStandardFloat(float f, bool allowHundredths = false)
	{
		string text = "";
		text = ((Mathf.Abs(f) == 0f) ? "0" : ((Mathf.Abs(f) < 0.1f && allowHundredths) ? "##0.##" : ((!(Mathf.Abs(f) < 1f)) ? "##0" : "##0.#")));
		return FloatToString(f, text);
	}

	public static string GetUnitFormattedName(GameObject go, bool upperName = false)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null && Assets.IsTagCountable(component.PrefabTag))
		{
			PrimaryElement component2 = go.GetComponent<PrimaryElement>();
			return GetUnitFormattedName(go.GetProperName(), component2.Units, upperName);
		}
		return upperName ? StringFormatter.ToUpper(go.GetProperName()) : go.GetProperName();
	}

	public static string GetUnitFormattedName(string name, float count, bool upperName = false)
	{
		if (upperName)
		{
			name = name.ToUpper();
		}
		return StringFormatter.Replace(UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", $"{count:0.##}");
	}

	public static string GetFormattedUnits(float units, TimeSlice timeSlice = TimeSlice.None, bool displaySuffix = true, string floatFormatOverride = "")
	{
		string str = ((units == 1f) ? UI.UNITSUFFIXES.UNIT : UI.UNITSUFFIXES.UNITS);
		units = ApplyTimeSlice(units, timeSlice);
		string text = GetStandardFloat(units);
		if (!floatFormatOverride.IsNullOrWhiteSpace())
		{
			text = string.Format(floatFormatOverride, units);
		}
		if (displaySuffix)
		{
			text += str;
		}
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedRocketRange(float range, TimeSlice timeSlice, bool displaySuffix = true)
	{
		if (timeSlice == TimeSlice.PerCycle)
		{
			return range.ToString("N1") + (displaySuffix ? (" " + UI.CLUSTERMAP.TILES_PER_CYCLE) : "");
		}
		return Mathf.Floor(range / 600f) + (displaySuffix ? (" " + UI.CLUSTERMAP.TILES) : "");
	}

	public static string ApplyBoldString(string source)
	{
		return "<b>" + source + "</b>";
	}

	public static float GetRoundedTemperatureInKelvin(float kelvin)
	{
		float result = 0f;
		switch (temperatureUnit)
		{
		case TemperatureUnit.Celsius:
			result = GetTemperatureConvertedToKelvin(Mathf.Round(GetConvertedTemperature(Mathf.Round(kelvin), roundOutput: true)));
			break;
		case TemperatureUnit.Fahrenheit:
		{
			float temperature = Mathf.RoundToInt(GetTemperatureConvertedFromKelvin(kelvin, TemperatureUnit.Fahrenheit));
			result = GetTemperatureConvertedToKelvin(temperature, TemperatureUnit.Fahrenheit);
			break;
		}
		case TemperatureUnit.Kelvin:
			result = Mathf.RoundToInt(kelvin);
			break;
		}
		return result;
	}

	public static string GetFormattedTemperature(float temp, TimeSlice timeSlice = TimeSlice.None, TemperatureInterpretation interpretation = TemperatureInterpretation.Absolute, bool displayUnits = true, bool roundInDestinationFormat = false)
	{
		temp = interpretation switch
		{
			TemperatureInterpretation.Absolute => GetConvertedTemperature(temp, roundInDestinationFormat), 
			_ => GetConvertedTemperatureDelta(temp), 
		};
		temp = ApplyTimeSlice(temp, timeSlice);
		string text = "";
		text = ((!(Mathf.Abs(temp) < 0.1f)) ? FloatToString(temp, "##0.#") : FloatToString(temp, "##0.####"));
		if (displayUnits)
		{
			text = AddTemperatureUnitSuffix(text);
		}
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedCaloriesForItem(Tag tag, float amount, TimeSlice timeSlice = TimeSlice.None, bool forceKcal = true)
	{
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
		return GetFormattedCalories((foodInfo != null) ? (foodInfo.CaloriesPerUnit * amount) : (-1f), timeSlice, forceKcal);
	}

	public static string GetFormattedCalories(float calories, TimeSlice timeSlice = TimeSlice.None, bool forceKcal = true)
	{
		string str = UI.UNITSUFFIXES.CALORIES.CALORIE;
		if (Mathf.Abs(calories) >= 1000f || forceKcal)
		{
			calories /= 1000f;
			str = UI.UNITSUFFIXES.CALORIES.KILOCALORIE;
		}
		calories = ApplyTimeSlice(calories, timeSlice);
		string text = GetStandardFloat(calories) + str;
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedPlantGrowth(float percent, TimeSlice timeSlice = TimeSlice.None)
	{
		percent = ApplyTimeSlice(percent, timeSlice);
		string text = string.Concat(GetStandardFloat(percent, allowHundredths: true), UI.UNITSUFFIXES.PERCENT, " ", UI.UNITSUFFIXES.GROWTH);
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedPercent(float percent, TimeSlice timeSlice = TimeSlice.None)
	{
		percent = ApplyTimeSlice(percent, timeSlice);
		string text = GetStandardFloat(percent, allowHundredths: true) + UI.UNITSUFFIXES.PERCENT;
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedRoundedJoules(float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return FloatToString(joules / 1000f, "F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return FloatToString(joules, "F1") + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedJoules(float joules, string floatFormat = "F1", TimeSlice timeSlice = TimeSlice.None)
	{
		if (timeSlice == TimeSlice.PerSecond)
		{
			return GetFormattedWattage(joules);
		}
		joules = ApplyTimeSlice(joules, timeSlice);
		string text = ((Math.Abs(joules) > 1000000f) ? (FloatToString(joules / 1000000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.MEGAJOULE) : ((!(Mathf.Abs(joules) > 1000f)) ? (FloatToString(joules, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.JOULE) : (FloatToString(joules / 1000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE)));
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedRads(float rads, TimeSlice timeSlice = TimeSlice.None)
	{
		rads = ApplyTimeSlice(rads, timeSlice);
		string text = GetStandardFloat(rads) + UI.UNITSUFFIXES.RADIATION.RADS;
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedHighEnergyParticles(float units, TimeSlice timeSlice = TimeSlice.None)
	{
		string str = ((units == 1f) ? UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLE : UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
		units = ApplyTimeSlice(units, timeSlice);
		string text = GetStandardFloat(units) + str;
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedWattage(float watts, WattageFormatterUnit unit = WattageFormatterUnit.Automatic, bool displayUnits = true)
	{
		LocString loc_string = "";
		switch (unit)
		{
		case WattageFormatterUnit.Automatic:
			if (Mathf.Abs(watts) > 1000f)
			{
				watts /= 1000f;
				loc_string = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			}
			else
			{
				loc_string = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			}
			break;
		case WattageFormatterUnit.Kilowatts:
			watts /= 1000f;
			loc_string = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			break;
		case WattageFormatterUnit.Watts:
			loc_string = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			break;
		}
		if (displayUnits)
		{
			return FloatToString(watts, "###0.##") + loc_string;
		}
		return FloatToString(watts, "###0.##");
	}

	public static string GetFormattedHeatEnergy(float dtu, HeatEnergyFormatterUnit unit = HeatEnergyFormatterUnit.Automatic)
	{
		LocString locString = "";
		string format;
		switch (unit)
		{
		default:
			if (Mathf.Abs(dtu) > 1000f)
			{
				dtu /= 1000f;
				locString = UI.UNITSUFFIXES.HEAT.KDTU;
				format = "###0.##";
			}
			else
			{
				locString = UI.UNITSUFFIXES.HEAT.DTU;
				format = "###0.";
			}
			break;
		case HeatEnergyFormatterUnit.KDTU_S:
			dtu /= 1000f;
			locString = UI.UNITSUFFIXES.HEAT.KDTU;
			format = "###0.##";
			break;
		case HeatEnergyFormatterUnit.DTU_S:
			locString = UI.UNITSUFFIXES.HEAT.DTU;
			format = "###0.";
			break;
		}
		return FloatToString(dtu, format) + locString;
	}

	public static string GetFormattedHeatEnergyRate(float dtu_s, HeatEnergyFormatterUnit unit = HeatEnergyFormatterUnit.Automatic)
	{
		LocString loc_string = "";
		switch (unit)
		{
		case HeatEnergyFormatterUnit.Automatic:
			if (Mathf.Abs(dtu_s) > 1000f)
			{
				dtu_s /= 1000f;
				loc_string = UI.UNITSUFFIXES.HEAT.KDTU_S;
			}
			else
			{
				loc_string = UI.UNITSUFFIXES.HEAT.DTU_S;
			}
			break;
		case HeatEnergyFormatterUnit.KDTU_S:
			dtu_s /= 1000f;
			loc_string = UI.UNITSUFFIXES.HEAT.KDTU_S;
			break;
		case HeatEnergyFormatterUnit.DTU_S:
			loc_string = UI.UNITSUFFIXES.HEAT.DTU_S;
			break;
		}
		return FloatToString(dtu_s, "###0.##") + loc_string;
	}

	public static string GetFormattedInt(float num, TimeSlice timeSlice = TimeSlice.None)
	{
		num = ApplyTimeSlice(num, timeSlice);
		return AddTimeSliceText(FloatToString(num, "F0"), timeSlice);
	}

	public static string GetFormattedSimple(float num, TimeSlice timeSlice = TimeSlice.None, string formatString = null)
	{
		num = ApplyTimeSlice(num, timeSlice);
		string text = "";
		text = ((formatString != null) ? FloatToString(num, formatString) : ((num == 0f) ? "0" : ((Mathf.Abs(num) < 1f) ? FloatToString(num, "#,##0.##") : ((!(Mathf.Abs(num) < 10f)) ? FloatToString(num, "#,###.##") : FloatToString(num, "#,###.##")))));
		return AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedLux(int lux)
	{
		return lux.ToString() + UI.UNITSUFFIXES.LIGHT.LUX;
	}

	public static string GetLightDescription(int lux)
	{
		if (lux == 0)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.NO_LIGHT;
		}
		if (lux < 100)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_LOW_LIGHT;
		}
		if (lux < 1000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.LOW_LIGHT;
		}
		if (lux < 10000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.MEDIUM_LIGHT;
		}
		if (lux < 50000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.HIGH_LIGHT;
		}
		if (lux < 100000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_HIGH_LIGHT;
		}
		return UI.OVERLAYS.LIGHTING.RANGES.MAX_LIGHT;
	}

	public static string GetRadiationDescription(float radsPerCycle)
	{
		if (radsPerCycle == 0f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.NONE;
		}
		if (radsPerCycle < 50f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.VERY_LOW;
		}
		if (radsPerCycle < 100f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.LOW;
		}
		if (radsPerCycle < 200f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.MEDIUM;
		}
		if (radsPerCycle < 1000f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.HIGH;
		}
		if (radsPerCycle < 2000f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.VERY_HIGH;
		}
		return UI.OVERLAYS.RADIATION.RANGES.MAX;
	}

	public static string GetFormattedByTag(Tag tag, float amount, TimeSlice timeSlice = TimeSlice.None)
	{
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			return GetFormattedCaloriesForItem(tag, amount, timeSlice);
		}
		if (GameTags.DisplayAsUnits.Contains(tag))
		{
			return GetFormattedUnits(amount, timeSlice);
		}
		return GetFormattedMass(amount, timeSlice);
	}

	public static string GetFormattedFoodQuality(int quality)
	{
		if (adjectives == null)
		{
			adjectives = LocString.GetStrings(typeof(DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVES));
		}
		LocString loc_string = ((quality >= 0) ? DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_POSITIVE : DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_NEGATIVE);
		int value = quality - DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_INDEX_OFFSET;
		value = Mathf.Clamp(value, 0, adjectives.Length);
		return string.Format(loc_string, adjectives[value], AddPositiveSign(quality.ToString(), quality > 0));
	}

	public static string GetFormattedBytes(ulong amount)
	{
		string[] array = new string[5]
		{
			UI.UNITSUFFIXES.INFORMATION.BYTE,
			UI.UNITSUFFIXES.INFORMATION.KILOBYTE,
			UI.UNITSUFFIXES.INFORMATION.MEGABYTE,
			UI.UNITSUFFIXES.INFORMATION.GIGABYTE,
			UI.UNITSUFFIXES.INFORMATION.TERABYTE
		};
		int num = ((amount != 0L) ? ((int)Math.Floor(Math.Floor(Math.Log(amount)) / Math.Log(1024.0))) : 0);
		double num2 = (double)amount / Math.Pow(1024.0, num);
		Debug.Assert(num >= 0 && num < array.Length);
		return $"{num2:F} {array[num]}";
	}

	public static string GetFormattedInfomation(float amount, TimeSlice timeSlice = TimeSlice.None)
	{
		amount = ApplyTimeSlice(amount, timeSlice);
		string arg = "";
		if (amount < 1024f)
		{
			arg = UI.UNITSUFFIXES.INFORMATION.KILOBYTE;
		}
		else if (amount < 1048576f)
		{
			amount /= 1000f;
			arg = UI.UNITSUFFIXES.INFORMATION.MEGABYTE;
		}
		else if (amount < 1.0737418E+09f)
		{
			amount /= 1048576f;
			arg = UI.UNITSUFFIXES.INFORMATION.GIGABYTE;
		}
		return AddTimeSliceText(amount + arg, timeSlice);
	}

	public static LocString GetCurrentMassUnit(bool useSmallUnit = false)
	{
		LocString result = null;
		switch (massUnit)
		{
		case MassUnit.Kilograms:
			result = ((!useSmallUnit) ? UI.UNITSUFFIXES.MASS.KILOGRAM : UI.UNITSUFFIXES.MASS.GRAM);
			break;
		case MassUnit.Pounds:
			result = UI.UNITSUFFIXES.MASS.POUND;
			break;
		}
		return result;
	}

	public static string GetFormattedMass(float mass, TimeSlice timeSlice = TimeSlice.None, MetricMassFormat massFormat = MetricMassFormat.UseThreshold, bool includeSuffix = true, string floatFormat = "{0:0.#}")
	{
		if (mass == float.MinValue)
		{
			return UI.CALCULATING;
		}
		mass = ApplyTimeSlice(mass, timeSlice);
		string str;
		if (massUnit == MassUnit.Kilograms)
		{
			str = UI.UNITSUFFIXES.MASS.TONNE;
			switch (massFormat)
			{
			case MetricMassFormat.UseThreshold:
			{
				float num = Mathf.Abs(mass);
				if (0f < num)
				{
					if (num < 5E-06f)
					{
						str = UI.UNITSUFFIXES.MASS.MICROGRAM;
						mass = Mathf.Floor(mass * 1E+09f);
					}
					else if (num < 0.005f)
					{
						mass *= 1000000f;
						str = UI.UNITSUFFIXES.MASS.MILLIGRAM;
					}
					else if (Mathf.Abs(mass) < 5f)
					{
						mass *= 1000f;
						str = UI.UNITSUFFIXES.MASS.GRAM;
					}
					else if (Mathf.Abs(mass) < 5000f)
					{
						str = UI.UNITSUFFIXES.MASS.KILOGRAM;
					}
					else
					{
						mass /= 1000f;
						str = UI.UNITSUFFIXES.MASS.TONNE;
					}
				}
				else
				{
					str = UI.UNITSUFFIXES.MASS.KILOGRAM;
				}
				break;
			}
			case MetricMassFormat.Kilogram:
				str = UI.UNITSUFFIXES.MASS.KILOGRAM;
				break;
			case MetricMassFormat.Gram:
				mass *= 1000f;
				str = UI.UNITSUFFIXES.MASS.GRAM;
				break;
			case MetricMassFormat.Tonne:
				mass /= 1000f;
				str = UI.UNITSUFFIXES.MASS.TONNE;
				break;
			}
		}
		else
		{
			mass /= 2.2f;
			str = UI.UNITSUFFIXES.MASS.POUND;
			if (massFormat == MetricMassFormat.UseThreshold)
			{
				float num2 = Mathf.Abs(mass);
				if (num2 < 5f && num2 > 0.001f)
				{
					mass *= 256f;
					str = UI.UNITSUFFIXES.MASS.DRACHMA;
				}
				else
				{
					mass *= 7000f;
					str = UI.UNITSUFFIXES.MASS.GRAIN;
				}
			}
		}
		if (!includeSuffix)
		{
			str = "";
			timeSlice = TimeSlice.None;
		}
		return AddTimeSliceText(string.Format(floatFormat, mass) + str, timeSlice);
	}

	public static string GetFormattedTime(float seconds, string floatFormat = "F0")
	{
		return string.Format(UI.FORMATSECONDS, seconds.ToString(floatFormat));
	}

	public static string GetFormattedEngineEfficiency(float amount)
	{
		return amount + " km /" + (string)UI.UNITSUFFIXES.MASS.KILOGRAM;
	}

	public static string GetFormattedDistance(float meters)
	{
		if (Mathf.Abs(meters) < 1f)
		{
			string text = (meters * 100f).ToString();
			string text2 = text.Substring(0, text.LastIndexOf('.') + Mathf.Min(3, text.Length - text.LastIndexOf('.')));
			if (text2 == "-0.0")
			{
				text2 = "0";
			}
			return text2 + " cm";
		}
		if (meters < 1000f)
		{
			return meters + " m";
		}
		return Util.FormatOneDecimalPlace(meters / 1000f) + " km";
	}

	public static string GetFormattedCycles(float seconds, string formatString = "F1", bool forceCycles = false)
	{
		if (forceCycles || Mathf.Abs(seconds) > 100f)
		{
			return string.Format(UI.FORMATDAY, FloatToString(seconds / 600f, formatString));
		}
		return GetFormattedTime(seconds);
	}

	public static float GetDisplaySHC(float shc)
	{
		if (temperatureUnit == TemperatureUnit.Fahrenheit)
		{
			shc /= 1.8f;
		}
		return shc;
	}

	public static string GetSHCSuffix()
	{
		return $"(DTU/g)/{GetTemperatureUnitSuffix()}";
	}

	public static string GetFormattedSHC(float shc)
	{
		shc = GetDisplaySHC(shc);
		return string.Format("{0} (DTU/g)/{1}", shc.ToString("0.000"), GetTemperatureUnitSuffix());
	}

	public static float GetDisplayThermalConductivity(float tc)
	{
		if (temperatureUnit == TemperatureUnit.Fahrenheit)
		{
			tc /= 1.8f;
		}
		return tc;
	}

	public static string GetThermalConductivitySuffix()
	{
		return $"(DTU/(m*s))/{GetTemperatureUnitSuffix()}";
	}

	public static string GetFormattedThermalConductivity(float tc)
	{
		tc = GetDisplayThermalConductivity(tc);
		return string.Format("{0} (DTU/(m*s))/{1}", tc.ToString("0.000"), GetTemperatureUnitSuffix());
	}

	public static string GetElementNameByElementHash(SimHashes elementHash)
	{
		return ElementLoader.FindElementByHash(elementHash).tag.ProperName();
	}

	public static bool HasTrait(GameObject go, string traitName)
	{
		Traits component = go.GetComponent<Traits>();
		return !(component == null) && component.HasTrait(traitName);
	}

	public static HashSet<int> GetFloodFillCavity(int startCell, bool allowLiquid)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (allowLiquid)
		{
			return FloodCollectCells(startCell, (int cell) => !Grid.Solid[cell]);
		}
		return FloodCollectCells(startCell, (int cell) => Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas);
	}

	public static HashSet<int> FloodCollectCells(int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		probeFromCell(start_cell, is_valid, hashSet, hashSet2, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet2);
			if (hashSet.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(hashSet);
			}
		}
		if (hashSet.Count > maxSize && clearOversizedResults)
		{
			hashSet.Clear();
		}
		return hashSet;
	}

	public static HashSet<int> FloodCollectCells(HashSet<int> results, int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		probeFromCell(start_cell, is_valid, results, hashSet, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet);
			if (results.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(results);
			}
		}
		if (results.Count > maxSize && clearOversizedResults)
		{
			results.Clear();
		}
		return results;
	}

	private static void probeFromCell(int start_cell, Func<int, bool> is_valid, HashSet<int> cells, HashSet<int> invalidCells, int maxSize = 300)
	{
		if (cells.Count > maxSize || !Grid.IsValidCell(start_cell) || invalidCells.Contains(start_cell) || cells.Contains(start_cell) || !is_valid(start_cell))
		{
			invalidCells.Add(start_cell);
			return;
		}
		cells.Add(start_cell);
		probeFromCell(Grid.CellLeft(start_cell), is_valid, cells, invalidCells, maxSize);
		probeFromCell(Grid.CellRight(start_cell), is_valid, cells, invalidCells, maxSize);
		probeFromCell(Grid.CellAbove(start_cell), is_valid, cells, invalidCells, maxSize);
		probeFromCell(Grid.CellBelow(start_cell), is_valid, cells, invalidCells, maxSize);
	}

	public static bool FloodFillCheck<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		int num = FloodFillFind(fn, arg, start_cell, max_depth, stop_at_solid, stop_at_liquid);
		return num != -1;
	}

	public static int FloodFillFind<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		Queue<FloodFillInfo> floodFillNext = FloodFillNext;
		FloodFillInfo item = new FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		};
		floodFillNext.Enqueue(item);
		int result = -1;
		while (FloodFillNext.Count > 0)
		{
			FloodFillInfo floodFillInfo = FloodFillNext.Dequeue();
			if (floodFillInfo.depth >= max_depth || !Grid.IsValidCell(floodFillInfo.cell))
			{
				continue;
			}
			Element element = Grid.Element[floodFillInfo.cell];
			if ((!stop_at_solid || !element.IsSolid) && (!stop_at_liquid || !element.IsLiquid) && !FloodFillVisited.Contains(floodFillInfo.cell))
			{
				FloodFillVisited.Add(floodFillInfo.cell);
				if (fn(floodFillInfo.cell, arg))
				{
					result = floodFillInfo.cell;
					break;
				}
				Queue<FloodFillInfo> floodFillNext2 = FloodFillNext;
				item = new FloodFillInfo
				{
					cell = Grid.CellLeft(floodFillInfo.cell),
					depth = floodFillInfo.depth + 1
				};
				floodFillNext2.Enqueue(item);
				Queue<FloodFillInfo> floodFillNext3 = FloodFillNext;
				item = new FloodFillInfo
				{
					cell = Grid.CellRight(floodFillInfo.cell),
					depth = floodFillInfo.depth + 1
				};
				floodFillNext3.Enqueue(item);
				Queue<FloodFillInfo> floodFillNext4 = FloodFillNext;
				item = new FloodFillInfo
				{
					cell = Grid.CellAbove(floodFillInfo.cell),
					depth = floodFillInfo.depth + 1
				};
				floodFillNext4.Enqueue(item);
				Queue<FloodFillInfo> floodFillNext5 = FloodFillNext;
				item = new FloodFillInfo
				{
					cell = Grid.CellBelow(floodFillInfo.cell),
					depth = floodFillInfo.depth + 1
				};
				floodFillNext5.Enqueue(item);
			}
		}
		FloodFillVisited.Clear();
		FloodFillNext.Clear();
		return result;
	}

	public static void FloodFillConditional(int start_cell, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null)
	{
		FloodFillNext.Enqueue(new FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		});
		FloodFillConditional(FloodFillNext, condition, visited_cells, valid_cells);
	}

	public static void FloodFillConditional(Queue<FloodFillInfo> queue, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null, int max_depth = 10000)
	{
		while (queue.Count > 0)
		{
			FloodFillInfo floodFillInfo = queue.Dequeue();
			if (floodFillInfo.depth < max_depth && Grid.IsValidCell(floodFillInfo.cell) && !visited_cells.Contains(floodFillInfo.cell))
			{
				visited_cells.Add(floodFillInfo.cell);
				if (condition(floodFillInfo.cell))
				{
					valid_cells?.Add(floodFillInfo.cell);
					FloodFillInfo item = new FloodFillInfo
					{
						cell = Grid.CellLeft(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					};
					queue.Enqueue(item);
					item = new FloodFillInfo
					{
						cell = Grid.CellRight(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					};
					queue.Enqueue(item);
					item = new FloodFillInfo
					{
						cell = Grid.CellAbove(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					};
					queue.Enqueue(item);
					item = new FloodFillInfo
					{
						cell = Grid.CellBelow(floodFillInfo.cell),
						depth = floodFillInfo.depth + 1
					};
					queue.Enqueue(item);
				}
			}
		}
		queue.Clear();
	}

	public static string GetHardnessString(Element element, bool addColor = true)
	{
		if (!element.IsSolid)
		{
			return ELEMENTS.HARDNESS.NA;
		}
		Color firmColor = Hardness.firmColor;
		string text = "";
		if (element.hardness >= byte.MaxValue)
		{
			firmColor = Hardness.ImpenetrableColor;
			text = string.Format(ELEMENTS.HARDNESS.IMPENETRABLE, element.hardness);
		}
		else if (element.hardness >= 150)
		{
			firmColor = Hardness.nearlyImpenetrableColor;
			text = string.Format(ELEMENTS.HARDNESS.NEARLYIMPENETRABLE, element.hardness);
		}
		else if (element.hardness >= 50)
		{
			firmColor = Hardness.veryFirmColor;
			text = string.Format(ELEMENTS.HARDNESS.VERYFIRM, element.hardness);
		}
		else if (element.hardness >= 25)
		{
			firmColor = Hardness.firmColor;
			text = string.Format(ELEMENTS.HARDNESS.FIRM, element.hardness);
		}
		else if (element.hardness >= 10)
		{
			firmColor = Hardness.softColor;
			text = string.Format(ELEMENTS.HARDNESS.SOFT, element.hardness);
		}
		else
		{
			firmColor = Hardness.verySoftColor;
			text = string.Format(ELEMENTS.HARDNESS.VERYSOFT, element.hardness);
		}
		if (addColor)
		{
			text = $"<color=#{firmColor.ToHexString()}>{text}</color>";
		}
		return text;
	}

	public static string GetGermResistanceModifierString(float modifier, bool addColor = true)
	{
		Color c = Color.black;
		string text = "";
		if (modifier > 0f)
		{
			if (modifier >= 5f)
			{
				c = GermResistanceValues.PositiveLargeColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_LARGE, modifier);
			}
			else if (modifier >= 2f)
			{
				c = GermResistanceValues.PositiveMediumColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_MEDIUM, modifier);
			}
			else if (modifier > 0f)
			{
				c = GermResistanceValues.PositiveSmallColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_SMALL, modifier);
			}
		}
		else if (modifier < 0f)
		{
			if (modifier <= -5f)
			{
				c = GermResistanceValues.NegativeLargeColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_LARGE, modifier);
			}
			else if (modifier <= -2f)
			{
				c = GermResistanceValues.NegativeMediumColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_MEDIUM, modifier);
			}
			else if (modifier < 0f)
			{
				c = GermResistanceValues.NegativeSmallColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_SMALL, modifier);
			}
		}
		else
		{
			addColor = false;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NONE, modifier);
		}
		if (addColor)
		{
			text = $"<color=#{c.ToHexString()}>{text}</color>";
		}
		return text;
	}

	public static string GetThermalConductivityString(Element element, bool addColor = true, bool addValue = true)
	{
		Color mediumConductivityColor = ThermalConductivityValues.mediumConductivityColor;
		string text = "";
		if (element.thermalConductivity >= 50f)
		{
			mediumConductivityColor = ThermalConductivityValues.veryHighConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 10f)
		{
			mediumConductivityColor = ThermalConductivityValues.highConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 2f)
		{
			mediumConductivityColor = ThermalConductivityValues.mediumConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.MEDIUM_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 1f)
		{
			mediumConductivityColor = ThermalConductivityValues.lowConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.LOW_CONDUCTIVITY;
		}
		else
		{
			mediumConductivityColor = ThermalConductivityValues.veryLowConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_LOW_CONDUCTIVITY;
		}
		if (addColor)
		{
			text = $"<color=#{mediumConductivityColor.ToHexString()}>{text}</color>";
		}
		if (addValue)
		{
			text = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VALUE_WITH_ADJECTIVE, element.thermalConductivity.ToString(), text);
		}
		return text;
	}

	public static string GetBreathableString(Element element, float Mass)
	{
		if (!element.IsGas && !element.IsVacuum)
		{
			return "";
		}
		Color positiveColor = BreathableValues.positiveColor;
		LocString arg;
		switch (element.id)
		{
		case SimHashes.Oxygen:
			if (Mass >= SimDebugView.optimallyBreathable)
			{
				positiveColor = BreathableValues.positiveColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND1;
			}
			else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
			{
				positiveColor = BreathableValues.positiveColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass >= SimDebugView.minimumBreathable)
			{
				positiveColor = BreathableValues.warningColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND3;
			}
			else
			{
				positiveColor = BreathableValues.negativeColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			break;
		case SimHashes.ContaminatedOxygen:
			if (Mass >= SimDebugView.optimallyBreathable)
			{
				positiveColor = BreathableValues.positiveColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND1;
			}
			else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
			{
				positiveColor = BreathableValues.positiveColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass >= SimDebugView.minimumBreathable)
			{
				positiveColor = BreathableValues.warningColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND3;
			}
			else
			{
				positiveColor = BreathableValues.negativeColor;
				arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			break;
		default:
			positiveColor = BreathableValues.negativeColor;
			arg = UI.OVERLAYS.OXYGEN.LEGEND4;
			break;
		}
		return string.Format(ELEMENTS.BREATHABLEDESC, positiveColor.ToHexString(), arg);
	}

	public static string GetWireLoadColor(float load, float maxLoad, float potentialLoad)
	{
		Color c = ((load > maxLoad + POWER.FLOAT_FUDGE_FACTOR) ? WireLoadValues.negativeColor : ((!(potentialLoad > maxLoad) || !(load / maxLoad >= 0.75f)) ? Color.white : WireLoadValues.warningColor));
		return c.ToHexString();
	}

	public static string AppendHotkeyString(string template, Action action)
	{
		return template + UI.FormatAsHotkey("[" + GetActionString(action) + "]");
	}

	public static string ReplaceHotkeyString(string template, Action action)
	{
		return template.Replace("{Hotkey}", UI.FormatAsHotkey("[" + GetActionString(action) + "]"));
	}

	public static string ReplaceHotkeyString(string template, Action action1, Action action2)
	{
		return template.Replace("{Hotkey}", UI.FormatAsHotkey("[" + GetActionString(action1) + "]") + UI.FormatAsHotkey("[" + GetActionString(action2) + "]"));
	}

	public static string GetKeycodeLocalized(KKeyCode key_code)
	{
		string result = key_code.ToString();
		switch (key_code)
		{
		case KKeyCode.Return:
			result = INPUT.ENTER;
			break;
		case KKeyCode.Escape:
			result = INPUT.ESCAPE;
			break;
		case KKeyCode.Backslash:
			result = "\\";
			break;
		case KKeyCode.Backspace:
			result = INPUT.BACKSPACE;
			break;
		case KKeyCode.Plus:
			result = "+";
			break;
		case KKeyCode.Slash:
			result = "/";
			break;
		case KKeyCode.Space:
			result = INPUT.SPACE;
			break;
		case KKeyCode.Tab:
			result = INPUT.TAB;
			break;
		case KKeyCode.LeftBracket:
			result = "[";
			break;
		case KKeyCode.RightBracket:
			result = "]";
			break;
		case KKeyCode.Semicolon:
			result = ";";
			break;
		case KKeyCode.Colon:
			result = ":";
			break;
		case KKeyCode.Period:
			result = INPUT.PERIOD;
			break;
		case KKeyCode.Comma:
			result = ",";
			break;
		case KKeyCode.BackQuote:
			result = INPUT.BACKQUOTE;
			break;
		case KKeyCode.MouseScrollUp:
			result = INPUT.MOUSE_SCROLL_UP;
			break;
		case KKeyCode.MouseScrollDown:
			result = INPUT.MOUSE_SCROLL_DOWN;
			break;
		case KKeyCode.Minus:
			result = "-";
			break;
		case KKeyCode.Equals:
			result = "=";
			break;
		case KKeyCode.LeftShift:
			result = INPUT.LEFT_SHIFT;
			break;
		case KKeyCode.RightShift:
			result = INPUT.RIGHT_SHIFT;
			break;
		case KKeyCode.LeftAlt:
			result = INPUT.LEFT_ALT;
			break;
		case KKeyCode.RightAlt:
			result = INPUT.RIGHT_ALT;
			break;
		case KKeyCode.LeftControl:
			result = INPUT.LEFT_CTRL;
			break;
		case KKeyCode.RightControl:
			result = INPUT.RIGHT_CTRL;
			break;
		case KKeyCode.Insert:
			result = INPUT.INSERT;
			break;
		case KKeyCode.Mouse0:
			result = string.Concat(INPUT.MOUSE, " 0");
			break;
		case KKeyCode.Mouse1:
			result = string.Concat(INPUT.MOUSE, " 1");
			break;
		case KKeyCode.Mouse2:
			result = string.Concat(INPUT.MOUSE, " 2");
			break;
		case KKeyCode.Mouse3:
			result = string.Concat(INPUT.MOUSE, " 3");
			break;
		case KKeyCode.Mouse4:
			result = string.Concat(INPUT.MOUSE, " 4");
			break;
		case KKeyCode.Mouse5:
			result = string.Concat(INPUT.MOUSE, " 5");
			break;
		case KKeyCode.Mouse6:
			result = string.Concat(INPUT.MOUSE, " 6");
			break;
		case KKeyCode.Keypad0:
			result = string.Concat(INPUT.NUM, " 0");
			break;
		case KKeyCode.Keypad1:
			result = string.Concat(INPUT.NUM, " 1");
			break;
		case KKeyCode.Keypad2:
			result = string.Concat(INPUT.NUM, " 2");
			break;
		case KKeyCode.Keypad3:
			result = string.Concat(INPUT.NUM, " 3");
			break;
		case KKeyCode.Keypad4:
			result = string.Concat(INPUT.NUM, " 4");
			break;
		case KKeyCode.Keypad5:
			result = string.Concat(INPUT.NUM, " 5");
			break;
		case KKeyCode.Keypad6:
			result = string.Concat(INPUT.NUM, " 6");
			break;
		case KKeyCode.Keypad7:
			result = string.Concat(INPUT.NUM, " 7");
			break;
		case KKeyCode.Keypad8:
			result = string.Concat(INPUT.NUM, " 8");
			break;
		case KKeyCode.Keypad9:
			result = string.Concat(INPUT.NUM, " 9");
			break;
		case KKeyCode.KeypadMultiply:
			result = string.Concat(INPUT.NUM, " *");
			break;
		case KKeyCode.KeypadPeriod:
			result = string.Concat(INPUT.NUM, " ", INPUT.PERIOD);
			break;
		case KKeyCode.KeypadPlus:
			result = string.Concat(INPUT.NUM, " +");
			break;
		case KKeyCode.KeypadMinus:
			result = string.Concat(INPUT.NUM, " -");
			break;
		case KKeyCode.KeypadDivide:
			result = string.Concat(INPUT.NUM, " /");
			break;
		case KKeyCode.KeypadEnter:
			result = string.Concat(INPUT.NUM, " ", INPUT.ENTER);
			break;
		default:
			if (KKeyCode.A <= key_code && key_code <= KKeyCode.Z)
			{
				result = ((char)(65 + (key_code - 97))).ToString();
			}
			else if (KKeyCode.Alpha0 <= key_code && key_code <= KKeyCode.Alpha9)
			{
				result = ((char)(48 + (key_code - 48))).ToString();
			}
			else if (KKeyCode.F1 <= key_code && key_code <= KKeyCode.F12)
			{
				result = "F" + (int)(key_code - 282 + 1);
			}
			else
			{
				Debug.LogWarning("Unable to find proper string for KKeyCode: " + key_code.ToString() + " using key_code.ToString()");
			}
			break;
		case KKeyCode.None:
			break;
		}
		return result;
	}

	public static string GetActionString(Action action)
	{
		string result = "";
		if (action == Action.NumActions)
		{
			return result;
		}
		BindingEntry bindingEntry = ActionToBinding(action);
		KKeyCode mKeyCode = bindingEntry.mKeyCode;
		if (bindingEntry.mModifier == Modifier.None)
		{
			return GetKeycodeLocalized(mKeyCode).ToUpper();
		}
		string str = "";
		switch (bindingEntry.mModifier)
		{
		case Modifier.Shift:
			str = GetKeycodeLocalized(KKeyCode.LeftShift).ToUpper();
			break;
		case Modifier.Ctrl:
			str = GetKeycodeLocalized(KKeyCode.LeftControl).ToUpper();
			break;
		case Modifier.CapsLock:
			str = GetKeycodeLocalized(KKeyCode.CapsLock).ToUpper();
			break;
		case Modifier.Alt:
			str = GetKeycodeLocalized(KKeyCode.LeftAlt).ToUpper();
			break;
		case Modifier.Backtick:
			str = GetKeycodeLocalized(KKeyCode.BackQuote).ToUpper();
			break;
		}
		return str + " + " + GetKeycodeLocalized(mKeyCode).ToUpper();
	}

	public static void CreateExplosion(Vector3 explosion_pos)
	{
		Vector2 b = new Vector2(explosion_pos.x, explosion_pos.y);
		float num = 5f;
		float num2 = num * num;
		foreach (Health item in Components.Health.Items)
		{
			Vector3 position = item.transform.GetPosition();
			Vector2 a = new Vector2(position.x, position.y);
			float sqrMagnitude = (a - b).sqrMagnitude;
			if (num2 >= sqrMagnitude && item != null)
			{
				item.Damage(item.maxHitPoints);
			}
		}
	}

	private static void GetNonSolidCells(int x, int y, List<int> cells, int min_x, int min_y, int max_x, int max_y)
	{
		int num = Grid.XYToCell(x, y);
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.DupePassable[num] && x >= min_x && x <= max_x && y >= min_y && y <= max_y && !cells.Contains(num))
		{
			cells.Add(num);
			GetNonSolidCells(x + 1, y, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x - 1, y, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x, y + 1, cells, min_x, min_y, max_x, max_y);
			GetNonSolidCells(x, y - 1, cells, min_x, min_y, max_x, max_y);
		}
	}

	public static void GetNonSolidCells(int cell, int radius, List<int> cells)
	{
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		GetNonSolidCells(x, y, cells, x - radius, y - radius, x + radius, y + radius);
	}

	public static float GetMaxSressInActiveWorld()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (item.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
			{
				num = Mathf.Max(num, Db.Get().Amounts.Stress.Lookup(item).value);
			}
		}
		return num;
	}

	public static float GetAverageStressInActiveWorld()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		int num2 = 0;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (item.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
			{
				num += Db.Get().Amounts.Stress.Lookup(item).value;
				num2++;
			}
		}
		return num / (float)num2;
	}

	public static string MigrateFMOD(FMODAsset asset)
	{
		if (asset == null)
		{
			return null;
		}
		return (asset.path != null) ? asset.path : asset.name;
	}

	private static void SortGameObjectDescriptors(List<IGameObjectEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2)
		{
			int num = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType());
			int value = TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType());
			return num.CompareTo(value);
		});
	}

	public static void IndentListOfDescriptors(List<Descriptor> list, int indentCount = 1)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Descriptor value = list[i];
			for (int j = 0; j < indentCount; j++)
			{
				value.IncreaseIndent();
			}
			list[i] = value;
		}
	}

	public static List<Descriptor> GetAllDescriptors(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors == null)
			{
				continue;
			}
			foreach (Descriptor item2 in descriptors)
			{
				if (!item2.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(item2);
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2 != null && component2.AdditionalRequirements != null)
		{
			foreach (Descriptor additionalRequirement in component2.AdditionalRequirements)
			{
				if (!additionalRequirement.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(additionalRequirement);
				}
			}
		}
		if (component2 != null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor additionalEffect in component2.AdditionalEffects)
			{
				if (!additionalEffect.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(additionalEffect);
				}
			}
		}
		return list;
	}

	public static List<Descriptor> GetDetailDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Detail)
			{
				list.Add(descriptor);
			}
		}
		IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetRequirementDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Requirement)
			{
				list.Add(descriptor);
			}
		}
		IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetEffectDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Effect || descriptor.type == Descriptor.DescriptorType.DiseaseSource)
			{
				list.Add(descriptor);
			}
		}
		IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetInformationDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Lifecycle)
			{
				list.Add(descriptor);
			}
		}
		IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetCropOptimumConditionDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Lifecycle)
			{
				Descriptor item = descriptor;
				item.text = "• " + item.text;
				list.Add(item);
			}
		}
		IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetGameObjectRequirements(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors == null)
			{
				continue;
			}
			foreach (Descriptor item2 in descriptors)
			{
				if (item2.type == Descriptor.DescriptorType.Requirement)
				{
					list.Add(item2);
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2.AdditionalRequirements != null)
		{
			list.AddRange(component2.AdditionalRequirements);
		}
		return list;
	}

	public static List<Descriptor> GetGameObjectEffects(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor item in list2)
		{
			List<Descriptor> descriptors = item.GetDescriptors(go);
			if (descriptors == null)
			{
				continue;
			}
			foreach (Descriptor item2 in descriptors)
			{
				if ((!item2.onlyForSimpleInfoScreen || simpleInfoScreen) && (item2.type == Descriptor.DescriptorType.Effect || item2.type == Descriptor.DescriptorType.DiseaseSource))
				{
					list.Add(item2);
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2 != null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor additionalEffect in component2.AdditionalEffects)
			{
				if (!additionalEffect.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(additionalEffect);
				}
			}
		}
		return list;
	}

	public static List<Descriptor> GetPlantRequirementDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> allDescriptors = GetAllDescriptors(go);
		List<Descriptor> requirementDescriptors = GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTREQUIREMENTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTREQUIREMENTS, Descriptor.DescriptorType.Requirement);
			list.Add(item);
			list.AddRange(requirementDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantLifeCycleDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> informationDescriptors = GetInformationDescriptors(GetAllDescriptors(go));
		if (informationDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.LIFECYCLE, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTLIFECYCLE, Descriptor.DescriptorType.Lifecycle);
			list.Add(item);
			list.AddRange(informationDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantEffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Growing component = go.GetComponent<Growing>();
		if (component == null)
		{
			return list;
		}
		List<Descriptor> allDescriptors = GetAllDescriptors(go);
		List<Descriptor> list2 = new List<Descriptor>();
		list2.AddRange(GetEffectDescriptors(allDescriptors));
		if (list2.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTEFFECTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTEFFECTS);
			list.Add(item);
			list.AddRange(list2);
		}
		return list;
	}

	public static string GetGameObjectEffectsTooltipString(GameObject go)
	{
		string text = "";
		List<Descriptor> gameObjectEffects = GetGameObjectEffects(go);
		if (gameObjectEffects.Count > 0)
		{
			text = string.Concat(text, UI.BUILDINGEFFECTS.OPERATIONEFFECTS, "\n");
		}
		foreach (Descriptor item in gameObjectEffects)
		{
			text = text + item.IndentedText() + "\n";
		}
		return text;
	}

	public static List<Descriptor> GetEquipmentEffects(EquipmentDef def)
	{
		Debug.Assert(def != null);
		List<Descriptor> list = new List<Descriptor>();
		List<AttributeModifier> attributeModifiers = def.AttributeModifiers;
		if (attributeModifiers != null)
		{
			foreach (AttributeModifier item in attributeModifiers)
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(item.AttributeId);
				string name = attribute.Name;
				string formattedString = item.GetFormattedString();
				string newValue = ((item.Value >= 0f) ? "produced" : "consumed");
				string text = UI.GAMEOBJECTEFFECTS.EQUIPMENT_MODS.text.Replace("{Attribute}", name).Replace("{Style}", newValue).Replace("{Value}", formattedString);
				list.Add(new Descriptor(text, text));
			}
		}
		return list;
	}

	public static string GetRecipeDescription(Recipe recipe)
	{
		string text = null;
		if (recipe != null)
		{
			text = recipe.recipeDescription;
		}
		if (text == null)
		{
			text = RESEARCH.TYPES.MISSINGRECIPEDESC;
			Debug.LogWarning("Missing recipeDescription");
		}
		return text;
	}

	public static int GetCurrentCycle()
	{
		return GameClock.Instance.GetCycle() + 1;
	}

	public static float GetCurrentTimeInCycles()
	{
		return GameClock.Instance.GetTimeInCycles() + 1f;
	}

	public static GameObject GetActiveTelepad()
	{
		GameObject telepad = GetTelepad(ClusterManager.Instance.activeWorldId);
		if (telepad == null)
		{
			telepad = GetTelepad(0);
		}
		return telepad;
	}

	public static GameObject GetTelepad(int worldId)
	{
		if (Components.Telepads.Count > 0)
		{
			for (int i = 0; i < Components.Telepads.Count; i++)
			{
				if (Components.Telepads[i].GetMyWorldId() == worldId)
				{
					return Components.Telepads[i].gameObject;
				}
			}
		}
		return null;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original, position, sceneLayer, null, name, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, GameObject parent, string name = null, int gameLayer = 0)
	{
		position.z = Grid.GetLayerZ(sceneLayer);
		return Util.KInstantiate(original, position, Quaternion.identity, parent, name, initialize_id: true, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public static GameObject KInstantiate(Component original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return KInstantiate(original.gameObject, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public unsafe static void IsEmissionBlocked(int cell, out bool all_not_gaseous, out bool all_over_pressure)
	{
		int* ptr = stackalloc int[4];
		*ptr = Grid.CellBelow(cell);
		ptr[1] = Grid.CellLeft(cell);
		ptr[2] = Grid.CellRight(cell);
		ptr[3] = Grid.CellAbove(cell);
		all_not_gaseous = true;
		all_over_pressure = true;
		for (int i = 0; i < 4; i++)
		{
			int num = ptr[i];
			if (Grid.IsValidCell(num))
			{
				Element element = Grid.Element[num];
				all_not_gaseous = all_not_gaseous && !element.IsGas && !element.IsVacuum;
				all_over_pressure = all_over_pressure && ((!element.IsGas && !element.IsVacuum) || Grid.Mass[num] >= 1.8f);
			}
		}
	}

	public static float GetDecorAtCell(int cell)
	{
		float result = 0f;
		if (!Grid.Solid[cell])
		{
			result = Grid.Decor[cell];
			result += (float)DecorProvider.GetLightDecorBonus(cell);
		}
		return result;
	}

	public static string GetKeywordStyle(Tag tag)
	{
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			return GetKeywordStyle(element);
		}
		if (foodTags.Contains(tag))
		{
			return "food";
		}
		if (solidTags.Contains(tag))
		{
			return "solid";
		}
		return null;
	}

	public static string GetKeywordStyle(SimHashes hash)
	{
		Element element = ElementLoader.FindElementByHash(hash);
		if (element != null)
		{
			return GetKeywordStyle(element);
		}
		return null;
	}

	public static string GetKeywordStyle(Element element)
	{
		if (element.id == SimHashes.Oxygen)
		{
			return "oxygen";
		}
		if (element.IsSolid)
		{
			return "solid";
		}
		if (element.IsLiquid)
		{
			return "liquid";
		}
		if (element.IsGas)
		{
			return "gas";
		}
		if (element.IsVacuum)
		{
			return "vacuum";
		}
		return null;
	}

	public static string GetKeywordStyle(GameObject go)
	{
		string result = "";
		Edible component = go.GetComponent<Edible>();
		Equippable component2 = go.GetComponent<Equippable>();
		MedicinalPill component3 = go.GetComponent<MedicinalPill>();
		ResearchPointObject component4 = go.GetComponent<ResearchPointObject>();
		if (component != null)
		{
			result = "food";
		}
		else if (component2 != null)
		{
			result = "equipment";
		}
		else if (component3 != null)
		{
			result = "medicine";
		}
		else if (component4 != null)
		{
			result = "research";
		}
		return result;
	}

	public static Sprite GetBiomeSprite(string id)
	{
		string text = "biomeIcon" + char.ToUpper(id[0]) + id.Substring(1).ToLower();
		Sprite sprite = Assets.GetSprite(text);
		if (sprite != null)
		{
			Tuple<Sprite, Color> tuple = new Tuple<Sprite, Color>(sprite, Color.white);
			return tuple.first;
		}
		Debug.LogWarning("Missing codex biome icon: " + text);
		return null;
	}

	public static string GenerateRandomDuplicantName()
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		bool flag = UnityEngine.Random.Range(0f, 1f) >= 0.5f;
		List<string> list = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.NB)));
		list.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.FEMALE)));
		text3 = list.GetRandom();
		if (UnityEngine.Random.Range(0f, 1f) > 0.7f)
		{
			List<string> list2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.NB)));
			list2.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.FEMALE)));
			text = list2.GetRandom();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text += " ";
		}
		if (UnityEngine.Random.Range(0f, 1f) >= 0.9f)
		{
			List<string> list3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.NB)));
			list3.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.FEMALE)));
			text2 = list3.GetRandom();
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = " " + text2;
		}
		return text + text3 + text2;
	}

	public static string GenerateRandomLaunchPadName()
	{
		return NAMEGEN.LAUNCHPAD.FORMAT.Replace("{Name}", UnityEngine.Random.Range(1, 1000).ToString());
	}

	public static string GenerateRandomRocketName()
	{
		string text = "";
		string newValue = "";
		string newValue2 = "";
		string newValue3 = "";
		int num = 1;
		int num2 = 2;
		int num3 = 4;
		List<string> tList = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.NOUN)));
		text = tList.GetRandom();
		int num4 = 0;
		if (UnityEngine.Random.value > 0.7f)
		{
			List<string> tList2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.PREFIX)));
			newValue = tList2.GetRandom();
			num4 |= num;
		}
		if (UnityEngine.Random.value > 0.5f)
		{
			List<string> tList3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.ADJECTIVE)));
			newValue2 = tList3.GetRandom();
			num4 |= num2;
		}
		if (UnityEngine.Random.value > 0.1f)
		{
			List<string> tList4 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.SUFFIX)));
			newValue3 = tList4.GetRandom();
			num4 |= num3;
		}
		string text2 = ((num4 == (num | num2 | num3)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN_SUFFIX) : ((num4 == (num2 | num3)) ? ((string)NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN_SUFFIX) : ((num4 == (num | num3)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_NOUN_SUFFIX) : ((num4 == num3) ? ((string)NAMEGEN.ROCKET.FMT_NOUN_SUFFIX) : ((num4 == (num | num2)) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN) : ((num4 == num) ? ((string)NAMEGEN.ROCKET.FMT_PREFIX_NOUN) : ((num4 != num2) ? ((string)NAMEGEN.ROCKET.FMT_NOUN) : ((string)NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN))))))));
		DebugUtil.LogArgs("Rocket name bits:", Convert.ToString(num4, 2));
		return text2.Replace("{Prefix}", newValue).Replace("{Adjective}", newValue2).Replace("{Noun}", text)
			.Replace("{Suffix}", newValue3);
	}

	public static string GenerateRandomWorldName(string worldType)
	{
		if (string.IsNullOrEmpty(worldType))
		{
			Debug.LogWarning("No name table provided to generate world name. Using GENERIC");
			worldType = "GENERIC";
		}
		string text = RandomValueFromSeparatedString(Strings.Get("STRINGS.NAMEGEN.WORLD.ROOTS." + worldType.ToUpper()));
		if (string.IsNullOrEmpty(text))
		{
			text = RandomValueFromSeparatedString(Strings.Get(NAMEGEN.WORLD.ROOTS.GENERIC));
		}
		string str = RandomValueFromSeparatedString(NAMEGEN.WORLD.SUFFIXES.GENERICLIST);
		return text + str;
	}

	public static float GetThermalComfort(int cell, float tolerance = -0.08368001f)
	{
		float num = 0f;
		Element element = ElementLoader.FindElementByHash(SimHashes.Creature);
		Element element2 = Grid.Element[cell];
		if (element2.thermalConductivity != 0f)
		{
			num = SimUtil.CalculateEnergyFlowCreatures(cell, 310.15f, element.specificHeatCapacity, element.thermalConductivity, 1f, 0.0045f);
		}
		num -= tolerance;
		return num * 1000f;
	}

	public static string RandomValueFromSeparatedString(string source, string separator = "\n")
	{
		int startIndex = 0;
		int num = 0;
		while (true)
		{
			startIndex = source.IndexOf(separator, startIndex);
			if (startIndex != -1)
			{
				startIndex += separator.Length;
				num++;
				continue;
			}
			break;
		}
		if (num == 0)
		{
			return "";
		}
		int num2 = UnityEngine.Random.Range(0, num);
		startIndex = 0;
		for (int i = 0; i < num2; i++)
		{
			startIndex = source.IndexOf(separator, startIndex) + separator.Length;
		}
		int num3 = source.IndexOf(separator, startIndex);
		return source.Substring(startIndex, (num3 == -1) ? (source.Length - startIndex) : (num3 - startIndex));
	}

	public static string GetFormattedDiseaseName(byte idx, bool color = false)
	{
		Disease disease = Db.Get().Diseases[idx];
		if (color)
		{
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT, disease.Name, ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName)));
		}
		return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT_NO_COLOR, disease.Name);
	}

	public static string GetFormattedDisease(byte idx, int units, bool color = false)
	{
		if (idx != byte.MaxValue && units > 0)
		{
			Disease disease = Db.Get().Diseases[idx];
			if (color)
			{
				return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT, disease.Name, GetFormattedDiseaseAmount(units), ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName)));
			}
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT_NO_COLOR, disease.Name, GetFormattedDiseaseAmount(units));
		}
		return UI.OVERLAYS.DISEASE.NO_DISEASE;
	}

	public static string GetFormattedDiseaseAmount(int units, TimeSlice timeSlice = TimeSlice.None)
	{
		float num = ApplyTimeSlice(units, timeSlice);
		return AddTimeSliceText(units.ToString("#,##0") + UI.UNITSUFFIXES.DISEASE.UNITS, timeSlice);
	}

	public static string ColourizeString(Color32 colour, string str)
	{
		return $"<color=#{ColourToHex(colour)}>{str}</color>";
	}

	public static string ColourToHex(Color32 colour)
	{
		return $"{colour.r:X2}{colour.g:X2}{colour.b:X2}{colour.a:X2}";
	}

	public static string GetFormattedDecor(float value, bool enforce_max = false)
	{
		string arg = "";
		LocString loc_string = ((value > DecorMonitor.MAXIMUM_DECOR_VALUE && enforce_max) ? UI.OVERLAYS.DECOR.MAXIMUM_DECOR : UI.OVERLAYS.DECOR.VALUE);
		if (enforce_max)
		{
			value = Math.Min(value, DecorMonitor.MAXIMUM_DECOR_VALUE);
		}
		if (value > 0f)
		{
			arg = "+";
		}
		else if (!(value < 0f))
		{
			loc_string = UI.OVERLAYS.DECOR.VALUE_ZERO;
		}
		return string.Format(loc_string, arg, value);
	}

	public static Color GetDecorColourFromValue(int decor)
	{
		Color black = Color.black;
		float num = (float)decor / 100f;
		if (num > 0f)
		{
			return Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num));
		}
		return Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num));
	}

	public static List<Descriptor> GetMaterialDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string txt = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
				string tooltip = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(txt, tooltip);
				item.IncreaseIndent();
				list.Add(item);
			}
		}
		list.AddRange(GetSignificantMaterialPropertyDescriptors(element));
		return list;
	}

	public static string GetMaterialTooltips(Element element)
	{
		string str = element.tag.ProperName();
		foreach (AttributeModifier attributeModifier in element.attributeModifiers)
		{
			string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
			string formattedString = attributeModifier.GetFormattedString();
			str = str + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
		}
		return str + GetSignificantMaterialPropertyTooltips(element);
	}

	public static string GetSignificantMaterialPropertyTooltips(Element element)
	{
		string text = "";
		List<Descriptor> significantMaterialPropertyDescriptors = GetSignificantMaterialPropertyDescriptors(element);
		if (significantMaterialPropertyDescriptors.Count > 0)
		{
			text += "\n";
			for (int i = 0; i < significantMaterialPropertyDescriptors.Count; i++)
			{
				text = text + "    • " + Util.StripTextFormatting(significantMaterialPropertyDescriptors[i].text) + "\n";
			}
		}
		return text;
	}

	public static List<Descriptor> GetSignificantMaterialPropertyDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.thermalConductivity > 10f)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.HIGH_THERMAL_CONDUCTIVITY, GetThermalConductivityString(element, addColor: false, addValue: false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")));
			item.IncreaseIndent();
			list.Add(item);
		}
		if (element.thermalConductivity < 1f)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.LOW_THERMAL_CONDUCTIVITY, GetThermalConductivityString(element, addColor: false, addValue: false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")));
			item2.IncreaseIndent();
			list.Add(item2);
		}
		if (element.specificHeatCapacity <= 0.2f)
		{
			Descriptor item3 = default(Descriptor);
			item3.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.LOW_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f));
			item3.IncreaseIndent();
			list.Add(item3);
		}
		if (element.specificHeatCapacity >= 1f)
		{
			Descriptor item4 = default(Descriptor);
			item4.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.HIGH_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f));
			item4.IncreaseIndent();
			list.Add(item4);
		}
		return list;
	}

	public static int NaturalBuildingCell(this KMonoBehaviour cmp)
	{
		return Grid.PosToCell(cmp.transform.GetPosition());
	}

	public static List<Descriptor> GetMaterialDescriptors(Tag tag)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			if (element.attributeModifiers.Count > 0)
			{
				foreach (AttributeModifier attributeModifier in element.attributeModifiers)
				{
					string txt = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
					string tooltip = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
					Descriptor item = default(Descriptor);
					item.SetupDescriptor(txt, tooltip);
					item.IncreaseIndent();
					list.Add(item);
				}
			}
			list.AddRange(GetSignificantMaterialPropertyDescriptors(element));
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if (gameObject != null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if (component != null)
				{
					foreach (AttributeModifier descriptor in component.descriptors)
					{
						string txt2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + descriptor.AttributeId.ToUpper())), descriptor.GetFormattedString());
						string tooltip2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + descriptor.AttributeId.ToUpper())), descriptor.GetFormattedString());
						Descriptor item2 = default(Descriptor);
						item2.SetupDescriptor(txt2, tooltip2);
						item2.IncreaseIndent();
						list.Add(item2);
					}
				}
			}
		}
		return list;
	}

	public static string GetMaterialTooltips(Tag tag)
	{
		string text = tag.ProperName();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString();
				text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
			}
			text += GetSignificantMaterialPropertyTooltips(element);
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if (gameObject != null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if (component != null)
				{
					foreach (AttributeModifier descriptor in component.descriptors)
					{
						string name2 = Db.Get().BuildingAttributes.Get(descriptor.AttributeId).Name;
						string formattedString2 = descriptor.GetFormattedString();
						text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name2, formattedString2);
					}
				}
			}
		}
		return text;
	}

	public static bool AreChoresUIMergeable(Chore.Precondition.Context choreA, Chore.Precondition.Context choreB)
	{
		if (choreA.chore.target.isNull || choreB.chore.target.isNull)
		{
			return false;
		}
		ChoreType choreType = choreB.chore.choreType;
		ChoreType choreType2 = choreA.chore.choreType;
		if (choreA.chore.choreType == choreB.chore.choreType && choreA.chore.target.GetComponent<KPrefabID>().PrefabTag == choreB.chore.target.GetComponent<KPrefabID>().PrefabTag)
		{
			return true;
		}
		if (choreA.chore.choreType == Db.Get().ChoreTypes.Dig && choreB.chore.choreType == Db.Get().ChoreTypes.Dig)
		{
			return true;
		}
		if (choreA.chore.choreType == Db.Get().ChoreTypes.Relax && choreB.chore.choreType == Db.Get().ChoreTypes.Relax)
		{
			return true;
		}
		if ((choreType2 == Db.Get().ChoreTypes.ReturnSuitIdle || choreType2 == Db.Get().ChoreTypes.ReturnSuitUrgent) && (choreType == Db.Get().ChoreTypes.ReturnSuitIdle || choreType == Db.Get().ChoreTypes.ReturnSuitUrgent))
		{
			return true;
		}
		if (choreA.chore.target.gameObject == choreB.chore.target.gameObject && choreA.chore.choreType == choreB.chore.choreType)
		{
			return true;
		}
		return false;
	}

	public static string GetChoreName(Chore chore, object choreData)
	{
		string result = "";
		if (chore.choreType == Db.Get().ChoreTypes.Fetch || chore.choreType == Db.Get().ChoreTypes.MachineFetch || chore.choreType == Db.Get().ChoreTypes.FabricateFetch || chore.choreType == Db.Get().ChoreTypes.FetchCritical || chore.choreType == Db.Get().ChoreTypes.PowerFetch)
		{
			result = chore.GetReportName(chore.gameObject.GetProperName());
		}
		else if (chore.choreType == Db.Get().ChoreTypes.StorageFetch || chore.choreType == Db.Get().ChoreTypes.FoodFetch)
		{
			FetchChore fetchChore = chore as FetchChore;
			FetchAreaChore fetchAreaChore = chore as FetchAreaChore;
			if (fetchAreaChore != null)
			{
				GameObject getFetchTarget = fetchAreaChore.GetFetchTarget;
				KMonoBehaviour kMonoBehaviour = choreData as KMonoBehaviour;
				result = ((getFetchTarget != null) ? chore.GetReportName(getFetchTarget.GetProperName()) : ((!(kMonoBehaviour != null)) ? chore.GetReportName() : chore.GetReportName(kMonoBehaviour.GetProperName())));
			}
			else if (fetchChore != null)
			{
				Pickupable fetchTarget = fetchChore.fetchTarget;
				KMonoBehaviour kMonoBehaviour2 = choreData as KMonoBehaviour;
				result = ((fetchTarget != null) ? chore.GetReportName(fetchTarget.GetProperName()) : ((!(kMonoBehaviour2 != null)) ? chore.GetReportName() : chore.GetReportName(kMonoBehaviour2.GetProperName())));
			}
		}
		else
		{
			result = chore.GetReportName();
		}
		return result;
	}

	public static string ChoreGroupsForChoreType(ChoreType choreType)
	{
		if (choreType.groups == null || choreType.groups.Length == 0)
		{
			return null;
		}
		string text = "";
		for (int i = 0; i < choreType.groups.Length; i++)
		{
			if (i != 0)
			{
				text += UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_GROUP_SEPARATOR;
			}
			text += choreType.groups[i].Name;
		}
		return text;
	}

	public static bool IsCapturingTimeLapse()
	{
		return Game.Instance != null && Game.Instance.timelapser != null && Game.Instance.timelapser.CapturingTimelapseScreenshot;
	}

	public static ExposureType GetExposureTypeForDisease(Disease disease)
	{
		for (int i = 0; i < GERM_EXPOSURE.TYPES.Length; i++)
		{
			if (disease.id == GERM_EXPOSURE.TYPES[i].germ_id)
			{
				return GERM_EXPOSURE.TYPES[i];
			}
		}
		return null;
	}

	public static Sickness GetSicknessForDisease(Disease disease)
	{
		for (int i = 0; i < GERM_EXPOSURE.TYPES.Length; i++)
		{
			if (disease.id == GERM_EXPOSURE.TYPES[i].germ_id)
			{
				if (GERM_EXPOSURE.TYPES[i].sickness_id == null)
				{
					return null;
				}
				return Db.Get().Sicknesses.Get(GERM_EXPOSURE.TYPES[i].sickness_id);
			}
		}
		return null;
	}

	public static void SubscribeToTags<T>(T target, EventSystem.IntraObjectHandler<T> handler, bool triggerImmediately) where T : KMonoBehaviour
	{
		if (triggerImmediately)
		{
			handler.Trigger(target.gameObject, new TagChangedEventData(Tag.Invalid, added: false));
		}
		target.Subscribe(-1582839653, handler);
	}

	public static void UnsubscribeToTags<T>(T target, EventSystem.IntraObjectHandler<T> handler) where T : KMonoBehaviour
	{
		target.Unsubscribe(-1582839653, handler);
	}

	public static EventSystem.IntraObjectHandler<T> CreateHasTagHandler<T>(Tag tag, Action<T, object> callback) where T : KMonoBehaviour
	{
		return new EventSystem.IntraObjectHandler<T>(delegate(T component, object data)
		{
			TagChangedEventData tagChangedEventData = (TagChangedEventData)data;
			if (tagChangedEventData.tag == Tag.Invalid)
			{
				KPrefabID component2 = component.GetComponent<KPrefabID>();
				tagChangedEventData = new TagChangedEventData(tag, component2.HasTag(tag));
			}
			if (tagChangedEventData.tag == tag && tagChangedEventData.added)
			{
				callback(component, data);
			}
		});
	}

	public static EventSystem.IntraObjectHandler<T> CreateDoesntHaveTagHandler<T>(Tag tag, Action<T, object> callback) where T : KMonoBehaviour
	{
		return new EventSystem.IntraObjectHandler<T>(delegate(T component, object data)
		{
			TagChangedEventData tagChangedEventData = (TagChangedEventData)data;
			if (tagChangedEventData.tag == Tag.Invalid)
			{
				KPrefabID component2 = component.GetComponent<KPrefabID>();
				tagChangedEventData = new TagChangedEventData(tag, component2.HasTag(tag));
			}
			if (tagChangedEventData.tag == tag && !tagChangedEventData.added)
			{
				callback(component, data);
			}
		});
	}
}
