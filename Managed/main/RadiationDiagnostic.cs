using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RadiationDiagnostic : ColonyDiagnostic
{
	public RadiationDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<RadiationTracker>(worldID);
		trackerSampleCountSeconds = 150f;
		presentationSetting = PresentationSetting.CurrentValue;
		icon = "overlay_radiation";
		AddCriterion("CheckSick", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA.CHECKSICK, CheckSick));
		AddCriterion("CheckExposed", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA.CHECKEXPOSED, CheckExposure));
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override string GetCurrentValueString()
	{
		return string.Format(UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.AVERAGE_RADS, GameUtil.GetFormattedRads(TrackerTool.Instance.GetWorldTracker<RadiationTracker>(base.worldID).GetCurrentValue()));
	}

	public override string GetAverageValueString()
	{
		return string.Format(UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.AVERAGE_RADS, GameUtil.GetFormattedRads(TrackerTool.Instance.GetWorldTracker<RadiationTracker>(base.worldID).GetCurrentValue()));
	}

	private DiagnosticResult CheckSick()
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
		result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.NORMAL;
		foreach (MinionIdentity item in worldItems)
		{
			RadiationMonitor.Instance sMI = item.GetSMI<RadiationMonitor.Instance>();
			if (sMI != null && sMI.sm.isSick.Get(sMI))
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_SICKNESS.FAIL;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
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
			return result;
		}
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.PASS;
		foreach (MinionIdentity item in worldItems)
		{
			RadiationMonitor.Instance sMI = item.GetSMI<RadiationMonitor.Instance>();
			if (sMI != null)
			{
				RadiationMonitor sm = sMI.sm;
				GameObject gameObject = item.gameObject;
				Vector3 position = gameObject.transform.position;
				float p = sm.currentExposurePerCycle.Get(sMI);
				float p2 = sm.radiationExposure.Get(sMI);
				if (RadiationMonitor.COMPARE_LT_MINOR(sMI, p) && RadiationMonitor.COMPARE_RECOVERY_IMMEDIATE(sMI, p2))
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(position, gameObject);
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.FAIL_CONCERN;
				}
				if (RadiationMonitor.COMPARE_GTE_DEADLY(sMI, p))
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(position, item.gameObject);
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.RADIATIONDIAGNOSTIC.CRITERIA_RADIATION_EXPOSURE.FAIL_WARNING;
				}
			}
		}
		return result;
	}
}
