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
		string str = string.Format(master.description, formatter.GetFormattedValue(instance.value));
		int cell = Grid.PosToCell(instance.gameObject);
		if (Grid.IsValidCell(cell))
		{
			str += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_CURRENT, GameUtil.GetDecorAtCell(cell));
		}
		str += "\n";
		DecorMonitor.Instance sMI = instance.gameObject.GetSMI<DecorMonitor.Instance>();
		if (sMI != null)
		{
			str += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_TODAY, formatter.GetFormattedValue(sMI.GetTodaysAverageDecor()));
			str += string.Format(DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_YESTERDAY, formatter.GetFormattedValue(sMI.GetYesterdaysAverageDecor()));
		}
		return str;
	}
}
