using Klei.AI;

public class GermResistanceAttributeFormatter : StandardAttributeFormatter
{
	public GermResistanceAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedModifier(AttributeModifier modifier)
	{
		return GameUtil.GetGermResistanceModifierString(modifier.Value, addColor: false);
	}
}
