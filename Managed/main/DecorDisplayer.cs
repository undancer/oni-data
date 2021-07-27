using Klei.AI;
using STRINGS;

public class DecorDisplayer : StandardAmountDisplayer
{
	public class DecorAttributeFormatter : StandardAttributeFormatter
	{
		public DecorAttributeFormatter()
			: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
		{
		}
	}

	public DecorDisplayer()
		: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
	{
		formatter = new DecorAttributeFormatter();
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = string.Format(master.description, formatter.GetFormattedValue(instance.value));
		int cell = Grid.PosToCell(instance.gameObject);
		if (Grid.IsValidCell(cell))
		{
			text += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_CURRENT, GameUtil.GetDecorAtCell(cell));
		}
		text += "\n";
		DecorMonitor.Instance sMI = instance.gameObject.GetSMI<DecorMonitor.Instance>();
		if (sMI != null)
		{
			text += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_TODAY, formatter.GetFormattedValue(sMI.GetTodaysAverageDecor()));
			text += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_YESTERDAY, formatter.GetFormattedValue(sMI.GetYesterdaysAverageDecor()));
		}
		return text;
	}
}
