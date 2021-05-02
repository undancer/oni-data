using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RadiationDiagnostic : ColonyDiagnostic
{
	public RadiationDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.ALL_NAME)
	{
		icon = "overlay_radiation";
		AddCriterion("CheckSick", new DiagnosticCriterion(CheckSick));
		AddCriterion("CheckExposed", new DiagnosticCriterion(CheckExposure));
	}

	private DiagnosticResult CheckSick()
	{
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
			result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.NORMAL;
			foreach (MinionIdentity item in worldItems)
			{
				RadiationMonitor.Instance sMI = item.GetSMI<RadiationMonitor.Instance>();
				if (sMI.sm.isSick.Get(sMI))
				{
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_SICKNESS.FAIL;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
				}
			}
		}
		return result;
	}

	private DiagnosticResult CheckExposure()
	{
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
			result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.PASS;
			foreach (MinionIdentity item in worldItems)
			{
				RadiationMonitor.Instance sMI = item.GetSMI<RadiationMonitor.Instance>();
				if (sMI.sm.currentExposurePerCycle.Get(sMI) > 600f)
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.FAIL_WARNING;
				}
				else if (sMI.sm.currentExposurePerCycle.Get(sMI) > 60f)
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.FAIL_CONCERN;
				}
			}
		}
		return result;
	}
}
