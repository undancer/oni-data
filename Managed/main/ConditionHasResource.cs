using STRINGS;

public class ConditionHasResource : ProcessCondition
{
	private Storage storage;

	private SimHashes resource;

	private float thresholdMass;

	public ConditionHasResource(Storage storage, SimHashes resource, float thresholdMass)
	{
		this.storage = storage;
		this.resource = resource;
		this.thresholdMass = thresholdMass;
	}

	public override Status EvaluateCondition()
	{
		if (!(storage.GetAmountAvailable(resource.CreateTag()) >= thresholdMass))
		{
			return Status.Warning;
		}
		return Status.Ready;
	}

	public override string GetStatusMessage(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.READY, storage.GetProperName(), ElementLoader.GetElement(resource.CreateTag()).name), 
			Status.Failure => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.FAILURE, storage.GetProperName(), ElementLoader.GetElement(resource.CreateTag()).name), 
			_ => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.WARNING, storage.GetProperName(), ElementLoader.GetElement(resource.CreateTag()).name), 
		};
	}

	public override string GetStatusTooltip(Status status)
	{
		string text = "";
		return status switch
		{
			Status.Ready => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.READY, storage.GetProperName(), ElementLoader.GetElement(resource.CreateTag()).name), 
			Status.Failure => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.FAILURE, storage.GetProperName(), GameUtil.GetFormattedMass(thresholdMass), ElementLoader.GetElement(resource.CreateTag()).name), 
			_ => string.Format(UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.WARNING, storage.GetProperName(), GameUtil.GetFormattedMass(thresholdMass), ElementLoader.GetElement(resource.CreateTag()).name), 
		};
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
