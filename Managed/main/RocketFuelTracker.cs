public class RocketFuelTracker : WorldTracker
{
	public RocketFuelTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		Clustercraft component = ClusterManager.Instance.GetWorld(base.WorldID).GetComponent<Clustercraft>();
		AddPoint((component != null) ? component.ModuleInterface.FuelRemaining : 0f);
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedMass(value);
	}
}
