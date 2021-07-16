using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BreathabilityDiagnostic : ColonyDiagnostic
{
	public BreathabilityDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<BreathabilityTracker>(worldID);
		trackerSampleCountSeconds = 50f;
		icon = "overlay_oxygen";
		AddCriterion("CheckSuffocation", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKSUFFOCATION, CheckSuffocation));
		AddCriterion("CheckLowBreathability", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.CRITERIA.CHECKLOWBREATHABILITY, CheckLowBreathability));
	}

	private DiagnosticResult CheckSuffocation()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		if (worldItems.Count != 0)
		{
			foreach (MinionIdentity item in worldItems)
			{
				item.GetComponent<OxygenBreather>().GetGasProvider();
				SuffocationMonitor.Instance sMI = item.GetSMI<SuffocationMonitor.Instance>();
				if (sMI != null && sMI.IsInsideState(sMI.sm.nooxygen.suffocating))
				{
					return new DiagnosticResult(DiagnosticResult.Opinion.DuplicantThreatening, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.SUFFOCATING, new Tuple<Vector3, GameObject>(sMI.transform.position, sMI.gameObject));
				}
			}
		}
		return new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
	}

	private DiagnosticResult CheckLowBreathability()
	{
		if (Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count != 0 && tracker.GetAverageValue(trackerSampleCountSeconds) < 60f)
		{
			return new DiagnosticResult(DiagnosticResult.Opinion.Concern, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.POOR);
		}
		return new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
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
