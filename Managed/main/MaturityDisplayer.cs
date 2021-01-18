using Klei.AI;
using STRINGS;
using UnityEngine;

public class MaturityDisplayer : AsPercentAmountDisplayer
{
	public class MaturityAttributeFormatter : StandardAttributeFormatter
	{
		public MaturityAttributeFormatter()
			: base(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None)
		{
		}

		public override string GetFormattedModifier(AttributeModifier modifier, GameObject parent_instance)
		{
			float num = modifier.Value;
			GameUtil.TimeSlice timeSlice = base.DeltaTimeSlice;
			if (modifier.IsMultiplier)
			{
				num *= 100f;
				timeSlice = GameUtil.TimeSlice.None;
			}
			return GetFormattedValue(num, timeSlice, parent_instance);
		}
	}

	public MaturityDisplayer()
		: base(GameUtil.TimeSlice.PerCycle)
	{
		formatter = new MaturityAttributeFormatter();
	}

	public override string GetTooltipDescription(Amount master, AmountInstance instance)
	{
		string tooltipDescription = base.GetTooltipDescription(master, instance);
		Growing component = instance.gameObject.GetComponent<Growing>();
		if (component.IsGrowing())
		{
			float seconds = (instance.GetMax() - instance.value) / instance.GetDelta();
			if (component != null && component.IsGrowing())
			{
				return tooltipDescription + string.Format(CREATURES.STATS.MATURITY.TOOLTIP_GROWING_CROP, GameUtil.GetFormattedCycles(seconds), GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest()));
			}
			return tooltipDescription + string.Format(CREATURES.STATS.MATURITY.TOOLTIP_GROWING, GameUtil.GetFormattedCycles(seconds));
		}
		if (component.ReachedNextHarvest())
		{
			return tooltipDescription + CREATURES.STATS.MATURITY.TOOLTIP_GROWN;
		}
		return tooltipDescription + CREATURES.STATS.MATURITY.TOOLTIP_STALLED;
	}

	public override string GetDescription(Amount master, AmountInstance instance)
	{
		Growing component = instance.gameObject.GetComponent<Growing>();
		if (component != null && component.IsGrowing())
		{
			return string.Format(CREATURES.STATS.MATURITY.AMOUNT_DESC_FMT, master.Name, formatter.GetFormattedValue(ToPercent(instance.value, instance)), GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest()));
		}
		return base.GetDescription(master, instance);
	}
}
