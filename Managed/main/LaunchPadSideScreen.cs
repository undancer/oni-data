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

	public KButton launchRocketButton;

	public GameObject landableRowContainer;

	public GameObject nothingWaitingRow;

	public KScreen changeModuleSideScreen;

	public List<GameObject> waitingToLandRows = new List<GameObject>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		startNewRocketbutton.onClick += ClickStartNewRocket;
		launchRocketButton.onClick += ClickLaunchRocket;
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
		selectedPad = new_target.GetComponent<LaunchPad>();
		if (selectedPad == null)
		{
			Debug.LogError("The gameObject received does not contain a LaunchPad component");
			return;
		}
		RefreshRocketButton();
		RefreshWaitingToLandList();
	}

	private void RefreshWaitingToLandList()
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
			if (!(craft == null) && craft.Status != 0 && (!craft.IsFlightInProgress() || !(craft.Destination != myWorldLocation)))
			{
				GameObject gameObject = Util.KInstantiateUI(landableRocketRowPrefab, landableRowContainer, force_active: true);
				gameObject.GetComponentInChildren<LocText>().text = craft.Name;
				waitingToLandRows.Add(gameObject);
				KButton componentInChildren = gameObject.GetComponentInChildren<KButton>();
				componentInChildren.onClick += delegate
				{
					craft.LandAtPad(selectedPad);
					RefreshWaitingToLandList();
				};
				componentInChildren.gameObject.SetActive(craft.Destination != myWorldLocation);
				nothingWaitingRow.SetActive(value: false);
			}
		}
	}

	private void ClickStartNewRocket()
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.SetLaunchPad(selectedPad);
	}

	private void ClickLaunchRocket()
	{
	}

	private void RefreshRocketButton()
	{
		startNewRocketbutton.isInteractable = selectedPad.LandedRocket == null;
	}
}
