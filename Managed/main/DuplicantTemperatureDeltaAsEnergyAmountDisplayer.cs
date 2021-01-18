using Klei.AI;
using STRINGS;

public class DuplicantTemperatureDeltaAsEnergyAmountDisplayer : StandardAmountDisplayer
{
	public DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass unitClass, GameUtil.TimeSlice timeSlice)
		: base(unitClass, timeSlice)
	{
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string str = string.Format(master.description, formatter.GetFormattedValue(instance.value), formatter.GetFormattedValue(310.15f));
		float num = ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * 30f * 1000f;
		str += "\n\n";
		if (formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
		{
			str += string.Format(UI.CHANGEPERCYCLE, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
		}
		else
		{
			str += string.Format(UI.CHANGEPERSECOND, formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond));
			str = str + "\n" + string.Format(UI.CHANGEPERSECOND, GameUtil.GetFormattedJoules(instance.deltaAttribute.GetTotalDisplayValue() * num));
		}
		for (int i = 0; i != instance.deltaAttribute.Modifiers.Count; i++)
		{
			AttributeModifier attributeModifier = instance.deltaAttribute.Modifiers[i];
			str = str + "\n" + string.Format(UI.MODIFIER_ITEM_TEMPLATE, attributeModifier.GetDescription(), GameUtil.GetFormattedHeatEnergyRate(attributeModifier.Value * num * 1f));
		}
		return str;
	}
}
