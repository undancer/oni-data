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
		Tuple<int, int> crewBoardedFraction = module.GetCrewBoardedFraction();
		if (crewBoardedFraction.first != crewBoardedFraction.second)
		{
			return Status.Failure;
		}
		return Status.Ready;
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
		Tuple<int, int> crewBoardedFraction = module.GetCrewBoardedFraction();
		if (status == Status.Ready)
		{
			if (crewBoardedFraction.second != 0)
			{
				return string.Format(UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.READY, crewBoardedFraction.first, crewBoardedFraction.second);
			}
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.NONE, crewBoardedFraction.first, crewBoardedFraction.second);
		}
		if (crewBoardedFraction.first == 0)
		{
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.FAILURE, crewBoardedFraction.first, crewBoardedFraction.second);
		}
		return string.Format(UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.WARNING, crewBoardedFraction.first, crewBoardedFraction.second);
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
