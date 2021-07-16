using System.Collections.Generic;

public class WorkingToiletTracker : WorldTracker
{
	public WorkingToiletTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		int num = 0;
		List<IUsable> worldItems = Components.Toilets.GetWorldItems(base.WorldID);
		for (int i = 0; i < worldItems.Count; i++)
		{
			if (worldItems[i].IsUsable())
			{
				num++;
			}
		}
		AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
