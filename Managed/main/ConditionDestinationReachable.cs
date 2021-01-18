using STRINGS;

public class ConditionDestinationReachable : ProcessCondition
{
	private LaunchableRocket.RegisterType craftRegisterType;

	private RocketModule module;

	public ConditionDestinationReachable(RocketModule module)
	{
		this.module = module;
		craftRegisterType = module.GetComponent<LaunchableRocket>().registerType;
	}

	public override Status EvaluateCondition()
	{
		Status result = Status.Failure;
		switch (craftRegisterType)
		{
		case LaunchableRocket.RegisterType.Spacecraft:
		{
			int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(module.GetComponent<LaunchConditionManager>()).id;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
			if (spacecraftDestination != null && CanReachSpacecraftDestination(spacecraftDestination) && spacecraftDestination.GetDestinationType().visitable)
			{
				result = Status.Ready;
			}
			break;
		}
		case LaunchableRocket.RegisterType.Clustercraft:
		{
			CraftModuleInterface craftInterface = module.CraftInterface;
			RocketClusterDestinationSelector component = craftInterface.GetComponent<RocketClusterDestinationSelector>();
			if (!component.IsAtDestination())
			{
				result = Status.Ready;
			}
			break;
		}
		}
		return result;
	}

	public bool CanReachSpacecraftDestination(SpaceDestination destination)
	{
		Debug.Assert(!DlcManager.IsExpansion1Active());
		float rocketMaxDistance = module.GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance();
		return (float)destination.OneBasedDistance * 10000f <= rocketMaxDistance;
	}

	public SpaceDestination GetSpacecraftDestination()
	{
		Debug.Assert(!DlcManager.IsExpansion1Active());
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(module.GetComponent<LaunchConditionManager>()).id;
		return SpacecraftManager.instance.GetSpacecraftDestination(id);
	}

	public override string GetStatusMessage(Status status)
	{
		string result = "";
		switch (craftRegisterType)
		{
		case LaunchableRocket.RegisterType.Spacecraft:
			result = ((status != 0 || GetSpacecraftDestination() == null) ? ((GetSpacecraftDestination() == null) ? ((string)UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED) : ((string)UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE)) : ((string)UI.STARMAP.DESTINATIONSELECTION.REACHABLE));
			break;
		case LaunchableRocket.RegisterType.Clustercraft:
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
		case LaunchableRocket.RegisterType.Spacecraft:
			result = ((status != 0 || GetSpacecraftDestination() == null) ? ((GetSpacecraftDestination() == null) ? ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED) : ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE)) : ((string)UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE));
			break;
		case LaunchableRocket.RegisterType.Clustercraft:
			result = UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
			break;
		}
		return result;
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
