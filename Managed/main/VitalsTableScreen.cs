using Klei.AI;
using STRINGS;
using UnityEngine;

public class VitalsTableScreen : TableScreen
{
	protected override void OnActivate()
	{
		has_default_duplicant_row = false;
		title = UI.VITALS;
		base.OnActivate();
		AddPortraitColumn("Portrait", base.on_load_portrait, null);
		AddButtonLabelColumn("Names", base.on_load_name_label, base.get_value_name_label, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, base.compare_rows_alphabetical, on_tooltip_name, base.on_tooltip_sort_alphabetically);
		AddLabelColumn("Stress", on_load_stress, get_value_stress_label, compare_rows_stress, on_tooltip_stress, on_tooltip_sort_stress, 64, should_refresh_columns: true);
		AddLabelColumn("QOLExpectations", on_load_qualityoflife_expectations, get_value_qualityoflife_expectations_label, compare_rows_qualityoflife_expectations, on_tooltip_qualityoflife_expectations, on_tooltip_sort_qualityoflife_expectations, 128, should_refresh_columns: true);
		AddLabelColumn("Fullness", on_load_fullness, get_value_fullness_label, compare_rows_fullness, on_tooltip_fullness, on_tooltip_sort_fullness, 96, should_refresh_columns: true);
		AddLabelColumn("EatenToday", on_load_eaten_today, get_value_eaten_today_label, compare_rows_eaten_today, on_tooltip_eaten_today, on_tooltip_sort_eaten_today, 96, should_refresh_columns: true);
		AddLabelColumn("Health", on_load_health, get_value_health_label, compare_rows_health, on_tooltip_health, on_tooltip_sort_health, 64, should_refresh_columns: true);
		AddLabelColumn("Immunity", on_load_sickness, get_value_sickness_label, compare_rows_sicknesses, on_tooltip_sicknesses, on_tooltip_sort_sicknesses, 192, should_refresh_columns: true);
	}

