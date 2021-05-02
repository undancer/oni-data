using STRINGS;

public class LoadingCompleteCondition : ProcessCondition
{
	private Storage target;

	private IUserControlledCapacity userControlledTarget;

	public LoadingCompleteCondition(Storage target)
	{
		this.target = target;
		userControlledTarget = target.GetComponent<IUserControlledCapacity>();
	}

	public override Status EvaluateCondition()
	{
		if (userControlledTarget != null)
		{
			return (!(userControlledTarget.AmountStored >= userControlledTarget.UserMaxCapacity)) ? Status.Warning : Status.Ready;
		}
		return (!target.IsFull()) ? Status.Warning : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.WARNING;
	}

	public override string GetStatusTooltip(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.WARNING;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
