using Klei.AI;
using UnityEngine;

public class CrewRationsEntry : CrewListEntry
{
	public KButton incRationPerDayButton;

	public KButton decRationPerDayButton;

	public LocText rationPerDayText;

	public LocText rationsEatenToday;

	public LocText currentCaloriesText;

	public LocText currentStressText;

	public LocText currentHealthText;

	public ValueTrendImageToggle stressTrendImage;

	private RationMonitor.Instance rationMonitor;

	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		rationMonitor = _identity.GetSMI<RationMonitor.Instance>();
		Refresh();
	}

	public override void Refresh()
	{
		base.Refresh();
		rationsEatenToday.text = GameUtil.GetFormattedCalories(rationMonitor.GetRationsAteToday());
		if (identity == null)
		{
			return;
		}
		foreach (AmountInstance amount in identity.GetAmounts())
		{
			float min = amount.GetMin();
			float max = amount.GetMax();
			float num = max - min;
			string str = Mathf.RoundToInt((num - (max - amount.value)) / num * 100f).ToString();
			if (amount.amount == Db.Get().Amounts.Stress)
			{
				currentStressText.text = amount.GetValueString();
				currentStressText.GetComponent<ToolTip>().toolTip = amount.GetTooltip();
				stressTrendImage.SetValue(amount);
			}
			else if (amount.amount == Db.Get().Amounts.Calories)
			{
				currentCaloriesText.text = str + "%";
				currentCaloriesText.GetComponent<ToolTip>().toolTip = amount.GetTooltip();
			}
			else if (amount.amount == Db.Get().Amounts.HitPoints)
			{
				currentHealthText.text = str + "%";
				currentHealthText.GetComponent<ToolTip>().toolTip = amount.GetTooltip();
			}
		}
	}
}
