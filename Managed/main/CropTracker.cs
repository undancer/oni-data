using System.Collections.Generic;

public class CropTracker : WorldTracker
{
	public CropTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.WorldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
		for (int i = 0; i < list.Count; i++)
		{
			if (!(list[i].plant == null) && !list[i].plant.HasTag(GameTags.Wilting))
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
