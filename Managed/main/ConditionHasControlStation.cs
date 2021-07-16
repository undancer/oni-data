using STRINGS;

public class ConditionHasControlStation : ProcessCondition
{
	private RocketModuleCluster module;

	public ConditionHasControlStation(RocketModuleCluster module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		if (Components.RocketControlStations.GetWorldItems(module.CraftInterface.GetComponent<WorldContainer>().id).Count <= 0)
		{
			return Status.Failure;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.STATUS.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.STATUS.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return EvaluateCondition() == Status.Failure;
	}
}
