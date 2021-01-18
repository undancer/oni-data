using STRINGS;
using UnityEngine;

public class LaunchButtonSideScreen : SideScreenContent
{
	public KButton launchButton;

	private RocketModule rocketModule;

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
		return (target.GetComponent<RocketModule>() != null && target.HasTag(GameTags.NoseRocketModule)) || ((bool)target.GetComponent<LaunchPad>() && target.GetComponent<LaunchPad>().HasRocketWithCommandModule());
	}

	public override void SetTarget(GameObject target)
	{
		rocketModule = target.GetComponent<RocketModule>();
		if (rocketModule == null)
		{
			LaunchPad component = target.GetComponent<LaunchPad>();
			if (component != null)
			{
				CraftModuleInterface craftInterface = component.LandedRocket.CraftInterface;
				foreach (Ref<RocketModule> module in craftInterface.Modules)
				{
					if ((bool)module.Get().GetComponent<LaunchableRocket>())
					{
						rocketModule = module.Get().GetComponent<RocketModule>();
						break;
					}
				}
			}
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
		bool flag = DebugHandler.InstantBuildMode || rocketModule.CraftInterface.CheckReadyToLaunch();
		bool flag2 = !acknowledgeWarnings && rocketModule.CraftInterface.HasLaunchWarnings();
		bool flag3 = rocketModule.CraftInterface.IsLaunchRequested();
		if (flag)
		{
			launchButton.isInteractable = true;
			if (flag2)
			{
				launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON_TOOLTIP;
			}
			else if (flag3)
			{
				launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON_TOOLTIP;
			}
			else
			{
				launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON;
				launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_TOOLTIP;
			}
		}
		else
		{
			launchButton.isInteractable = false;
			launchButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON;
			launchButton.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_NOT_READY_TOOLTIP;
		}
	}
}
