using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class StandardAttributeFormatter : IAttributeFormatter
{
	public GameUtil.UnitClass unitClass;

	public GameUtil.TimeSlice DeltaTimeSlice
	{
		get;
		set;
	}

	public StandardAttributeFormatter(GameUtil.UnitClass unitClass, GameUtil.TimeSlice deltaTimeSlice)
	{
		this.unitClass = unitClass;
		DeltaTimeSlice = deltaTimeSlice;
	}

	public virtual string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue());
	}

	public virtual string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
	{
		return GetFormattedValue(modifier.Value, DeltaTimeSlice);
	}

	public virtual string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameObject parent_instance = null)
	{
		return unitClass switch
		{
			GameUtil.UnitClass.SimpleInteger => GameUtil.GetFormattedInt(value, timeSlice), 
			GameUtil.UnitClass.Mass => GameUtil.GetFormattedMass(value, timeSlice), 
			GameUtil.UnitClass.Temperature => GameUtil.GetFormattedTemperature(value, timeSlice, (timeSlice != 0) ? GameUtil.TemperatureInterpretation.Relative : GameUtil.TemperatureInterpretation.Absolute), 
			GameUtil.UnitClass.Percent => GameUtil.GetFormattedPercent(value, timeSlice), 
			GameUtil.UnitClass.Calories => GameUtil.GetFormattedCalories(value, timeSlice), 
			GameUtil.UnitClass.Distance => GameUtil.GetFormattedDistance(value), 
			GameUtil.UnitClass.Disease => GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(value)), 
			GameUtil.UnitClass.Radiation => GameUtil.GetFormattedRads(value, timeSlice), 
			GameUtil.UnitClass.Energy => GameUtil.GetFormattedJoules(value, "F1", timeSlice), 
			GameUtil.UnitClass.Power => GameUtil.GetFormattedWattage(value), 
			_ => GameUtil.GetFormattedSimple(value, timeSlice), 
		};
	}

	public virtual string GetTooltipDescription(Attribute master, AttributeInstance instance)
	{
		return master.Description;
	}

	public virtual string GetTooltip(Attribute master, AttributeInstance instance)
	{
		string tooltipDescription = GetTooltipDescription(master, instance);
		tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.TOTAL_VALUE, GetFormattedValue(instance.GetTotalDisplayValue()), instance.Name);
		if (instance.GetBaseValue() != 0f)
		{
			tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.BASE_VALUE, instance.GetBaseValue());
		}
		List<AttributeModifier> list = new List<AttributeModifier>();
		for (int i = 0; i < instance.Modifiers.Count; i++)
		{
			list.Add(instance.Modifiers[i]);
		}
		list.Sort((AttributeModifier p1, AttributeModifier p2) => p2.Value.CompareTo(p1.Value));
		for (int j = 0; j != list.Count; j++)
		{
			AttributeModifier attributeModifier = list[j];
			string formattedString = attributeModifier.GetFormattedString(instance.gameObject);
			if (formattedString != null)
			{
				tooltipDescription += string.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifier.GetDescription(), formattedString);
			}
		}
		string text = "";
		AttributeConverters component = instance.gameObject.GetComponent<AttributeConverters>();
		if (component != null && master.converters.Count > 0)
		{
			foreach (AttributeConverterInstance converter in component.converters)
			{
				if (converter.converter.attribute == master)
				{
					string text2 = converter.DescriptionFromAttribute(converter.Evaluate(), converter.gameObject);
					if (text2 != null)
					{
						text = text + "\n" + text2;
					}
				}
			}
		}
		if (text.Length > 0)
		{
			tooltipDescription = tooltipDescription + "\n" + text;
		}
		return tooltipDescription;
	}
}
