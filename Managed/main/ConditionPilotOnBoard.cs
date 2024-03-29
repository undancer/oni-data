using STRINGS;

public class ConditionPilotOnBoard : ProcessCondition
{
	private PassengerRocketModule module;

	public ConditionPilotOnBoard(PassengerRocketModule module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		if (!module.CheckPilotBoarded())
		{
			return Status.Failure;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
