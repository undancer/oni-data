public class RadsPerCycleAttributeFormatter : StandardAttributeFormatter
{
	public RadsPerCycleAttributeFormatter()
		: base(GameUtil.UnitClass.Radiation, GameUtil.TimeSlice.PerCycle)
	{
	}

	public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice)
	{
		return base.GetFormattedValue(value / 600f, timeSlice);
	}
}
