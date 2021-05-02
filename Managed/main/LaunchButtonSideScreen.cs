using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LaunchButtonSideScreen : SideScreenContent
{
	public KButton launchButton;

	public LocText statusText;

	private RocketModuleCluster rocketModule;

	private LaunchPad selectedPad;

	private bool acknowledgeWarnings = false;

	private float lastRefreshTime = 0f;

	private const float UPDATE_FREQUENCY = 1f;

	private static readonly EventSystem.IntraObjectHandler<LaunchButtonSideScreen> RefreshDelegate = new EventSystem.IntraObjectHandler<LaunchButtonSideScreen>(delegate(LaunchButtonSideScreen cmp, object data)
	{
		cmp.Refresh();
	});

	protected override void OnSpawn()
	{
		Refresh();
		launchButton.onClick += TriggerLaunch;
	}

	public override int GetSideScreenSortOrder()
	{
		return -100;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (target.GetComponent<RocketModule>() != null && target.HasTag(GameTags.LaunchButtonRocketModule)) || ((bool)target.GetComponent<LaunchPad>() && target.GetComponent<LaunchPad>().HasRocketWithCommandModule());
	}

	public override void SetTarget(GameObject target)
	{
		rocketModule = target.GetComponent<RocketModuleCluster>();
		if (rocketModule == null)
		{
			selectedPad = target.GetComponent<LaunchPad>();
			if (selectedPad != null)
			{
				CraftModuleInterface craftInterface = selectedPad.LandedRocket.CraftInterface;
				foreach (Ref<RocketModuleCluster> clusterModule in craftInterface.ClusterModules)
				{
					if ((bool)clusterModule.Get().GetComponent<LaunchableRocketCluster>())
					{
						rocketModule = clusterModule.Get().GetComponent<RocketModuleCluster>();
						break;
					}
				}
			}
		}
		if (selectedPad == null)
		{
			CraftModuleInterface craftInterface2 = rocketModule.CraftInterface;
			selectedPad = craftInterface2.currentPad.Get();
		}
		acknowledgeWarnings = false;
		rocketModule.CraftInterface.Subscribe(543433792, RefreshDelegate);
		rocketModule.CraftInterface.Subscribe(1655598572, RefreshDelegate);
		Refresh();
	}

	public override void ClearTarget()
	{
		if (rocketModule != null)
		{
			rocketModule.CraftInterface.Unsubscribe(543433792, RefreshDelegate);
			rocketModule.CraftInterface.Unsubscribe(1655598572, RefreshDelegate);
			rocketModule = null;
		}
	}

	private void TriggerLaunch()
	{
		bool flag = !acknowledgeWarnings && rocketModule.CraftInterface.HasLaunchWarnings();
		bool flag2 = rocketModule.CraftInterface.IsLaunchRequested();
		if (flag)
		{
			acknowledgeWarnings = true;
		}
		else if (flag2)
		{
			rocketModule.CraftInterface.CancelLaunch();
			acknowledgeWarnings = false;
		}
		else
		{
			rocketModule.CraftInterface.TriggerLaunch();
		}
		Refresh();
	}

	public void Update()
	{
		if (Time.unscaledTime > lastRefreshTime + 1f)
		{
			lastRefreshTime = Time.unscaledTime;
			Refresh();
		}
	}

	private void Refresh()
	{
		if (rocketModule == null || selectedPad == null)
		{
			return;
		}
		bool flag = !acknowledgeWarnings && rocketModule.CraftInterface.HasLaunchWarnings();
		bool flag2 = rocketModule.CraftInterface.IsLaunchRequested();
		bool flag3 = selectedPad.IsLogicInputConnected();
		bool flag4 = (flag3 ? rocketModule.CraftInterface.CheckReadyForAutomatedLaunchCommand() : rocketModule.CraftInterface.CheckPreppedForLaunch());
		if (flag3)
		{
			launchButton.isInteractable = false;
			launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_AUTOMATION_CONTROLLED;
			launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_AUTOMATION_CONTROLLED_TOOLTIP;
		}
		else if (DebugHandler.InstantBuildMode || flag4)
		{
			launchButton.isInteractable = true;
			if (flag2)
			{
				launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON_TOOLTIP;
			}
			else if (flag)
			{
				launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON_TOOLTIP;
			}
			else
			{
				LocString loc_string = (DebugHandler.InstantBuildMode ? UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_DEBUG : UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON);
				launchButton.GetComponentInChildren<LocText>().text = loc_string;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_TOOLTIP;
			}
		}
		else
		{
			launchButton.isInteractable = false;
			launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON;
			launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_NOT_READY_TOOLTIP;
		}
		if (rocketModule.CraftInterface.GetInteriorWorld() == null)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
			return;
		}
		PassengerRocketModule component = rocketModule.GetComponent<PassengerRocketModule>();
		List<RocketControlStation> worldItems = Components.RocketControlStations.GetWorldItems(rocketModule.CraftInterface.GetInteriorWorld().id);
		RocketControlStationLaunchWorkable rocketControlStationLaunchWorkable = null;
		if (worldItems != null && worldItems.Count > 0)
		{
			RocketControlStation rocketControlStation = worldItems[0];
			rocketControlStationLaunchWorkable = rocketControlStation.GetComponent<RocketControlStationLaunchWorkable>();
		}
		if (component == null || rocketControlStationLaunchWorkable == null)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
			return;
		}
		bool flag5 = component.CheckPassengersBoarded();
		bool flag6 = !component.CheckExtraPassengers();
		bool flag7 = rocketControlStationLaunchWorkable.worker != null;
		bool flag8 = rocketModule.CraftInterface.HasTag(GameTags.RocketNotOnGround);
		if (!flag4)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
		}
		else if (!flag2)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.READY_FOR_LAUNCH;
		}
		else if (!flag5)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.LOADING_CREW;
		}
		else if (!flag6)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.UNLOADING_PASSENGERS;
		}
		else if (!flag7)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.WAITING_FOR_PILOT;
		}
		else if (!flag8)
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.COUNTING_DOWN;
		}
		else
		{
			statusText.text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.TAKING_OFF;
		}
	}
}