	private void on_load_stress(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = (widgetRow.isDefault ? "" : UI.VITALSSCREEN.STRESS.ToString());
		}
	}

	private string get_value_stress_label(IAssignableIdentity identity, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion)
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if (minionIdentity != null)
			{
				return Db.Get().Amounts.Stress.Lookup(minionIdentity).GetValueString();
			}
		}
		else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			return UI.TABLESCREENS.NA;
		}
		return "";
	}

	private int compare_rows_stress(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if (minionIdentity == null && minionIdentity2 == null)
		{
			return 0;
		}
		if (minionIdentity == null)
		{
			return -1;
		}
		if (minionIdentity2 == null)
		{
			return 1;
		}
		float value = Db.Get().Amounts.Stress.Lookup(minionIdentity).value;
		float value2 = Db.Get().Amounts.Stress.Lookup(minionIdentity2).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_stress(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = minion as MinionIdentity;
			if (minionIdentity != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.Stress.Lookup(minionIdentity).GetTooltip(), null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(minion, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_stress(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_STRESS, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void on_load_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = (widgetRow.isDefault ? "" : UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString());
		}
	}

	private string get_value_qualityoflife_expectations_label(IAssignableIdentity identity, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion)
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if (minionIdentity != null)
			{
				return Db.Get().Attributes.QualityOfLife.Lookup(minionIdentity).GetFormattedValue();
			}
		}
		else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			return UI.TABLESCREENS.NA;
		}
		return "";
	}

	private int compare_rows_qualityoflife_expectations(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if (minionIdentity == null && minionIdentity2 == null)
		{
			return 0;
		}
		if (minionIdentity == null)
		{
			return -1;
		}
		if (minionIdentity2 == null)
		{
			return 1;
		}
		float totalValue = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minionIdentity).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minionIdentity2).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_qualityoflife_expectations(IAssignableIdentity identity, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if (minionIdentity != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup(minionIdentity).GetAttributeValueTooltip(), null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(identity, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void on_load_health(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			return;
		}
		object text3;
		if (!widgetRow.isDefault)
		{
			string text2 = (componentInChildren.text = UI.VITALSSCREEN_HEALTH.ToString());
			text3 = text2;
		}
		else
		{
			text3 = "";
		}
		componentInChildren.text = (string)text3;
	}

	private string get_value_health_label(IAssignableIdentity minion, GameObject widget_go)
	{
		if (minion != null)
		{
			TableRow widgetRow = GetWidgetRow(widget_go);
			if (widgetRow.rowType == TableRow.RowType.Minion && minion as MinionIdentity != null)
			{
				return Db.Get().Amounts.HitPoints.Lookup(minion as MinionIdentity).GetValueString();
			}
			if (widgetRow.rowType == TableRow.RowType.StoredMinon)
			{
				return UI.TABLESCREENS.NA;
			}
		}
		return "";
	}

	private int compare_rows_health(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if (minionIdentity == null && minionIdentity2 == null)
		{
			return 0;
		}
		if (minionIdentity == null)
		{
			return -1;
		}
		if (minionIdentity2 == null)
		{
			return 1;
		}
		float value = Db.Get().Amounts.HitPoints.Lookup(minionIdentity).value;
		float value2 = Db.Get().Amounts.HitPoints.Lookup(minionIdentity2).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_health(IAssignableIdentity identity, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if (minionIdentity != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.HitPoints.Lookup(minionIdentity).GetTooltip(), null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(identity, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_health(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_HITPOINTS, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void on_load_sickness(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = (widgetRow.isDefault ? "" : UI.VITALSSCREEN_SICKNESS.ToString());
		}
	}

	private string get_value_sickness_label(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion)
		{
			MinionIdentity minionIdentity = minion as MinionIdentity;
			if (minionIdentity != null)
			{
				Sicknesses sicknesses = minionIdentity.GetComponent<MinionModifiers>().sicknesses;
				if (sicknesses.IsInfected())
				{
					string text = "";
					if (sicknesses.Count > 1)
					{
						float seconds = 0f;
						foreach (SicknessInstance item in sicknesses)
						{
							seconds = Mathf.Min(item.GetInfectedTimeRemaining());
						}
						return text + string.Format(UI.VITALSSCREEN.MULTIPLE_SICKNESSES, GameUtil.GetFormattedCycles(seconds));
					}
					{
						foreach (SicknessInstance item2 in sicknesses)
						{
							if (!string.IsNullOrEmpty(text))
							{
								text += "\n";
							}
							text += string.Format(UI.VITALSSCREEN.SICKNESS_REMAINING, item2.modifier.Name, GameUtil.GetFormattedCycles(item2.GetInfectedTimeRemaining()));
						}
						return text;
					}
				}
				return UI.VITALSSCREEN.NO_SICKNESSES;
			}
		}
		else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			return UI.TABLESCREENS.NA;
		}
		return "";
	}

	private int compare_rows_sicknesses(IAssignableIdentity a, IAssignableIdentity b)
	{
		float value = 0f;
		return 0f.CompareTo(value);
	}

	protected void on_tooltip_sicknesses(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = minion as MinionIdentity;
			if (!(minionIdentity != null))
			{
				break;
			}
			Sicknesses sicknesses = minionIdentity.GetComponent<MinionModifiers>().sicknesses;
			if (sicknesses.IsInfected())
			{
				foreach (SicknessInstance item in sicknesses)
				{
					tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, null);
					tooltip.AddMultiStringTooltip(item.modifier.Name, null);
					StatusItem statusItem = item.GetStatusItem();
					tooltip.AddMultiStringTooltip(statusItem.GetTooltip(item.ExposureInfo), null);
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.VITALSSCREEN.NO_SICKNESSES, null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(minion, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_sicknesses(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_SICKNESSES, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void on_load_fullness(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = (widgetRow.isDefault ? "" : UI.VITALSSCREEN_CALORIES.ToString());
		}
	}

	private string get_value_fullness_label(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion && minion as MinionIdentity != null)
		{
			return Db.Get().Amounts.Calories.Lookup(minion as MinionIdentity).GetValueString();
		}
		if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			return UI.TABLESCREENS.NA;
		}
		return "";
	}

	private int compare_rows_fullness(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if (minionIdentity == null && minionIdentity2 == null)
		{
			return 0;
		}
		if (minionIdentity == null)
		{
			return -1;
		}
		if (minionIdentity2 == null)
		{
			return 1;
		}
		float value = Db.Get().Amounts.Calories.Lookup(minionIdentity).value;
		float value2 = Db.Get().Amounts.Calories.Lookup(minionIdentity2).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_fullness(IAssignableIdentity identity, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if (minionIdentity != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.Calories.Lookup(minionIdentity).GetTooltip(), null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(identity, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_fullness(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_FULLNESS, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), null);
			}
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void on_load_eaten_today(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(includeInactive: true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = (widgetRow.isDefault ? "" : UI.VITALSSCREEN_EATENTODAY.ToString());
		}
	}

	private static float RationsEatenToday(MinionIdentity minion)
	{
		float result = 0f;
		if (minion != null)
		{
			RationMonitor.Instance sMI = minion.GetSMI<RationMonitor.Instance>();
			if (sMI != null)
			{
				result = sMI.GetRationsAteToday();
			}
		}
		return result;
	}

	private string get_value_eaten_today_label(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion)
		{
			return GameUtil.GetFormattedCalories(RationsEatenToday(minion as MinionIdentity));
		}
		if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			return UI.TABLESCREENS.NA;
		}
		return "";
	}

	private int compare_rows_eaten_today(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if (minionIdentity == null && minionIdentity2 == null)
		{
			return 0;
		}
		if (minionIdentity == null)
		{
			return -1;
		}
		if (minionIdentity2 == null)
		{
			return 1;
		}
		float value = RationsEatenToday(minionIdentity);
		return RationsEatenToday(minionIdentity2).CompareTo(value);
	}

	protected void on_tooltip_eaten_today(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				float calories = RationsEatenToday(minion as MinionIdentity);
				tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.EATEN_TODAY_TOOLTIP, GameUtil.GetFormattedCalories(calories)), null);
			}
			break;
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(minion, tooltip);
			break;
		case TableRow.RowType.Header:
		case TableRow.RowType.Default:
			break;
		}
	}

	protected void on_tooltip_sort_eaten_today(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		switch (GetWidgetRow(widget_go).rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EATEN_TODAY, null);
			break;
		case TableRow.RowType.Default:
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		}
	}

	private void StoredMinionTooltip(IAssignableIdentity minion, ToolTip tooltip)
	{
		if (minion != null && minion as StoredMinionIdentity != null)
		{
			tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (minion as StoredMinionIdentity).GetStorageReason(), minion.GetProperName()), null);
		}
	}
}
