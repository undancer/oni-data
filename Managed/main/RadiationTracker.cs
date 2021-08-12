using System.Collections.Generic;
using Klei.AI;

public class RadiationTracker : WorldTracker
{
	public RadiationTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(base.WorldID);
		if (worldItems.Count == 0)
		{
			AddPoint(0f);
			return;
		}
		foreach (MinionIdentity item in worldItems)
		{
			num += item.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id).value;
		}
		float value = num / (float)worldItems.Count;
		AddPoint(value);
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedRads(value);
	}
}
