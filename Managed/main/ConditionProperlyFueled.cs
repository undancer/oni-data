using STRINGS;

public class ConditionProperlyFueled : ProcessCondition
{
	private IFuelTank fuelTank;

	public ConditionProperlyFueled(IFuelTank fuelTank)
	{
		this.fuelTank = fuelTank;
	}

	public override Status EvaluateCondition()
	{
		RocketModuleCluster component = ((KMonoBehaviour)fuelTank).GetComponent<RocketModuleCluster>();
		if (component != null && component.CraftInterface != null)
		{
			Clustercraft component2 = component.CraftInterface.GetComponent<Clustercraft>();
			ClusterTraveler component3 = component.CraftInterface.GetComponent<ClusterTraveler>();
			if (component2 == null || component3 == null || component3.CurrentPath == null)
			{
				return Status.Failure;
			}
			int num = component3.RemainingTravelNodes();
			if (num == 0)
			{
				return component2.HasResourcesToMove(1, Clustercraft.CombustionResource.Fuel) ? Status.Ready : Status.Failure;
			}
			bool flag = component2.HasResourcesToMove(num * 2, Clustercraft.CombustionResource.Fuel);
			bool flag2 = component2.HasResourcesToMove(num, Clustercraft.CombustionResource.Fuel);
			if (flag)
			{
				return Status.Ready;
			}
			if (flag2)
			{
				return Status.Warning;
			}
		}
		return Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		Clustercraft component = ((KMonoBehaviour)fuelTank).GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
		string text = "";
		switch (status)
		{
		case Status.Ready:
			if (component.Destination == component.Location)
			{
				return UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.READY_NO_DESTINATION;
			}
			return UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.READY;
		case Status.Failure:
			return UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.FAILURE;
		default:
			return UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.WARNING;
		}
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
