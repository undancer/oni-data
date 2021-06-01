using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class StressDiagnostic : ColonyDiagnostic
{
	public StressDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(worldID);
		icon = "mod_stress";
		AddCriterion("CheckStressed", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckStressed));
	}

	private DiagnosticResult CheckStressed()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			string text = (TrackerTool.Instance.IsRocketInterior(base.worldID) ? UI.COLONY_DIAGNOSTICS.ROCKET : UI.CLUSTERMAP.PLANETOID_KEYWORD);
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
		}
		else
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.NORMAL;
			if (tracker.GetAverageValue(trackerSampleCountSeconds) >= STRESS.ACTING_OUT_RESET)
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.HIGH_STRESS;
				MinionIdentity minionIdentity = worldItems.Find((MinionIdentity match) => match.gameObject.GetAmounts().GetValue(Db.Get().Amounts.Stress.Id) >= STRESS.ACTING_OUT_RESET);
				if (minionIdentity != null)
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(minionIdentity.gameObject.transform.position, minionIdentity.gameObject);
				}
			}
		}
		return result;
	}
}
