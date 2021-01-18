using STRINGS;
using UnityEngine;

public class CargoBayIsEmpty : RocketLaunchCondition
{
	private CommandModule commandModule;

	public CargoBayIsEmpty(CommandModule module)
	{
		commandModule = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override LaunchStatus EvaluateLaunchCondition()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(commandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if (component != null && component.storage.MassStored() != 0f)
			{
				return LaunchStatus.Failure;
			}
		}
		return LaunchStatus.Ready;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		return UI.STARMAP.CARGOEMPTY.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		return UI.STARMAP.CARGOEMPTY.TOOLTIP;
	}
}
