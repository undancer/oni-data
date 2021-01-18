using STRINGS;
using UnityEngine;

public class TransferCargoCompleteCondition : ProcessCondition
{
	private GameObject target;

	public TransferCargoCompleteCondition(GameObject target)
	{
		this.target = target;
	}

	public override Status EvaluateCondition()
	{
		return (!target.HasTag(GameTags.TransferringCargoComplete)) ? Status.Warning : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.READY, 
			Status.Warning => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.WARNING, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.FAILURE, 
			_ => "", 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.READY, 
			Status.Warning => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.WARNING, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.FAILURE, 
			_ => "", 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
