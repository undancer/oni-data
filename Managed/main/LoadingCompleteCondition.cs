using STRINGS;

public class LoadingCompleteCondition : ProcessCondition
{
	private Storage target;

	public LoadingCompleteCondition(Storage target)
	{
		this.target = target;
	}

	public override Status EvaluateCondition()
	{
		return (!target.IsFull()) ? Status.Warning : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
