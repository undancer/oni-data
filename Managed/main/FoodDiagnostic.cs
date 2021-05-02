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
		AddCriterion(UI.COLONY_DIAGNOSTICS.NO_MINIONS, ColonyDiagnosticUtility.GetWorldHasMinionCriterion(worldID));
		AddCriterion("CheckEnoughFood", new DiagnosticCriterion(CheckEnoughFood));
		AddCriterion("CheckStarvation", new DiagnosticCriterion(CheckStarvation));
	}

	private DiagnosticResult CheckAnyFood()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.PASS);
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
				result.Message = string.Format(MISC.NOTIFICATIONS.FOODLOW.TOOLTIP, GameUtil.GetFormattedCalories(currentValue), GameUtil.GetFormattedCalories(Mathf.Abs(f)));
			}
		}
		return result;
	}

	private DiagnosticResult CheckStarvation()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		foreach (MinionIdentity item in worldItems)
		{
			if (!item.IsNull())
			{
				CalorieMonitor.Instance sMI = item.GetSMI<CalorieMonitor.Instance>();
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
