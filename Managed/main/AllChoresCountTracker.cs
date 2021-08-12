public class AllChoresCountTracker : WorldTracker
{
	public AllChoresCountTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
		{
			num += TrackerTool.Instance.GetChoreGroupTracker(base.WorldID, Db.Get().ChoreGroups[i]).GetCurrentValue();
		}
		AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
