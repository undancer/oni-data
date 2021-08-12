using System.Collections.Generic;

public class IdleTracker : WorldTracker
{
	public IdleTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		int num = 0;
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.WorldID);
		for (int i = 0; i < worldItems.Count; i++)
		{
			if (worldItems[i].HasTag(GameTags.Idle))
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
