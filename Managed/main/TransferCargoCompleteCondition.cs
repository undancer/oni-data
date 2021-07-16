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
		CraftModuleInterface craftModuleInterface = null;
		LaunchPad component = target.GetComponent<LaunchPad>();
		if (component == null)
		{
			craftModuleInterface = target.GetComponent<Clustercraft>().ModuleInterface;
		}
		else
		{
			RocketModuleCluster landedRocket = component.LandedRocket;
			if (landedRocket == null)
			{
				return Status.Ready;
			}
			craftModuleInterface = landedRocket.CraftInterface;
		}
		if (!craftModuleInterface.HasCargoModule)
		{
			return Status.Ready;
		}
		if (!target.HasTag(GameTags.TransferringCargoComplete))
		{
			return Status.Warning;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.WARNING;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.WARNING;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
