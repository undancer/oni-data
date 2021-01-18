using STRINGS;

public class ConditionProperlyFueled : ProcessCondition
{
	private FuelTank fuelTank;

	public ConditionProperlyFueled(FuelTank fuelTank)
	{
		this.fuelTank = fuelTank;
	}

	public override Status EvaluateCondition()
	{
		RocketModule component = fuelTank.GetComponent<RocketModule>();
		if (component != null && component.CraftInterface != null)
		{
			Clustercraft component2 = component.CraftInterface.GetComponent<Clustercraft>();
			if (component2 == null)
			{
				return Status.Failure;
			}
			if (component2.CurrentPath == null)
			{
				return Status.Failure;
			}
			int num = component2.RemainingTravelNodes();
			if (num == 0)
			{
				return Status.Ready;
			}
			bool flag = component2.HasResourcesToMove(num * 2);
			bool flag2 = component2.HasResourcesToMove(num);
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
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
