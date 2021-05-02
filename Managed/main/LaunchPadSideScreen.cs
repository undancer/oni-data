using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LaunchPadSideScreen : SideScreenContent
{
	public GameObject content;

	private LaunchPad selectedPad;

	public LocText DescriptionText;

	public GameObject landableRocketRowPrefab;

	public GameObject newRocketPanel;

	public KButton startNewRocketbutton;

	public GameObject landableRowContainer;

	public GameObject nothingWaitingRow;

	public KScreen changeModuleSideScreen;

	private int refreshEventHandle = -1;

	public List<GameObject> waitingToLandRows = new List<GameObject>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		startNewRocketbutton.onClick += ClickStartNewRocket;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
		}
	}

	public override int GetSideScreenSortOrder()
	{
		return 100;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LaunchPad>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		if (refreshEventHandle != -1)
		{
			selectedPad.Unsubscribe(refreshEventHandle);
		}
		selectedPad = new_target.GetComponent<LaunchPad>();
		if (selectedPad == null)
		{
			Debug.LogError("The gameObject received does not contain a LaunchPad component");
			return;
		}
		refreshEventHandle = selectedPad.Subscribe(-887025858, RefreshWaitingToLandList);
		RefreshRocketButton();
		RefreshWaitingToLandList();
	}

	private void RefreshWaitingToLandList(object data = null)
	{
		for (int num = waitingToLandRows.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(waitingToLandRows[num]);
		}
		waitingToLandRows.Clear();
		nothingWaitingRow.SetActive(value: true);
		AxialI myWorldLocation = selectedPad.GetMyWorldLocation();
		foreach (ClusterGridEntity item in ClusterGrid.Instance.GetEntitiesInRange(myWorldLocation))
		{
			Clustercraft craft = item as Clustercraft;
			if (craft == null || craft.Status != Clustercraft.CraftStatus.InFlight || (craft.IsFlightInProgress() && craft.Destination != myWorldLocation))
			{
				continue;
			}
			GameObject gameObject = Util.KInstantiateUI(landableRocketRowPrefab, landableRowContainer, force_active: true);
			gameObject.GetComponentInChildren<LocText>().text = craft.Name;
			waitingToLandRows.Add(gameObject);
			KButton componentInChildren = gameObject.GetComponentInChildren<KButton>();
			componentInChildren.GetComponentInChildren<LocText>().SetText((craft.ModuleInterface.GetClusterDestinationSelector().GetDestinationPad() == selectedPad) ? UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.CANCEL_LAND_BUTTON : UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAND_BUTTON);
			componentInChildren.isInteractable = craft.CanLandAtPad(selectedPad, out var _) != Clustercraft.PadLandingStatus.CanNeverLand;
			componentInChildren.onClick += delegate
			{
				if (craft.ModuleInterface.GetClusterDestinationSelector().GetDestinationPad() == selectedPad)
				{
					craft.GetComponent<ClusterDestinationSelector>().SetDestination(craft.Location);
				}
				else
				{
					craft.LandAtPad(selectedPad);
				}
				RefreshWaitingToLandList();
			};
			nothingWaitingRow.SetActive(value: false);
		}
	}

	private void ClickStartNewRocket()
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.SetLaunchPad(selectedPad);
	}

	private void RefreshRocketButton()
	{
		startNewRocketbutton.isInteractable = selectedPad.LandedRocket == null;
	}
}
