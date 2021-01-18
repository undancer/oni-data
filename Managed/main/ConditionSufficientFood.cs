using STRINGS;

public class ConditionSufficientFood : ProcessCondition
{
	private CommandModule module;

	public ConditionSufficientFood(CommandModule module)
	{
		this.module = module;
	}

	public override Status EvaluateCondition()
	{
		return (!(module.storage.GetAmountAvailable(GameTags.Edible) > 1f)) ? Status.Failure : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.HASFOOD.NAME;
		}
		return UI.STARMAP.NOFOOD.NAME;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return UI.STARMAP.HASFOOD.TOOLTIP;
		}
		return UI.STARMAP.NOFOOD.TOOLTIP;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
