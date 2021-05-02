using Klei.AI;

public class FoodQualityAttributeFormatter : StandardAttributeFormatter
{
	public FoodQualityAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		return GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None);
	}

	public override string GetFormattedModifier(AttributeModifier modifier)
	{
		return GameUtil.GetFormattedInt(modifier.Value);
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice)
	{
		int quality = (int)value;
		return Util.StripTextFormatting(GameUtil.GetFormattedFoodQuality(quality));
	}
}
