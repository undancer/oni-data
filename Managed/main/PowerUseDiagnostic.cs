using System.Collections.Generic;
using STRINGS;

public class PowerUseDiagnostic : ColonyDiagnostic
{
	public PowerUseDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<PowerUseTracker>(worldID);
		trackerSampleCountSeconds = 30f;
		icon = "overlay_power";
		AddCriterion("CheckPower", new DiagnosticCriterion(CheckPower));
	}

	private DiagnosticResult CheckPower()
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
			result.Message = UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
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
}
