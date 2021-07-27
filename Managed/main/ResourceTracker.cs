public class ResourceTracker : WorldTracker
{
	public Tag tag { get; private set; }

	public ResourceTracker(int worldID, Tag materialCategoryTag)
		: base(worldID)
	{
		tag = materialCategoryTag;
	}

	public override void UpdateData()
	{
		if (!(ClusterManager.Instance.GetWorld(base.WorldID).worldInventory == null))
		{
			AddPoint(ClusterManager.Instance.GetWorld(base.WorldID).worldInventory.GetAmount(tag, includeRelatedWorlds: false));
		}
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedMass(value);
	}
}
