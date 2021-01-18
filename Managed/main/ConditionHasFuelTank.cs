using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ConditionHasFuelTank : ProcessCondition
{
	private LaunchableRocket launchable;

	public ConditionHasFuelTank(LaunchableRocket launchable)
	{
		this.launchable = launchable;
	}

	public override Status EvaluateCondition()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchable.GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (item.GetComponent<FuelTank>() != null)
			{
				return Status.Failure;
			}
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_FUEL_TANK.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
