public class AllWorkTimeTracker : WorldTracker
{
	public AllWorkTimeTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
		{
			num += TrackerTool.Instance.GetWorkTimeTracker(base.WorldID, Db.Get().ChoreGroups[i]).GetCurrentValue();
		}
		AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedPercent(value).ToString();
	}
}
