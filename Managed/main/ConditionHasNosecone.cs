using STRINGS;

public class ConditionHasNosecone : ProcessCondition
{
	private LaunchableRocketCluster launchable;

	public ConditionHasNosecone(LaunchableRocketCluster launchable)
	{
		this.launchable = launchable;
	}

	public override Status EvaluateCondition()
	{
		foreach (Ref<RocketModuleCluster> part in launchable.parts)
		{
			if (part.Get().HasTag(GameTags.NoseRocketModule))
			{
				return Status.Ready;
			}
		}
		return Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.STATUS.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.STATUS.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.STATUS.WARNING, 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.TOOLTIP.READY, 
			Status.Failure => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.TOOLTIP.FAILURE, 
			_ => UI.STARMAP.LAUNCHCHECKLIST.HAS_NOSECONE.TOOLTIP.WARNING, 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
