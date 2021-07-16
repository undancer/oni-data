using System.Collections.Generic;
using STRINGS;

public class ConditionHasAstronaut : ProcessCondition
{
	private CommandModule module;

	public ConditionHasAstronaut(CommandModule module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		List<MinionStorage.Info> storedMinionInfo = module.GetComponent<MinionStorage>().GetStoredMinionInfo();
		if (storedMinionInfo.Count > 0 && storedMinionInfo[0].serializedMinion != null)
		{
			return Status.Ready;
		}
		return Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUT_TITLE;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.HASASTRONAUT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
