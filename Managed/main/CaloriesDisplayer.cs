using Klei.AI;

public class CaloriesDisplayer : StandardAmountDisplayer
{
	public class CaloriesAttributeFormatter : StandardAttributeFormatter
	{
		public CaloriesAttributeFormatter()
			: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle)
		{
		}

		public override string GetFormattedModifier(AttributeModifier modifier)
		{
			if (modifier.IsMultiplier)
			{
				return GameUtil.GetFormattedPercent((0f - modifier.Value) * 100f);
			}
			return base.GetFormattedModifier(modifier);
		}
	}

	public CaloriesDisplayer()
		: base(GameUtil.UnitClass.Calories, GameUtil.TimeSlice.PerCycle)
	{
		formatter = new CaloriesAttributeFormatter();
	}
}
