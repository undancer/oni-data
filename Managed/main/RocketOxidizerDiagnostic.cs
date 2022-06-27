using STRINGS;

public class RocketOxidizerDiagnostic : ColonyDiagnostic
{
	public RocketOxidizerDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<RocketOxidizerTracker>(worldID);
		icon = "rocket_oxidizer";
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override DiagnosticResult Evaluate()
	{
		Clustercraft component = ClusterManager.Instance.GetWorld(base.worldID).gameObject.GetComponent<Clustercraft>();
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
		{
			return result;
		}
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.NORMAL;
		RocketEngineCluster engine = component.ModuleInterface.GetEngine();
		if (component.ModuleInterface.OxidizerPowerRemaining == 0f && engine != null && engine.requireOxidizer)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.WARNING;
		}
		return result;
	}
}
