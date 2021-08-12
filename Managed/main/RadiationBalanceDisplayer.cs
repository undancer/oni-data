using Klei.AI;
using STRINGS;
using UnityEngine;

public class RadiationBalanceDisplayer : StandardAmountDisplayer
{
	public class RadiationAttributeFormatter : StandardAttributeFormatter
	{
		public RadiationAttributeFormatter()
			: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
		{
		}
	}

	public RadiationBalanceDisplayer()
		: base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
	{
		formatter = new RadiationAttributeFormatter();
	}

	public override string GetValueString(Amount master, AmountInstance instance)
	{
		return base.GetValueString(master, instance) + UI.UNITSUFFIXES.RADIATION.RADS;
	}

	public override string GetTooltip(Amount master, AmountInstance instance)
	{
		string text = "";
		if (instance.gameObject.GetSMI<RadiationMonitor.Instance>() != null)
		{
			int num = Grid.PosToCell(instance.gameObject);
			if (Grid.IsValidCell(num))
			{
				text += DUPLICANTS.STATS.RADIATIONBALANCE.TOOLTIP_CURRENT_BALANCE;
			}
			text += "\n\n";
			float num2 = 1f - Db.Get().Attributes.RadiationResistance.Lookup(instance.gameObject).GetTotalValue();
			text += string.Format(DUPLICANTS.STATS.RADIATIONBALANCE.CURRENT_EXPOSURE, Mathf.RoundToInt(Grid.Radiation[num] * num2));
			text += "\n";
			text += string.Format(DUPLICANTS.STATS.RADIATIONBALANCE.CURRENT_REJUVENATION, Mathf.RoundToInt(Db.Get().Attributes.RadiationRecovery.Lookup(instance.gameObject).GetTotalValue() * 600f));
		}
		return text;
	}
}
