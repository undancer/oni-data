public class CropTracker : WorldTracker
{
	public CropTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		foreach (PlantablePlot item in Components.PlantablePlots.GetItems(base.WorldID))
		{
			if (!(item.plant == null) && item.HasDepositTag(GameTags.CropSeed) && !item.plant.HasTag(GameTags.Wilting))
			{
				num += 1f;
			}
		}
		AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value + "%";
	}
}
