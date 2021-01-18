using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ConditionHasEngine : ProcessCondition
{
	private LaunchableRocket launchable;

	public ConditionHasEngine(LaunchableRocket launchable)
	{
		this.launchable = launchable;
	}

	public override Status EvaluateCondition()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchable.GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (item.GetComponent<RocketEngine>() != null)
			{
				return Status.Ready;
			}
		}
		return Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_ENGINE.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
