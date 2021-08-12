using Klei.AI;
using UnityEngine;

public class StressTracker : WorldTracker
{
	public StressTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			if (Components.LiveMinionIdentities[i].GetMyWorldId() == base.WorldID)
			{
				num = Mathf.Max(num, Components.LiveMinionIdentities[i].gameObject.GetAmounts().GetValue(Db.Get().Amounts.Stress.Id));
			}
		}
		AddPoint(Mathf.Round(num));
	}

	public override string FormatValueString(float value)
	{
		return value + "%";
	}
}
