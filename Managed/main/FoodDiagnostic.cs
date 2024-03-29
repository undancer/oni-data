using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FoodDiagnostic : ColonyDiagnostic
{
	public FoodDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<KCalTracker>(worldID);
		icon = "icon_category_food";
		trackerSampleCountSeconds = 150f;
		presentationSetting = PresentationSetting.CurrentValue;
		AddCriterion("CheckEnoughFood", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKENOUGHFOOD, CheckEnoughFood));
		AddCriterion("CheckStarvation", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKSTARVATION, CheckStarvation));
	}

	private DiagnosticResult CheckAnyFood()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.PASS);
		if (Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count != 0)
		{
			if (tracker.GetDataTimeLength() < 10f)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
			}
			else if (tracker.GetAverageValue(trackerSampleCountSeconds) == 0f)
			{
				result.opinion = DiagnosticResult.Opinion.Bad;
				result.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.FAIL;
			}
		}
		return result;
	}

	private DiagnosticResult CheckEnoughFood()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		if (tracker.GetDataTimeLength() < 10f)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
		}
		else
		{
			int num = 3000;
			if ((float)worldItems.Count * (1000f * (float)num) > tracker.GetAverageValue(trackerSampleCountSeconds))
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				float currentValue = tracker.GetCurrentValue();
				float f = (float)Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count * -1000000f;
				string formattedCalories = GameUtil.GetFormattedCalories(currentValue);
				string formattedCalories2 = GameUtil.GetFormattedCalories(Mathf.Abs(f));
				string text = MISC.NOTIFICATIONS.FOODLOW.TOOLTIP;
				text = text.Replace("{0}", formattedCalories);
				text = (result.Message = text.Replace("{1}", formattedCalories2));
			}
		}
		return result;
	}

	private DiagnosticResult CheckStarvation()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(base.worldID))
		{
			if (!worldItem.IsNull())
			{
				CalorieMonitor.Instance sMI = worldItem.GetSMI<CalorieMonitor.Instance>();
				if (!sMI.IsNullOrStopped() && sMI.IsInsideState(sMI.sm.hungry.starving))
				{
					result.opinion = DiagnosticResult.Opinion.Bad;
					result.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.HUNGRY;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(sMI.gameObject.transform.position, sMI.gameObject);
				}
			}
		}
		return result;
	}

	public override string GetCurrentValueString()
	{
		return GameUtil.GetFormattedCalories(tracker.GetCurrentValue());
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
		{
			return result;
		}
		return base.Evaluate();
	}
}
