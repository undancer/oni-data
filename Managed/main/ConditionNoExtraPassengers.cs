using STRINGS;

public class ConditionNoExtraPassengers : ProcessCondition
{
	private PassengerRocketModule module;

	public ConditionNoExtraPassengers(PassengerRocketModule module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		return (!module.CheckExtraPassengers()) ? Status.Ready : Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}