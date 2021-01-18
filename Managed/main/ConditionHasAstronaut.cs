using System.Collections.Generic;
using STRINGS;

public class ConditionHasAstronaut : RocketLaunchCondition
{
	private CommandModule module;

	public ConditionHasAstronaut(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override LaunchStatus EvaluateLaunchCondition()
	{
		List<MinionStorage.Info> storedMinionInfo = module.GetComponent<MinionStorage>().GetStoredMinionInfo();
		if (storedMinionInfo.Count > 0 && storedMinionInfo[0].serializedMinion != null)
		{
			return LaunchStatus.Ready;
		}
		return LaunchStatus.Failure;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUT_TITLE;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.HASASTRONAUT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}
}
