public class RocketOxidizerTracker : WorldTracker
{
	public RocketOxidizerTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		Clustercraft component = ClusterManager.Instance.GetWorld(base.WorldID).GetComponent<Clustercraft>();
		AddPoint((component != null) ? component.ModuleInterface.OxidizerPowerRemaining : 0f);
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedMass(value);
	}
}
