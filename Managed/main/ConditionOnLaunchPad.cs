using STRINGS;

public class ConditionOnLaunchPad : ProcessCondition
{
	private CraftModuleInterface craftInterface;

	public ConditionOnLaunchPad(CraftModuleInterface craftInterface)
	{
		this.craftInterface = craftInterface;
	}

	public override Status EvaluateCondition()
	{
		if (!(craftInterface.CurrentPad != null))
		{
			return Status.Failure;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.ON_LAUNCHPAD.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
