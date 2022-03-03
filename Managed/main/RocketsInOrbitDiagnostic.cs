using STRINGS;

public class RocketsInOrbitDiagnostic : ColonyDiagnostic
{
	private int numRocketsInOrbit;

	public RocketsInOrbitDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_action_dig";
		AddCriterion("RocketsOrbiting", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.CRITERIA.CHECKORBIT, CheckOrbit));
	}

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public DiagnosticResult CheckOrbit()
	{
		AxialI myWorldLocation = ClusterManager.Instance.GetWorld(base.worldID).GetMyWorldLocation();
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		numRocketsInOrbit = 0;
		Clustercraft clustercraft = null;
		bool flag = false;
		foreach (Clustercraft item in Components.Clustercrafts.Items)
		{
			AxialI myWorldLocation2 = item.GetMyWorldLocation();
			AxialI destination = item.Destination;
			if (myWorldLocation2 != myWorldLocation && ClusterGrid.Instance.IsInRange(myWorldLocation2, myWorldLocation) && ClusterGrid.Instance.IsInRange(myWorldLocation, destination))
			{
				numRocketsInOrbit++;
				clustercraft = item;
				flag = flag || !item.CanLandAtAsteroid(myWorldLocation, mustLandImmediately: false);
			}
		}
		if (numRocketsInOrbit == 1 && clustercraft != null)
		{
			result.Message = string.Format(flag ? UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.WARNING_ONE_ROCKETS_STRANDED : UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_ONE_IN_ORBIT, clustercraft.Name);
		}
		else if (numRocketsInOrbit > 0)
		{
			result.Message = string.Format(flag ? UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.WARNING_ROCKETS_STRANDED : UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_IN_ORBIT, numRocketsInOrbit);
		}
		else
		{
			result.Message = UI.COLONY_DIAGNOSTICS.ROCKETINORBITDIAGNOSTIC.NORMAL_NO_ROCKETS;
		}
		if (flag)
		{
			result.opinion = DiagnosticResult.Opinion.Warning;
		}
		else if (numRocketsInOrbit > 0)
		{
			result.opinion = DiagnosticResult.Opinion.Suggestion;
		}
		return result;
	}
}
