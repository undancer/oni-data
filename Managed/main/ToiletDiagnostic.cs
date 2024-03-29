using System.Collections.Generic;
using STRINGS;

public class ToiletDiagnostic : ColonyDiagnostic
{
	public ToiletDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_action_region_toilet";
		tracker = TrackerTool.Instance.GetWorldTracker<WorkingToiletTracker>(worldID);
		AddCriterion("CheckHasAnyToilets", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKHASANYTOILETS, CheckHasAnyToilets));
		AddCriterion("CheckEnoughToilets", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKENOUGHTOILETS, CheckEnoughToilets));
		AddCriterion("CheckBladders", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKBLADDERS, CheckBladders));
	}

	private DiagnosticResult CheckHasAnyToilets()
	{
		List<IUsable> worldItems = Components.Toilets.GetWorldItems(base.worldID);
		List<MinionIdentity> worldItems2 = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems2.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
		}
		else if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_TOILETS;
		}
		return result;
	}

	private DiagnosticResult CheckEnoughToilets()
	{
		Components.Toilets.GetWorldItems(base.worldID);
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
		}
		else
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NORMAL;
			if (tracker.GetDataTimeLength() > 10f && tracker.GetAverageValue(trackerSampleCountSeconds) <= 0f)
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_WORKING_TOILETS;
			}
		}
		return result;
	}

	private DiagnosticResult CheckBladders()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			return result;
		}
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NORMAL;
		PeeChoreMonitor.Instance instance = null;
		foreach (MinionIdentity item in worldItems)
		{
			instance = item.GetSMI<PeeChoreMonitor.Instance>();
			if (instance != null && instance.IsCritical())
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.TOILET_URGENT;
				return result;
			}
		}
		return result;
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

	public override string GetAverageValueString()
	{
		List<IUsable> worldItems = Components.Toilets.GetWorldItems(base.worldID);
		int num = worldItems.Count;
		for (int i = 0; i < worldItems.Count; i++)
		{
			if (!worldItems[i].IsUsable())
			{
				num--;
			}
		}
		return num + ":" + Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count;
	}
}
