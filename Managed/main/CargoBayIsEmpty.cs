using STRINGS;
using UnityEngine;

public class CargoBayIsEmpty : ProcessCondition
{
	private CommandModule commandModule;

	public CargoBayIsEmpty(CommandModule module)
	{
		commandModule = module;
	}

	public override Status EvaluateCondition()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if (component != null && component.storage.MassStored() != 0f)
			{
				return Status.Failure;
			}
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		return UI.STARMAP.CARGOEMPTY.NAME;
	}

	public override string GetStatusTooltip(Status status)
	{
		return UI.STARMAP.CARGOEMPTY.TOOLTIP;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
