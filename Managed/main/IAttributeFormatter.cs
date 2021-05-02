using System.Collections.Generic;
using Klei.AI;

public interface IAttributeFormatter
{
	GameUtil.TimeSlice DeltaTimeSlice
	{
		get;
		set;
	}

	string GetFormattedAttribute(AttributeInstance instance);

	string GetFormattedModifier(AttributeModifier modifier);

	string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice);

	string GetTooltip(Attribute master, AttributeInstance instance);

	string GetTooltip(Attribute master, List<AttributeModifier> modifiers, AttributeConverters converters);
}
