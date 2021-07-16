using Klei.AI;
using STRINGS;

public class QualityOfLifeAttributeFormatter : StandardAttributeFormatter
{
	public QualityOfLifeAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		return string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.DESC_FORMAT, GetFormattedValue(instance.GetTotalDisplayValue()), GetFormattedValue(attributeInstance.GetTotalDisplayValue()));
	}

	public override string GetTooltip(Attribute master, AttributeInstance instance)
	{
		string tooltip = base.GetTooltip(master, instance);
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		tooltip = tooltip + "\n\n" + string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION, GetFormattedValue(attributeInstance.GetTotalDisplayValue()));
		if (instance.GetTotalDisplayValue() - attributeInstance.GetTotalDisplayValue() >= 0f)
		{
			return tooltip + "\n\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_OVER;
		}
		return tooltip + "\n\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_UNDER;
	}
}
