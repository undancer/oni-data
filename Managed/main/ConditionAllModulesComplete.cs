using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ConditionAllModulesComplete : ProcessCondition
{
	private LaunchableRocket launchable;

	public ConditionAllModulesComplete(LaunchableRocket launchable)
	{
		this.launchable = launchable;
	}

	public override Status EvaluateCondition()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchable.GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			if (item.GetComponent<Constructable>() != null || item.GetComponent<Building>().Def.PrefabID == "UnconstructedRocketModule")
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
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.CONSTRUCTION_COMPLETE.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
