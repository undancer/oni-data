using STRINGS;

public class RocketFuelDiagnostic : ColonyDiagnostic
{
	public RocketFuelDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<RocketFuelTracker>(worldID);
		icon = "icon_action_dig";
	}

	public override DiagnosticResult Evaluate()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(base.worldID);
		Clustercraft component = world.gameObject.GetComponent<Clustercraft>();
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
		{
			return result;
		}
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.NORMAL;
		if (component.ModuleInterface.FuelRemaining == 0f)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.WARNING;
		}
		return result;
	}
}
