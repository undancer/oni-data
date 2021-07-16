using STRINGS;

public class ConditionRocketHeight : ProcessCondition
{
	private RocketEngineCluster engine;

	public ConditionRocketHeight(RocketEngineCluster engine)
	{
		this.engine = engine;
	}

	public override Status EvaluateCondition()
	{
		if (engine.maxHeight < engine.GetComponent<RocketModuleCluster>().CraftInterface.RocketHeight)
		{
			return Status.Failure;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.MAX_HEIGHT.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
