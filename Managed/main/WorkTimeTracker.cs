using System.Collections.Generic;
using UnityEngine;

public class WorkTimeTracker : WorldTracker
{
	public ChoreGroup choreGroup;

	public WorkTimeTracker(int worldID, ChoreGroup group)
		: base(worldID)
	{
		choreGroup = group;
	}

	public override void UpdateData()
	{
		float num = 0f;
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.WorldID);
		Chore chore;
		foreach (MinionIdentity item in worldItems)
		{
			chore = item.GetComponent<ChoreConsumer>().choreDriver.GetCurrentChore();
			if (chore != null && choreGroup.choreTypes.Find((ChoreType match) => match == chore.choreType) != null)
			{
				num += 1f;
			}
		}
		AddPoint(num / (float)worldItems.Count * 100f);
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedPercent(Mathf.Round(value)).ToString();
	}
}
