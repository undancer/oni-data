using STRINGS;

public class InternalConstructionCompleteCondition : ProcessCondition
{
	private BuildingInternalConstructor.Instance target;

	public InternalConstructionCompleteCondition(BuildingInternalConstructor.Instance target)
	{
		this.target = target;
	}

	public override Status EvaluateCondition()
	{
		return (target.IsRequestingConstruction() && !target.HasOutputInStorage()) ? Status.Warning : Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.STATUS.FAILURE;
	}

	public override string GetStatusTooltip(Status status)
	{
		return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
