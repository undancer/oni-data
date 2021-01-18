using STRINGS;

public class ConditionSufficientFood : RocketLaunchCondition
{
	private CommandModule module;

	public ConditionSufficientFood(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override LaunchStatus EvaluateLaunchCondition()
	{
		if (!(module.storage.GetAmountAvailable(GameTags.Edible) > 1f))
		{
			return LaunchStatus.Failure;
		}
		return LaunchStatus.Ready;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASFOOD.NAME;
		}
		return UI.STARMAP.NOFOOD.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASFOOD.TOOLTIP;
		}
		return UI.STARMAP.NOFOOD.TOOLTIP;
	}
}
