using STRINGS;

public class ConditionDestinationReachable : ProcessCondition
{
	private LaunchableRocketRegisterType craftRegisterType;

	private RocketModule module;

	public ConditionDestinationReachable(RocketModule module)
	{
		this.module = module;
		craftRegisterType = module.GetComponent<ILaunchableRocket>().registerType;
	}

	public override Status EvaluateCondition()
	{
		Status result = Status.Failure;
		switch (craftRegisterType)
		{
		case LaunchableRocketRegisterType.Spacecraft:
		{
			int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(module.GetComponent<LaunchConditionManager>()).id;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
			if (spacecraftDestination != null && CanReachSpacecraftDestination(spacecraftDestination) && spacecraftDestination.GetDestinationType().visitable)
			{
				result = Status.Ready;
			}
			break;
		}
		case LaunchableRocketRegisterType.Clustercraft:
			if (!module.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<RocketClusterDestinationSelector>().IsAtDestination())
			{
				result = Status.Ready;
			}
			break;
		}
		return result;
	}

	public bool CanReachSpacecraftDestination(SpaceDestination destination)
	{
		Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
		float rocketMaxDistance = module.GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance();
		return (float)destination.OneBasedDistance * 10000f <= rocketMaxDistance;
	}

	public SpaceDestination GetSpacecraftDestination()
	{
		Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(module.GetComponent<LaunchConditionManager>()).id;
		return SpacecraftManager.instance.GetSpacecraftDestination(id);
	}

	public override string GetStatusMessage(Status status)
	{
		string result = "";
		switch (craftRegisterType)
		{
		case LaunchableRocketRegisterType.Spacecraft:
			result = ((status == Status.Ready && GetSpacecraftDestination() != null) ? ((string)UI.STARMAP.DESTINATIONSELECTION.REACHABLE) : ((GetSpacecraftDestination() == null) ? ((string)UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED) : ((string)UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE)));
			break;
		case LaunchableRocketRegisterType.Clustercraft:
			result = UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
			break;
		}
		return result;
	}

	public override string GetStatusTooltip(Status status)
	{
		string result = "";
		switch (craftRegisterType)
		{
		case LaunchableRocketRegisterType.Spacecraft:
			result = ((status == Status.Ready && GetSpacecraftDestination() != null) ? ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE) : ((GetSpacecraftDestination() == null) ? ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED) : ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE)));
			break;
		case LaunchableRocketRegisterType.Clustercraft:
			result = ((status != Status.Ready) ? ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED) : ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE));
			break;
		}
		return result;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
