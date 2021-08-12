using UnityEngine;

public class BreathabilityTracker : WorldTracker
{
	public BreathabilityTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		int count = Components.LiveMinionIdentities.GetWorldItems(base.WorldID).Count;
		if (count == 0)
		{
			AddPoint(0f);
			return;
		}
		foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(base.WorldID))
		{
			OxygenBreather component = worldItem.GetComponent<OxygenBreather>();
			if (component.GetGasProvider() is GasBreatherFromWorldProvider)
			{
				if (component.IsBreathableElement)
				{
					num += 100f;
					if (component.IsLowOxygen())
					{
						num -= 50f;
					}
				}
			}
			else if (!component.IsSuffocating)
			{
				num += 100f;
				if (component.IsLowOxygen())
				{
					num -= 50f;
				}
			}
		}
		num /= (float)count;
		AddPoint(Mathf.RoundToInt(num));
	}

	public override string FormatValueString(float value)
	{
		return value + "%";
	}
}
