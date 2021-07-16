using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RequestCrewSideScreen : SideScreenContent
{
	private PassengerRocketModule rocketModule;

	public KToggle crewReleaseButton;

	public KToggle crewRequestButton;

	private Dictionary<KToggle, PassengerRocketModule.RequestCrewState> toggleMap = new Dictionary<KToggle, PassengerRocketModule.RequestCrewState>();

	public KButton changeCrewButton;

	public KScreen changeCrewSideScreenPrefab;

	private AssignmentGroupControllerSideScreen activeChangeCrewSideScreen;

	protected override void OnSpawn()
	{
		changeCrewButton.onClick += OnChangeCrewButtonPressed;
		crewReleaseButton.onClick += CrewRelease;
		crewRequestButton.onClick += CrewRequest;
		toggleMap.Add(crewReleaseButton, PassengerRocketModule.RequestCrewState.Release);
		toggleMap.Add(crewRequestButton, PassengerRocketModule.RequestCrewState.Request);
	}

	public override int GetSideScreenSortOrder()
	{
		return 100;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		PassengerRocketModule component = target.GetComponent<PassengerRocketModule>();
		RocketControlStation component2 = target.GetComponent<RocketControlStation>();
		if (component != null)
		{
			return component.GetMyWorld() != null;
		}
		if (component2 != null)
		{
			RocketControlStation.StatesInstance sMI = component2.GetSMI<RocketControlStation.StatesInstance>();
			if (!sMI.sm.IsInFlight(sMI))
			{
				return !sMI.sm.IsLaunching(sMI);
			}
			return false;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		if (target.GetComponent<RocketControlStation>() != null)
		{
			rocketModule = target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule();
		}
		else
		{
			rocketModule = target.GetComponent<PassengerRocketModule>();
		}
		Refresh();
	}

	private void Refresh()
	{
		RefreshRequestButtons();
	}

	private void CrewRelease()
	{
		rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Release);
		RefreshRequestButtons();
	}

	private void CrewRequest()
	{
		rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
		RefreshRequestButtons();
	}

	private void RefreshRequestButtons()
	{
		foreach (KeyValuePair<KToggle, PassengerRocketModule.RequestCrewState> item in toggleMap)
		{
			RefreshRequestButton(item.Key);
		}
	}

	private void RefreshRequestButton(KToggle button)
	{
		if (toggleMap[button] == rocketModule.PassengersRequested)
		{
			button.isOn = true;
			ImageToggleState[] componentsInChildren = button.GetComponentsInChildren<ImageToggleState>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetActive();
			}
			button.GetComponent<ImageToggleStateThrobber>().enabled = false;
		}
		else
		{
			button.isOn = false;
			ImageToggleState[] componentsInChildren = button.GetComponentsInChildren<ImageToggleState>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetInactive();
			}
			button.GetComponent<ImageToggleStateThrobber>().enabled = false;
		}
	}

	private void OnChangeCrewButtonPressed()
	{
		if (activeChangeCrewSideScreen == null)
		{
			activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeCrewSideScreenPrefab, UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TITLE);
			activeChangeCrewSideScreen.SetTarget(rocketModule.gameObject);
		}
		else
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
			activeChangeCrewSideScreen = null;
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
			activeChangeCrewSideScreen = null;
		}
	}
}
