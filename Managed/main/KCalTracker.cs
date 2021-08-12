public class KCalTracker : WorldTracker
{
	public KCalTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		AddPoint(RationTracker.Get().CountRations(null, ClusterManager.Instance.GetWorld(base.WorldID).worldInventory));
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedCalories(value);
	}
}
