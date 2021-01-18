using STRINGS;

public class ConditionPassengersOnBoard : ProcessCondition
{
	private PassengerRocketModule module;

	public ConditionPassengersOnBoard(PassengerRocketModule module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		return (!module.CheckPassengersBoarded()) ? Status.Failure : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
