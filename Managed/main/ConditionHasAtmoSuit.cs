using STRINGS;

public class ConditionHasAtmoSuit : RocketLaunchCondition
{
	private CommandModule module;

	public ConditionHasAtmoSuit(CommandModule module)
	{
		this.module = module;
		ManualDeliveryKG manualDeliveryKG = this.module.FindOrAdd<ManualDeliveryKG>();
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.SetStorage(module.storage);
		manualDeliveryKG.requestedItemTag = GameTags.AtmoSuit;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.refillMass = 0.1f;
		manualDeliveryKG.capacity = 1f;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override LaunchStatus EvaluateLaunchCondition()
	{
		if (!(module.storage.GetAmountAvailable(GameTags.AtmoSuit) >= 1f))
		{
			return LaunchStatus.Failure;
		}
		return LaunchStatus.Ready;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASSUIT.NAME;
		}
		return UI.STARMAP.NOSUIT.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASSUIT.TOOLTIP;
		}
		return UI.STARMAP.NOSUIT.TOOLTIP;
	}
}
