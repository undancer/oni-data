using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
	public GameObject listPanel;

	public GameObject rocketPanel;

	public LocText listHeaderLabel;

	public LocText listHeaderStatusLabel;

	public HierarchyReferences listRocketTemplate;

	public LocText listNoRocketText;

	public RectTransform rocketListContainer;

	private Dictionary<Spacecraft, HierarchyReferences> listRocketRows = new Dictionary<Spacecraft, HierarchyReferences>();

	[Header("Shared References")]
	public BreakdownList breakdownListPrefab;

	public GameObject progressBarPrefab;

	[Header("Selected Rocket References")]
	public LocText rocketHeaderLabel;

	public LocText rocketHeaderStatusLabel;

	private BreakdownList rocketDetailsStatus;

	public Sprite rocketDetailsStatusIcon;

	private BreakdownList rocketDetailsChecklist;

	public Sprite rocketDetailsChecklistIcon;

	private BreakdownList rocketDetailsMass;

	public Sprite rocketDetailsMassIcon;

	private BreakdownList rocketDetailsRange;

	public Sprite rocketDetailsRangeIcon;

	public RocketThrustWidget rocketThrustWidget;

	private BreakdownList rocketDetailsStorage;

	public Sprite rocketDetailsStorageIcon;

	private BreakdownList rocketDetailsDupes;

	public Sprite rocketDetailsDupesIcon;

	private BreakdownList rocketDetailsFuel;

	public Sprite rocketDetailsFuelIcon;

	private BreakdownList rocketDetailsOxidizer;

	public Sprite rocketDetailsOxidizerIcon;

	public RectTransform rocketDetailsContainer;

	[Header("Selected Destination References")]
	public LocText destinationHeaderLabel;

	public LocText destinationStatusLabel;

	public LocText destinationNameLabel;

	public LocText destinationTypeNameLabel;

	public LocText destinationTypeValueLabel;

	public LocText destinationDistanceNameLabel;

	public LocText destinationDistanceValueLabel;

	public LocText destinationDescriptionLabel;

	private BreakdownList destinationDetailsAnalysis;

	private GenericUIProgressBar destinationAnalysisProgressBar;

	public Sprite destinationDetailsAnalysisIcon;

	private BreakdownList destinationDetailsResearch;

	public Sprite destinationDetailsResearchIcon;

	private BreakdownList destinationDetailsMass;

	public Sprite destinationDetailsMassIcon;

	private BreakdownList destinationDetailsComposition;

	public Sprite destinationDetailsCompositionIcon;

	private BreakdownList destinationDetailsResources;

	public Sprite destinationDetailsResourcesIcon;

	private BreakdownList destinationDetailsArtifacts;

	public Sprite destinationDetailsArtifactsIcon;

	public RectTransform destinationDetailsContainer;

	public MultiToggle showRocketsButton;

	public MultiToggle launchButton;

	public MultiToggle analyzeButton;

	private int rocketConditionEventHandler = -1;

	[Header("Map References")]
	public RectTransform Map;

	public RectTransform rowsContiner;

	public GameObject rowPrefab;

	public StarmapPlanet planetPrefab;

	public GameObject rocketIconPrefab;

	private List<GameObject> planetRows = new List<GameObject>();

	private Dictionary<SpaceDestination, StarmapPlanet> planetWidgets = new Dictionary<SpaceDestination, StarmapPlanet>();

	private float planetsMaxDistance = 1f;

	public Image distanceOverlay;

	private int distanceOverlayVerticalOffset = 500;

	private int distanceOverlayYOffset = 24;

	public Image visualizeRocketImage;

	public Image visualizeRocketTrajectory;

	public LocText visualizeRocketLabel;

	public LocText visualizeRocketProgress;

	public Color[] distanceColors;

	public LocText titleBarLabel;

	public KButton button;

	private const int DESTINATION_ICON_SCALE = 2;

	public static StarmapScreen Instance;

	private int selectionUpdateHandle = -1;

	private SpaceDestination selectedDestination;

	private KSelectable currentSelectable;

	private CommandModule currentCommandModule;

	private LaunchConditionManager currentLaunchConditionManager;

	private bool currentRocketHasGasContainer;

	private bool currentRocketHasLiquidContainer;

	private bool currentRocketHasSolidContainer;

	private bool currentRocketHasEntitiesContainer;

	private bool forceScrollDown = true;

	private Coroutine animateAnalysisRoutine;

	private Coroutine animateSelectedPlanetRoutine;

	private BreakdownListRow rangeRowTotal;

	public override float GetSortKey()
	{
		return 20f;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
		rocketDetailsStatus = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsStatus.SetTitle(UI.STARMAP.LISTTITLES.MISSIONSTATUS);
		rocketDetailsStatus.SetIcon(rocketDetailsStatusIcon);
		rocketDetailsStatus.gameObject.name = "rocketDetailsStatus";
		rocketDetailsChecklist = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsChecklist.SetTitle(UI.STARMAP.LISTTITLES.LAUNCHCHECKLIST);
		rocketDetailsChecklist.SetIcon(rocketDetailsChecklistIcon);
		rocketDetailsChecklist.gameObject.name = "rocketDetailsChecklist";
		rocketDetailsRange = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsRange.SetTitle(UI.STARMAP.LISTTITLES.MAXRANGE);
		rocketDetailsRange.SetIcon(rocketDetailsRangeIcon);
		rocketDetailsRange.gameObject.name = "rocketDetailsRange";
		rocketDetailsMass = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.MASS);
		rocketDetailsMass.SetIcon(rocketDetailsMassIcon);
		rocketDetailsMass.gameObject.name = "rocketDetailsMass";
		rocketThrustWidget = UnityEngine.Object.Instantiate(rocketThrustWidget, rocketDetailsContainer);
		rocketDetailsStorage = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsStorage.SetTitle(UI.STARMAP.LISTTITLES.STORAGE);
		rocketDetailsStorage.SetIcon(rocketDetailsStorageIcon);
		rocketDetailsStorage.gameObject.name = "rocketDetailsStorage";
		rocketDetailsFuel = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsFuel.SetTitle(UI.STARMAP.LISTTITLES.FUEL);
		rocketDetailsFuel.SetIcon(rocketDetailsFuelIcon);
		rocketDetailsFuel.gameObject.name = "rocketDetailsFuel";
		rocketDetailsOxidizer = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsOxidizer.SetTitle(UI.STARMAP.LISTTITLES.OXIDIZER);
		rocketDetailsOxidizer.SetIcon(rocketDetailsOxidizerIcon);
		rocketDetailsOxidizer.gameObject.name = "rocketDetailsOxidizer";
		rocketDetailsDupes = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsDupes.SetTitle(UI.STARMAP.LISTTITLES.PASSENGERS);
		rocketDetailsDupes.SetIcon(rocketDetailsDupesIcon);
		rocketDetailsDupes.gameObject.name = "rocketDetailsDupes";
		destinationDetailsAnalysis = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsAnalysis.SetTitle(UI.STARMAP.LISTTITLES.ANALYSIS);
		destinationDetailsAnalysis.SetIcon(destinationDetailsAnalysisIcon);
		destinationDetailsAnalysis.gameObject.name = "destinationDetailsAnalysis";
		destinationDetailsAnalysis.SetDescription(string.Format(UI.STARMAP.ANALYSIS_DESCRIPTION, 0));
		destinationAnalysisProgressBar = UnityEngine.Object.Instantiate(progressBarPrefab.gameObject, destinationDetailsContainer).GetComponent<GenericUIProgressBar>();
		destinationAnalysisProgressBar.SetMaxValue(ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
		destinationDetailsResearch = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsResearch.SetTitle(UI.STARMAP.LISTTITLES.RESEARCH);
		destinationDetailsResearch.SetIcon(destinationDetailsResearchIcon);
		destinationDetailsResearch.gameObject.name = "destinationDetailsResearch";
		destinationDetailsResearch.SetDescription(string.Format(UI.STARMAP.RESEARCH_DESCRIPTION, 0));
		destinationDetailsMass = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.DESTINATION_MASS);
		destinationDetailsMass.SetIcon(destinationDetailsMassIcon);
		destinationDetailsMass.gameObject.name = "destinationDetailsMass";
		destinationDetailsComposition = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsComposition.SetTitle(UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
		destinationDetailsComposition.SetIcon(destinationDetailsCompositionIcon);
		destinationDetailsComposition.gameObject.name = "destinationDetailsComposition";
		destinationDetailsResources = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsResources.SetTitle(UI.STARMAP.LISTTITLES.RESOURCES);
		destinationDetailsResources.SetIcon(destinationDetailsResourcesIcon);
		destinationDetailsResources.gameObject.name = "destinationDetailsResources";
		destinationDetailsArtifacts = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsArtifacts.SetTitle(UI.STARMAP.LISTTITLES.ARTIFACTS);
		destinationDetailsArtifacts.SetIcon(destinationDetailsArtifactsIcon);
		destinationDetailsArtifacts.gameObject.name = "destinationDetailsArtifacts";
		LoadPlanets();
		selectionUpdateHandle = Game.Instance.Subscribe(-1503271301, OnSelectableChanged);
		titleBarLabel.text = UI.STARMAP.TITLE;
		button.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		launchButton.play_sound_on_click = false;
		MultiToggle multiToggle = launchButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			if (currentLaunchConditionManager != null && selectedDestination != null)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
				LaunchRocket(currentLaunchConditionManager);
			}
			else
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
			}
		});
		launchButton.ChangeState(1);
		MultiToggle multiToggle2 = showRocketsButton;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			OnSelectableChanged(null);
		});
		SelectDestination(null);
		SpacecraftManager.instance.Subscribe(532901469, delegate
		{
			RefreshAnalyzeButton();
			UpdateDestinationStates();
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (selectionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(selectionUpdateHandle);
		}
		StopAllCoroutines();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap");
			UpdateDestinationStates();
			Refresh();
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
			MusicManager.instance.StopSong("Music_Starmap");
		}
		OnSelectableChanged((SelectTool.Instance.selected == null) ? null : SelectTool.Instance.selected.gameObject);
		forceScrollDown = true;
	}

	private void UpdateDestinationStates()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		foreach (SpaceDestination destination in SpacecraftManager.instance.destinations)
		{
			num = Mathf.Max(num, destination.OneBasedDistance);
			if (destination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num2 = Mathf.Max(num2, destination.OneBasedDistance);
			}
		}
		for (int i = num2; i < num; i++)
		{
			bool flag = false;
			foreach (SpaceDestination destination2 in SpacecraftManager.instance.destinations)
			{
				if (destination2.distance == i)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			num3++;
		}
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> KVP in planetWidgets)
		{
			SpaceDestination key = KVP.Key;
			StarmapPlanet planet = KVP.Value;
			Color color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
			Color color2 = new Color(0.75f, 0.75f, 0.75f, 0.75f);
			if (KVP.Key.distance >= num2 + num3)
			{
				planet.SetUnknownBGActive(active: false, Color.white);
				planet.SetSprite(Assets.GetSprite("unknown_far"), color);
				continue;
			}
			planet.SetAnalysisActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == KVP.Key.id);
			bool flag2 = SpacecraftManager.instance.GetDestinationAnalysisState(key) == SpacecraftManager.DestinationAnalysisState.Complete;
			SpaceDestinationType destinationType = key.GetDestinationType();
			planet.SetLabel(flag2 ? (destinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float)KVP.Key.OneBasedDistance * 10000f * 1000f) + "</color>") : string.Concat(UI.STARMAP.UNKNOWN_DESTINATION, "\n", string.Format(UI.STARMAP.ANALYSIS_AMOUNT.text, GameUtil.GetFormattedPercent(100f * (SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE)))));
			planet.SetSprite(flag2 ? Assets.GetSprite(destinationType.spriteName) : Assets.GetSprite("unknown"), flag2 ? Color.white : color2);
			planet.SetUnknownBGActive(SpacecraftManager.instance.GetDestinationAnalysisState(KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete, color2);
			planet.SetFillAmount(SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
			List<int> spacecraftsForDestination = SpacecraftManager.instance.GetSpacecraftsForDestination(key);
			planet.SetRocketIcons(spacecraftsForDestination.Count, rocketIconPrefab);
			bool show = currentLaunchConditionManager != null && key == SpacecraftManager.instance.GetSpacecraftDestination(currentLaunchConditionManager);
			planet.ShowAsCurrentRocketDestination(show);
			planet.SetOnClick(delegate
			{
				if (currentLaunchConditionManager == null)
				{
					SelectDestination(KVP.Key);
				}
				else if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(currentLaunchConditionManager).state == Spacecraft.MissionState.Grounded)
				{
					SelectDestination(KVP.Key);
				}
			});
			planet.SetOnEnter(delegate
			{
				planet.ShowLabel(show: true);
			});
			planet.SetOnExit(delegate
			{
				planet.ShowLabel(show: false);
			});
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
	}

	private string DisplayDistance(float distance)
	{
		return Util.FormatWholeNumber(distance) + " " + UI.UNITSUFFIXES.DISTANCE.KILOMETER;
	}

	private string DisplayDestinationMass(SpaceDestination selectedDestination)
	{
		return GameUtil.GetFormattedMass(selectedDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne);
	}

	private string DisplayTotalStorageCapacity(CommandModule command)
	{
		float num = 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(command.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if (component != null)
			{
				num += component.storage.Capacity();
			}
		}
		return GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne);
	}

	private string StorageCapacityTooltip(CommandModule command, SpaceDestination dest)
	{
		string text = "";
		bool flag = dest != null && SpacecraftManager.instance.GetDestinationAnalysisState(dest) == SpacecraftManager.DestinationAnalysisState.Complete;
		if (dest != null && flag)
		{
			if (dest.AvailableMass <= ConditionHasMinimumMass.CargoCapacity(dest, command))
			{
				text = string.Concat(text, UI.STARMAP.LAUNCHCHECKLIST.INSUFFICENT_MASS_TOOLTIP, UI.HORIZONTAL_BR_RULE);
			}
			text = text + string.Format(UI.STARMAP.LAUNCHCHECKLIST.RESOURCE_MASS_TOOLTIP, dest.GetDestinationType().Name, GameUtil.GetFormattedMass(dest.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(ConditionHasMinimumMass.CargoCapacity(dest, command), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)) + "\n\n";
		}
		float num = dest?.AvailableMass ?? 0f;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(command.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if (component != null)
			{
				if (flag)
				{
					float availableResourcesPercentage = dest.GetAvailableResourcesPercentage(component.storageType);
					float num2 = Mathf.Min(component.storage.Capacity(), availableResourcesPercentage * num);
					num -= num2;
					text = text + component.gameObject.GetProperName() + " " + string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)) + "\n";
				}
				else
				{
					text = text + component.gameObject.GetProperName() + " " + string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)) + "\n";
				}
			}
		}
		return text;
	}

	private void LoadPlanets()
	{
		foreach (SpaceDestination destination in Game.Instance.spacecraftManager.destinations)
		{
			if ((float)destination.OneBasedDistance * 10000f > planetsMaxDistance)
			{
				planetsMaxDistance = (float)destination.OneBasedDistance * 10000f;
			}
			while (planetRows.Count < destination.distance + 1)
			{
				GameObject gameObject = Util.KInstantiateUI(rowPrefab, rowsContiner.gameObject, force_active: true);
				gameObject.rectTransform().SetAsFirstSibling();
				planetRows.Add(gameObject);
				gameObject.GetComponentInChildren<Image>().color = distanceColors[planetRows.Count % distanceColors.Length];
				gameObject.GetComponentInChildren<LocText>().text = DisplayDistance((float)(planetRows.Count + 1) * 10000f);
			}
			GameObject gameObject2 = Util.KInstantiateUI(planetPrefab.gameObject, planetRows[destination.distance], force_active: true);
			planetWidgets.Add(destination, gameObject2.GetComponent<StarmapPlanet>());
		}
		UpdateDestinationStates();
	}

	private void UnselectAllPlanets()
	{
		if (animateSelectedPlanetRoutine != null)
		{
			StopCoroutine(animateSelectedPlanetRoutine);
		}
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in planetWidgets)
		{
			planetWidget.Value.SetSelectionActive(active: false);
			planetWidget.Value.ShowAsCurrentRocketDestination(show: false);
		}
	}

	private void SelectPlanet(StarmapPlanet planet)
	{
		planet.SetSelectionActive(active: true);
		if (animateSelectedPlanetRoutine != null)
		{
			StopCoroutine(animateSelectedPlanetRoutine);
		}
		animateSelectedPlanetRoutine = StartCoroutine(AnimatePlanetSelection(planet));
	}

	private IEnumerator AnimatePlanetSelection(StarmapPlanet planet)
	{
		while (true)
		{
			planet.AnimateSelector(Time.unscaledTime);
			yield return new WaitForEndOfFrame();
		}
	}

	private void Update()
	{
		PositionPlanetWidgets();
		if (forceScrollDown)
		{
			ScrollToBottom();
			forceScrollDown = false;
		}
	}

	private void ScrollToBottom()
	{
		RectTransform rectTransform = Map.GetComponentInChildren<VerticalLayoutGroup>().rectTransform();
		rectTransform.SetLocalPosition(new Vector3(rectTransform.localPosition.x, rectTransform.rect.height - Map.rect.height, rectTransform.localPosition.z));
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		if (UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
				{
					EditableTitleBar component = listRocketRow.Value.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
					if (currentSelectedGameObject == component.inputField.gameObject)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void PositionPlanetWidgets()
	{
		float num = rowPrefab.GetComponent<RectTransform>().rect.height / 2f;
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in planetWidgets)
		{
			planetWidget.Value.rectTransform().anchoredPosition = new Vector2(planetWidget.Value.transform.parent.rectTransform().sizeDelta.x * planetWidget.Key.startingOrbitPercentage, 0f - num);
		}
	}

	private void OnSelectableChanged(object data)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (rocketConditionEventHandler != -1)
		{
			Unsubscribe(rocketConditionEventHandler);
		}
		if (data != null)
		{
			currentSelectable = ((GameObject)data).GetComponent<KSelectable>();
			currentCommandModule = currentSelectable.GetComponent<CommandModule>();
			currentLaunchConditionManager = currentSelectable.GetComponent<LaunchConditionManager>();
			if (currentCommandModule != null && currentLaunchConditionManager != null)
			{
				SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(currentLaunchConditionManager);
				SelectDestination(spacecraftDestination);
				rocketConditionEventHandler = currentLaunchConditionManager.Subscribe(1655598572, Refresh);
				ShowRocketDetailsPanel();
			}
			else
			{
				currentSelectable = null;
				currentCommandModule = null;
				currentLaunchConditionManager = null;
				ShowRocketListPanel();
			}
		}
		else
		{
			currentSelectable = null;
			currentCommandModule = null;
			currentLaunchConditionManager = null;
			ShowRocketListPanel();
		}
		Refresh();
	}

	private void ShowRocketListPanel()
	{
		listPanel.SetActive(value: true);
		rocketPanel.SetActive(value: false);
		launchButton.ChangeState(1);
		UpdateDistanceOverlay();
		UpdateMissionOverlay();
	}

	private void ShowRocketDetailsPanel()
	{
		listPanel.SetActive(value: false);
		rocketPanel.SetActive(value: true);
		ValidateTravelAbility();
		UpdateDistanceOverlay();
		UpdateMissionOverlay();
	}

	private void LaunchRocket(LaunchConditionManager lcm)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcm);
		if (spacecraftDestination != null)
		{
			lcm.Launch(spacecraftDestination);
			ClearRocketListPanel();
			FillRocketListPanel();
			ShowRocketListPanel();
			Refresh();
		}
	}

	private void FillRocketListPanel()
	{
		ClearRocketListPanel();
		List<Spacecraft> spacecraft = SpacecraftManager.instance.GetSpacecraft();
		if (spacecraft.Count == 0)
		{
			listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
			listNoRocketText.gameObject.SetActive(value: true);
		}
		else
		{
			listHeaderStatusLabel.text = string.Format(UI.STARMAP.ROCKET_COUNT, spacecraft.Count);
			listNoRocketText.gameObject.SetActive(value: false);
		}
		foreach (Spacecraft rocket in spacecraft)
		{
			HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(listRocketTemplate.gameObject, rocketListContainer.gameObject, force_active: true);
			BreakdownList component = hierarchyReferences.GetComponent<BreakdownList>();
			MultiToggle component2 = hierarchyReferences.GetComponent<MultiToggle>();
			EditableTitleBar component3 = hierarchyReferences.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
			MultiToggle component4 = hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			MultiToggle component5 = hierarchyReferences.GetReference<RectTransform>("LandRocketButton").GetComponent<MultiToggle>();
			HierarchyReferences component6 = hierarchyReferences.GetReference<RectTransform>("ProgressBar").GetComponent<HierarchyReferences>();
			LaunchConditionManager launchConditionManager = rocket.launchConditions;
			CommandModule component7 = launchConditionManager.GetComponent<CommandModule>();
			MinionStorage component8 = launchConditionManager.GetComponent<MinionStorage>();
			component3.SetTitle(rocket.rocketName);
			component3.OnNameChanged += delegate(string newName)
			{
				rocket.SetRocketName(newName);
			};
			component2.onEnter = (System.Action)Delegate.Combine(component2.onEnter, (System.Action)delegate
			{
				LaunchConditionManager launchConditions = rocket.launchConditions;
				UpdateDistanceOverlay(launchConditions);
				UpdateMissionOverlay(launchConditions);
			});
			component2.onExit = (System.Action)Delegate.Combine(component2.onExit, (System.Action)delegate
			{
				UpdateDistanceOverlay();
				UpdateMissionOverlay();
			});
			component2.onClick = (System.Action)Delegate.Combine(component2.onClick, (System.Action)delegate
			{
				OnSelectableChanged(rocket.launchConditions.gameObject);
			});
			component4.play_sound_on_click = false;
			component4.onClick = (System.Action)Delegate.Combine(component4.onClick, (System.Action)delegate
			{
				if (launchConditionManager != null)
				{
					KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
					LaunchRocket(launchConditionManager);
				}
				else
				{
					KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
				}
			});
			if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).state != 0)
			{
				component5.gameObject.SetActive(value: true);
				component5.transform.SetAsLastSibling();
				component5.play_sound_on_click = false;
				component5.onClick = (System.Action)Delegate.Combine(component5.onClick, (System.Action)delegate
				{
					if (launchConditionManager != null)
					{
						KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
						SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).ForceComplete();
						ClearRocketListPanel();
						FillRocketListPanel();
						ShowRocketListPanel();
						Refresh();
					}
					else
					{
						KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
					}
				});
			}
			else
			{
				component5.gameObject.SetActive(value: false);
			}
			BreakdownListRow breakdownListRow = component.AddRow();
			string text = UI.STARMAP.MISSION_STATUS.GROUNDED;
			BreakdownListRow.Status status = BreakdownListRow.Status.Green;
			Tuple<string, BreakdownListRow.Status> textForState = GetTextForState(rocket.state);
			breakdownListRow.ShowStatusData(value: textForState.first, dotColor: textForState.second, name: UI.STARMAP.ROCKETSTATUS.STATUS);
			breakdownListRow.SetHighlighted(highlighted: true);
			if (component8 != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component8.GetStoredMinionInfo();
				BreakdownListRow breakdownListRow2 = component.AddRow();
				int count = storedMinionInfo.Count;
				breakdownListRow2.ShowStatusData(UI.STARMAP.LISTTITLES.PASSENGERS, count.ToString(), (count == 0) ? BreakdownListRow.Status.Red : BreakdownListRow.Status.Green);
			}
			if (rocket.state == Spacecraft.MissionState.Grounded)
			{
				string text2 = "";
				List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchConditionManager.GetComponent<AttachableBuilding>());
				foreach (GameObject item in attachedNetwork)
				{
					text2 = text2 + item.GetProperName() + "\n";
				}
				BreakdownListRow breakdownListRow3 = component.AddRow();
				breakdownListRow3.ShowData(UI.STARMAP.LISTTITLES.MODULES, attachedNetwork.Count.ToString());
				breakdownListRow3.AddTooltip(text2);
				component.AddRow().ShowData(UI.STARMAP.LISTTITLES.MAXRANGE, DisplayDistance(component7.rocketStats.GetRocketMaxDistance()));
				BreakdownListRow breakdownListRow4 = component.AddRow();
				breakdownListRow4.ShowData(UI.STARMAP.LISTTITLES.STORAGE, DisplayTotalStorageCapacity(component7));
				breakdownListRow4.AddTooltip(StorageCapacityTooltip(component7, selectedDestination));
				BreakdownListRow breakdownListRow5 = component.AddRow();
				if (selectedDestination != null)
				{
					if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
					{
						bool flag = selectedDestination.AvailableMass >= ConditionHasMinimumMass.CargoCapacity(selectedDestination, component7);
						breakdownListRow5.ShowStatusData(UI.STARMAP.LISTTITLES.DESTINATION_MASS, DisplayDestinationMass(selectedDestination), (!flag) ? BreakdownListRow.Status.Yellow : BreakdownListRow.Status.Default);
						breakdownListRow5.AddTooltip(StorageCapacityTooltip(component7, selectedDestination));
					}
					else
					{
						breakdownListRow5.ShowStatusData(UI.STARMAP.LISTTITLES.DESTINATION_MASS, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT, BreakdownListRow.Status.Default);
					}
				}
				else
				{
					breakdownListRow5.ShowStatusData(UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED, "", BreakdownListRow.Status.Red);
					breakdownListRow5.AddTooltip(UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED);
				}
				component4.GetComponent<RectTransform>().SetAsLastSibling();
				component4.gameObject.SetActive(value: true);
				component6.gameObject.SetActive(value: false);
			}
			else
			{
				float duration = rocket.GetDuration();
				float timeLeft = rocket.GetTimeLeft();
				float num = ((duration == 0f) ? 0f : (1f - timeLeft / duration));
				component.AddRow().ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration));
				component6.gameObject.SetActive(value: true);
				RectTransform reference = component6.GetReference<RectTransform>("ProgressImage");
				LocText component9 = component6.GetReference<RectTransform>("ProgressText").GetComponent<LocText>();
				reference.transform.localScale = new Vector3(num, 1f, 1f);
				component9.text = GameUtil.GetFormattedPercent(num * 100f);
				component6.GetComponent<RectTransform>().SetAsLastSibling();
				component4.gameObject.SetActive(value: false);
			}
			listRocketRows.Add(rocket, hierarchyReferences);
		}
		UpdateRocketRowsTravelAbility();
	}

	public static Tuple<string, BreakdownListRow.Status> GetTextForState(Spacecraft.MissionState state)
	{
		return state switch
		{
			Spacecraft.MissionState.Grounded => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.GROUNDED, BreakdownListRow.Status.Green), 
			Spacecraft.MissionState.Launching => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.LAUNCHING, BreakdownListRow.Status.Yellow), 
			Spacecraft.MissionState.WaitingToLand => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.WAITING_TO_LAND, BreakdownListRow.Status.Yellow), 
			Spacecraft.MissionState.Landing => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.LANDING, BreakdownListRow.Status.Yellow), 
			Spacecraft.MissionState.Underway => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.UNDERWAY, BreakdownListRow.Status.Red), 
			_ => new Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.DESTROYED, BreakdownListRow.Status.Red), 
		};
	}

	private void ClearRocketListPanel()
	{
		listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
		{
			UnityEngine.Object.Destroy(listRocketRow.Value.gameObject);
		}
		listRocketRows.Clear();
	}

	private void FillChecklist(LaunchConditionManager launchConditionManager)
	{
		foreach (ProcessCondition launchCondition in launchConditionManager.GetLaunchConditionList())
		{
			BreakdownListRow breakdownListRow = rocketDetailsChecklist.AddRow();
			string statusMessage = launchCondition.GetStatusMessage(ProcessCondition.Status.Ready);
			ProcessCondition.Status status = launchCondition.EvaluateCondition();
			BreakdownListRow.Status status2 = BreakdownListRow.Status.Green;
			switch (status)
			{
			case ProcessCondition.Status.Failure:
				status2 = BreakdownListRow.Status.Red;
				break;
			case ProcessCondition.Status.Warning:
				status2 = BreakdownListRow.Status.Yellow;
				break;
			}
			breakdownListRow.ShowCheckmarkData(statusMessage, "", status2);
			if (status != ProcessCondition.Status.Ready)
			{
				breakdownListRow.SetHighlighted(highlighted: true);
			}
			breakdownListRow.AddTooltip(launchCondition.GetStatusTooltip(status));
		}
	}

	private void SelectDestination(SpaceDestination destination)
	{
		selectedDestination = destination;
		UnselectAllPlanets();
		if (selectedDestination != null)
		{
			SelectPlanet(planetWidgets[selectedDestination]);
			if (currentLaunchConditionManager != null)
			{
				SpacecraftManager.instance.SetSpacecraftDestination(currentLaunchConditionManager, selectedDestination);
			}
			ShowDestinationPanel();
			UpdateRocketRowsTravelAbility();
		}
		else
		{
			ClearDestinationPanel();
		}
		if (rangeRowTotal != null && selectedDestination != null && currentCommandModule != null)
		{
			rangeRowTotal.SetStatusColor((!currentCommandModule.conditions.reachable.CanReachSpacecraftDestination(selectedDestination)) ? BreakdownListRow.Status.Red : BreakdownListRow.Status.Green);
		}
		UpdateDestinationStates();
		Refresh();
	}

	private void UpdateRocketRowsTravelAbility()
	{
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
		{
			Spacecraft key = listRocketRow.Key;
			LaunchConditionManager launchConditions = key.launchConditions;
			CommandModule component = launchConditions.GetComponent<CommandModule>();
			MultiToggle component2 = listRocketRow.Value.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			bool flag = key.state == Spacecraft.MissionState.Grounded;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(launchConditions);
			bool flag2 = spacecraftDestination != null && component.conditions.reachable.CanReachSpacecraftDestination(spacecraftDestination);
			bool flag3 = launchConditions.CheckReadyToLaunch();
			component2.ChangeState((!(flag && flag2 && flag3)) ? 1 : 0);
		}
	}

	private void RefreshAnalyzeButton()
	{
		if (selectedDestination == null)
		{
			analyzeButton.ChangeState(1);
			analyzeButton.onClick = null;
			analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.NO_ANALYZABLE_DESTINATION_SELECTED;
			return;
		}
		if (selectedDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			if (DebugHandler.InstantBuildMode)
			{
				analyzeButton.ChangeState(0);
				analyzeButton.onClick = delegate
				{
					selectedDestination.TryCompleteResearchOpportunity();
					ShowDestinationPanel();
				};
				analyzeButton.GetComponentInChildren<LocText>().text = string.Concat(UI.STARMAP.ANALYSIS_COMPLETE, " (debug research)");
			}
			else
			{
				analyzeButton.ChangeState(1);
				analyzeButton.onClick = null;
				analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE;
			}
			return;
		}
		analyzeButton.ChangeState(0);
		if (selectedDestination.id == SpacecraftManager.instance.GetStarmapAnalysisDestinationID())
		{
			analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.SUSPEND_DESTINATION_ANALYSIS;
			analyzeButton.onClick = delegate
			{
				SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1);
			};
			return;
		}
		analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYZE_DESTINATION;
		analyzeButton.onClick = delegate
		{
			if (DebugHandler.InstantBuildMode)
			{
				SpacecraftManager.instance.SetStarmapAnalysisDestinationID(selectedDestination.id);
				SpacecraftManager.instance.EarnDestinationAnalysisPoints(selectedDestination.id, 99999f);
				ShowDestinationPanel();
			}
			else
			{
				SpacecraftManager.instance.SetStarmapAnalysisDestinationID(selectedDestination.id);
			}
		};
	}

	private void Refresh(object data = null)
	{
		FillRocketListPanel();
		RefreshAnalyzeButton();
		ShowDestinationPanel();
		if (currentCommandModule != null && currentLaunchConditionManager != null)
		{
			FillRocketPanel();
			if (selectedDestination != null)
			{
				ValidateTravelAbility();
			}
		}
		else
		{
			ClearRocketPanel();
		}
	}

	private void ClearRocketPanel()
	{
		rocketHeaderStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
		rocketDetailsChecklist.ClearRows();
		rocketDetailsMass.ClearRows();
		rocketDetailsRange.ClearRows();
		rocketThrustWidget.gameObject.SetActive(value: false);
		rocketDetailsStorage.ClearRows();
		rocketDetailsFuel.ClearRows();
		rocketDetailsOxidizer.ClearRows();
		rocketDetailsDupes.ClearRows();
		rocketDetailsStatus.ClearRows();
		currentRocketHasLiquidContainer = false;
		currentRocketHasGasContainer = false;
		currentRocketHasSolidContainer = false;
		currentRocketHasEntitiesContainer = false;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rocketDetailsContainer);
	}

	private void FillRocketPanel()
	{
		ClearRocketPanel();
		rocketHeaderStatusLabel.text = UI.STARMAP.STATUS;
		UpdateDistanceOverlay();
		UpdateMissionOverlay();
		FillChecklist(currentLaunchConditionManager);
		UpdateRangeDisplay();
		UpdateMassDisplay();
		UpdateOxidizerDisplay();
		UpdateStorageDisplay();
		UpdateFuelDisplay();
		LayoutRebuilder.ForceRebuildLayoutImmediate(rocketDetailsContainer);
	}

	private void UpdateRangeDisplay()
	{
		rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZABLE_FUEL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalOxidizableFuel()));
		rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.ENGINE_EFFICIENCY, GameUtil.GetFormattedEngineEfficiency(currentCommandModule.rocketStats.GetEngineEfficiency()));
		rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.OXIDIZER_EFFICIENCY, GameUtil.GetFormattedPercent(currentCommandModule.rocketStats.GetAverageOxidizerEfficiency()));
		float num = currentCommandModule.rocketStats.GetBoosterThrust() * 1000f;
		if (num != 0f)
		{
			rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.SOLID_BOOSTER, GameUtil.GetFormattedDistance(num));
		}
		BreakdownListRow breakdownListRow = rocketDetailsRange.AddRow();
		breakdownListRow.ShowStatusData(UI.STARMAP.ROCKETSTATS.TOTAL_THRUST, GameUtil.GetFormattedDistance(currentCommandModule.rocketStats.GetTotalThrust() * 1000f), BreakdownListRow.Status.Green);
		breakdownListRow.SetImportant(important: true);
		float distance = 0f - (currentCommandModule.rocketStats.GetTotalThrust() - currentCommandModule.rocketStats.GetRocketMaxDistance());
		rocketThrustWidget.gameObject.SetActive(value: true);
		BreakdownListRow breakdownListRow2 = rocketDetailsRange.AddRow();
		breakdownListRow2.ShowStatusData(UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, DisplayDistance(distance), BreakdownListRow.Status.Red);
		breakdownListRow2.SetHighlighted(highlighted: true);
		rocketDetailsRange.AddCustomRow(rocketThrustWidget.gameObject);
		rocketThrustWidget.Draw(currentCommandModule);
		BreakdownListRow breakdownListRow3 = rocketDetailsRange.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_RANGE, GameUtil.GetFormattedDistance(currentCommandModule.rocketStats.GetRocketMaxDistance() * 1000f));
		breakdownListRow3.SetImportant(important: true);
	}

	private void UpdateMassDisplay()
	{
		rocketDetailsMass.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.DRY_MASS, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetDryMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
		rocketDetailsMass.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.WET_MASS, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetWetMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
		BreakdownListRow breakdownListRow = rocketDetailsMass.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
		breakdownListRow.SetImportant(important: true);
	}

	private void UpdateFuelDisplay()
	{
		Tag engineFuelTag = currentCommandModule.rocketStats.GetEngineFuelTag();
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			IFuelTank component = item.GetComponent<IFuelTank>();
			if (!component.IsNullOrDestroyed())
			{
				BreakdownListRow breakdownListRow = rocketDetailsFuel.AddRow();
				if (engineFuelTag.IsValid)
				{
					Element element = ElementLoader.GetElement(engineFuelTag);
					Debug.Assert(element != null, "fuel_element");
					breakdownListRow.ShowData(item.gameObject.GetProperName() + " (" + element.name + ")", GameUtil.GetFormattedMass(component.Storage.GetAmountAvailable(engineFuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
				}
				else
				{
					breakdownListRow.ShowData(item.gameObject.GetProperName(), UI.STARMAP.ROCKETSTATS.NO_ENGINE);
					breakdownListRow.SetStatusColor(BreakdownListRow.Status.Red);
				}
			}
			SolidBooster component2 = item.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				BreakdownListRow breakdownListRow2 = rocketDetailsFuel.AddRow();
				Element element2 = ElementLoader.GetElement(component2.fuelTag);
				Debug.Assert(element2 != null, "fuel_element");
				breakdownListRow2.ShowData(item.gameObject.GetProperName() + " (" + element2.name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(component2.fuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
			}
		}
		BreakdownListRow breakdownListRow3 = rocketDetailsFuel.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_FUEL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalFuel(includeBoosters: true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
		breakdownListRow3.SetImportant(important: true);
	}

	private void UpdateOxidizerDisplay()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = item.GetComponent<OxidizerTank>();
			if (component != null)
			{
				foreach (KeyValuePair<Tag, float> item2 in component.GetOxidizersAvailable())
				{
					if (item2.Value != 0f)
					{
						rocketDetailsOxidizer.AddRow().ShowData(item.gameObject.GetProperName() + " (" + item2.Key.ProperName() + ")", GameUtil.GetFormattedMass(item2.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
					}
				}
			}
			SolidBooster component2 = item.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				rocketDetailsOxidizer.AddRow().ShowData(item.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
			}
		}
		BreakdownListRow breakdownListRow = rocketDetailsOxidizer.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZER, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalOxidizer(includeBoosters: true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
		breakdownListRow.SetImportant(important: true);
	}

	private void UpdateStorageDisplay()
	{
		float num = ((selectedDestination != null) ? selectedDestination.AvailableMass : 0f);
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if (component != null)
			{
				BreakdownListRow breakdownListRow = rocketDetailsStorage.AddRow();
				if (selectedDestination != null)
				{
					float availableResourcesPercentage = selectedDestination.GetAvailableResourcesPercentage(component.storageType);
					float num2 = Mathf.Min(component.storage.Capacity(), availableResourcesPercentage * num);
					num -= num2;
					breakdownListRow.ShowData(item.gameObject.GetProperName(), string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)));
				}
				else
				{
					breakdownListRow.ShowData(item.gameObject.GetProperName(), string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)));
				}
			}
		}
	}

	private void ClearDestinationPanel()
	{
		destinationDetailsContainer.gameObject.SetActive(value: false);
		destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
	}

	private void ShowDestinationPanel()
	{
		if (selectedDestination == null)
		{
			return;
		}
		destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.SELECTED;
		if (currentLaunchConditionManager != null && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(currentLaunchConditionManager).state != 0)
		{
			destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.LOCKEDIN;
		}
		SpaceDestinationType destinationType = selectedDestination.GetDestinationType();
		destinationNameLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete) ? destinationType.Name : UI.STARMAP.UNKNOWN_DESTINATION.text);
		destinationTypeValueLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete) ? destinationType.typeName : UI.STARMAP.UNKNOWN_TYPE.text);
		destinationDistanceValueLabel.text = DisplayDistance((float)selectedDestination.OneBasedDistance * 10000f);
		destinationDescriptionLabel.text = destinationType.description;
		destinationDetailsComposition.ClearRows();
		destinationDetailsResearch.ClearRows();
		destinationDetailsMass.ClearRows();
		destinationDetailsResources.ClearRows();
		destinationDetailsArtifacts.ClearRows();
		if (destinationType.visitable)
		{
			float num = 0f;
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num = selectedDestination.GetTotalMass();
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (SpaceDestination.ResearchOpportunity researchOpportunity in selectedDestination.researchOpportunities)
				{
					BreakdownListRow breakdownListRow = destinationDetailsResearch.AddRow();
					string name = ((researchOpportunity.discoveredRareResource != SimHashes.Void) ? $"(!!) {researchOpportunity.description}" : researchOpportunity.description);
					breakdownListRow.ShowCheckmarkData(name, researchOpportunity.dataValue.ToString(), researchOpportunity.completed ? BreakdownListRow.Status.Green : BreakdownListRow.Status.Default);
				}
			}
			destinationAnalysisProgressBar.SetFillPercentage(SpacecraftManager.instance.GetDestinationAnalysisScore(selectedDestination.id) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
			float num2 = ConditionHasMinimumMass.CargoCapacity(selectedDestination, currentCommandModule);
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				string formattedMass = GameUtil.GetFormattedMass(selectedDestination.CurrentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne);
				string formattedMass2 = GameUtil.GetFormattedMass(destinationType.minimumMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne);
				BreakdownListRow breakdownListRow2 = destinationDetailsMass.AddRow();
				breakdownListRow2.ShowData(UI.STARMAP.CURRENT_MASS, formattedMass);
				if (selectedDestination.AvailableMass < num2)
				{
					breakdownListRow2.SetStatusColor(BreakdownListRow.Status.Yellow);
					breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CURRENT_MASS_TOOLTIP, GameUtil.GetFormattedMass(selectedDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram)));
				}
				destinationDetailsMass.AddRow().ShowData(UI.STARMAP.MAXIMUM_MASS, GameUtil.GetFormattedMass(destinationType.maxiumMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne));
				BreakdownListRow breakdownListRow3 = destinationDetailsMass.AddRow();
				breakdownListRow3.ShowData(UI.STARMAP.MINIMUM_MASS, formattedMass2);
				breakdownListRow3.AddTooltip(UI.STARMAP.MINIMUM_MASS_TOOLTIP);
				BreakdownListRow breakdownListRow4 = destinationDetailsMass.AddRow();
				breakdownListRow4.ShowData(UI.STARMAP.REPLENISH_RATE, GameUtil.GetFormattedMass(destinationType.replishmentPerCycle, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram));
				breakdownListRow4.AddTooltip(UI.STARMAP.REPLENISH_RATE_TOOLTIP);
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (KeyValuePair<SimHashes, float> recoverableElement in selectedDestination.recoverableElements)
				{
					BreakdownListRow breakdownListRow5 = destinationDetailsComposition.AddRow();
					float num3 = selectedDestination.GetResourceValue(recoverableElement.Key, recoverableElement.Value) / num * 100f;
					Element element = ElementLoader.FindElementByHash(recoverableElement.Key);
					Tuple<Sprite, Color> uISprite = Def.GetUISprite(element);
					if (num3 <= 1f)
					{
						breakdownListRow5.ShowIconData(element.name, UI.STARMAP.COMPOSITION_SMALL_AMOUNT, uISprite.first, uISprite.second);
					}
					else
					{
						breakdownListRow5.ShowIconData(element.name, GameUtil.GetFormattedPercent(num3), uISprite.first, uISprite.second);
					}
					if (element.IsGas)
					{
						string properName = Assets.GetPrefab("GasCargoBay".ToTag()).GetProperName();
						if (currentRocketHasGasContainer)
						{
							breakdownListRow5.SetHighlighted(highlighted: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName));
						}
						else
						{
							breakdownListRow5.SetDisabled(disabled: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName));
						}
					}
					if (element.IsLiquid)
					{
						string properName2 = Assets.GetPrefab("LiquidCargoBay".ToTag()).GetProperName();
						if (currentRocketHasLiquidContainer)
						{
							breakdownListRow5.SetHighlighted(highlighted: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName2));
						}
						else
						{
							breakdownListRow5.SetDisabled(disabled: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName2));
						}
					}
					if (element.IsSolid)
					{
						string properName3 = Assets.GetPrefab("CargoBay".ToTag()).GetProperName();
						if (currentRocketHasSolidContainer)
						{
							breakdownListRow5.SetHighlighted(highlighted: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName3));
						}
						else
						{
							breakdownListRow5.SetDisabled(disabled: true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName3));
						}
					}
				}
				foreach (SpaceDestination.ResearchOpportunity researchOpportunity2 in selectedDestination.researchOpportunities)
				{
					if (!researchOpportunity2.completed && researchOpportunity2.discoveredRareResource != SimHashes.Void)
					{
						BreakdownListRow breakdownListRow6 = destinationDetailsComposition.AddRow();
						breakdownListRow6.ShowData(UI.STARMAP.COMPOSITION_UNDISCOVERED, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
						breakdownListRow6.SetDisabled(disabled: true);
						breakdownListRow6.AddTooltip(UI.STARMAP.COMPOSITION_UNDISCOVERED_TOOLTIP);
					}
				}
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (KeyValuePair<Tag, int> recoverableEntity in selectedDestination.GetRecoverableEntities())
				{
					BreakdownListRow breakdownListRow7 = destinationDetailsResources.AddRow();
					GameObject prefab = Assets.GetPrefab(recoverableEntity.Key);
					Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(prefab);
					breakdownListRow7.ShowIconData(prefab.GetProperName(), "", uISprite2.first, uISprite2.second);
					string properName4 = Assets.GetPrefab("SpecialCargoBay".ToTag()).GetProperName();
					if (currentRocketHasEntitiesContainer)
					{
						breakdownListRow7.SetHighlighted(highlighted: true);
						breakdownListRow7.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, prefab.GetProperName(), properName4));
					}
					else
					{
						breakdownListRow7.SetDisabled(disabled: true);
						breakdownListRow7.AddTooltip(string.Format(UI.STARMAP.CANT_CARRY_ELEMENT, properName4, prefab.GetProperName()));
					}
				}
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				ArtifactDropRate artifactDropTable = selectedDestination.GetDestinationType().artifactDropTable;
				foreach (Tuple<ArtifactTier, float> rate in artifactDropTable.rates)
				{
					destinationDetailsArtifacts.AddRow().ShowData(Strings.Get(rate.first.name_key), GameUtil.GetFormattedPercent(rate.second / artifactDropTable.totalWeight * 100f));
				}
			}
			destinationDetailsContainer.gameObject.SetActive(value: true);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(destinationDetailsContainer);
	}

	private void ValidateTravelAbility()
	{
		if (selectedDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete && currentCommandModule != null && currentLaunchConditionManager != null)
		{
			launchButton.ChangeState((!currentLaunchConditionManager.CheckReadyToLaunch()) ? 1 : 0);
		}
	}

	private void UpdateDistanceOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if (lcmToVisualize == null)
		{
			lcmToVisualize = currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if (lcmToVisualize != null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if (lcmToVisualize != null && spacecraft != null && spacecraft.state == Spacecraft.MissionState.Grounded)
		{
			distanceOverlay.gameObject.SetActive(value: true);
			float rocketMaxDistance = lcmToVisualize.GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance();
			rocketMaxDistance = (float)(int)(rocketMaxDistance / 10000f) * 10000f;
			Vector2 sizeDelta = distanceOverlay.rectTransform.sizeDelta;
			sizeDelta.x = rowsContiner.rect.width;
			sizeDelta.y = (1f - rocketMaxDistance / planetsMaxDistance) * rowsContiner.rect.height + (float)distanceOverlayYOffset + (float)distanceOverlayVerticalOffset;
			distanceOverlay.rectTransform.sizeDelta = sizeDelta;
			distanceOverlay.rectTransform.anchoredPosition = new Vector3(0f, distanceOverlayVerticalOffset, 0f);
		}
		else
		{
			distanceOverlay.gameObject.SetActive(value: false);
		}
	}

	private void UpdateMissionOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if (lcmToVisualize == null)
		{
			lcmToVisualize = currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if (lcmToVisualize != null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if (lcmToVisualize != null && spacecraft != null)
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcmToVisualize);
			if (spacecraftDestination == null)
			{
				Debug.Log("destination is null");
				return;
			}
			StarmapPlanet starmapPlanet = planetWidgets[spacecraftDestination];
			if (spacecraft == null)
			{
				Debug.Log("craft is null");
				return;
			}
			if (starmapPlanet == null)
			{
				Debug.Log("planet is null");
				return;
			}
			UnselectAllPlanets();
			SelectPlanet(starmapPlanet);
			starmapPlanet.ShowAsCurrentRocketDestination(spacecraftDestination.GetDestinationType().visitable);
			if (spacecraft.state != 0 && spacecraftDestination.GetDestinationType().visitable)
			{
				visualizeRocketImage.gameObject.SetActive(value: true);
				visualizeRocketTrajectory.gameObject.SetActive(value: true);
				visualizeRocketLabel.gameObject.SetActive(value: true);
				visualizeRocketProgress.gameObject.SetActive(value: true);
				float duration = spacecraft.GetDuration();
				float timeLeft = spacecraft.GetTimeLeft();
				float num = ((duration == 0f) ? 0f : (1f - timeLeft / duration));
				bool num2 = num > 0.5f;
				Vector2 vector = new Vector2(0f, 0f - rowsContiner.rect.size.y);
				Vector3 b = starmapPlanet.rectTransform().localPosition + new Vector3(starmapPlanet.rectTransform().sizeDelta.x * 0.5f, 0f, 0f);
				b = starmapPlanet.transform.parent.rectTransform().localPosition + b;
				Vector2 vector2 = new Vector2(b.x, b.y);
				float x = Vector2.Distance(vector, vector2);
				Vector2 vector3 = vector2 - vector;
				float z = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
				Vector2 v = (num2 ? new Vector2(Mathf.Lerp(vector.x, vector2.x, 1f - num * 2f + 1f), Mathf.Lerp(vector.y, vector2.y, 1f - num * 2f + 1f)) : new Vector2(Mathf.Lerp(vector.x, vector2.x, num * 2f), Mathf.Lerp(vector.y, vector2.y, num * 2f)));
				visualizeRocketLabel.text = GetTextForState(spacecraft.state).first;
				visualizeRocketProgress.text = GameUtil.GetFormattedPercent(num * 100f);
				visualizeRocketTrajectory.transform.SetLocalPosition(vector);
				visualizeRocketTrajectory.rectTransform.sizeDelta = new Vector2(x, visualizeRocketTrajectory.rectTransform.sizeDelta.y);
				visualizeRocketTrajectory.rectTransform.localRotation = Quaternion.Euler(0f, 0f, z);
				visualizeRocketImage.transform.SetLocalPosition(v);
			}
		}
		else
		{
			if (selectedDestination != null && planetWidgets.ContainsKey(selectedDestination))
			{
				UnselectAllPlanets();
				StarmapPlanet planet = planetWidgets[selectedDestination];
				SelectPlanet(planet);
			}
			else
			{
				UnselectAllPlanets();
			}
			visualizeRocketImage.gameObject.SetActive(value: false);
			visualizeRocketTrajectory.gameObject.SetActive(value: false);
			visualizeRocketLabel.gameObject.SetActive(value: false);
			visualizeRocketProgress.gameObject.SetActive(value: false);
		}
	}
}
