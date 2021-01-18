using STRINGS;

public class ConditionModuleCount : ProcessCondition
{
	private RocketEngine engine;

	public ConditionModuleCount(RocketEngine engine)
	{
		this.engine = engine;
	}

	public override Status EvaluateCondition()
	{
		return (engine.maxModules < engine.GetComponent<RocketModule>().CraftInterface.Modules.Count) ? Status.Failure : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.MAX_MODULES.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
