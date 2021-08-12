using Klei.AI;

public class PercentAttributeFormatter : StandardAttributeFormatter
{
	public PercentAttributeFormatter()
		: base(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue(), base.DeltaTimeSlice);
	}

	public override string GetFormattedModifier(AttributeModifier modifier)
	{
		return GetFormattedValue(modifier.Value, base.DeltaTimeSlice);
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice)
	{
		return GameUtil.GetFormattedPercent(value * 100f, timeSlice);
	}
}
