using STRINGS;

public class FloatingRocketDiagnostic : ColonyDiagnostic
{
	public FloatingRocketDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.ALL_NAME)
	{
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
		if (world.ParentWorldId == ClusterManager.INVALID_WORLD_IDX || world.ParentWorldId == world.id)
		{
			result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_FLIGHT;
			if (component.Destination == component.Location)
			{
				result.opinion = DiagnosticResult.Opinion.Suggestion;
				result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_DESTINATION;
			}
			else if (component.Speed == 0f)
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_SPEED;
			}
		}
		else
		{
			result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_LANDED;
		}
		return result;
	}
}
