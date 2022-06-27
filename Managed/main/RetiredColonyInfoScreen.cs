using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RetiredColonyInfoScreen : KModalScreen
{
	public static RetiredColonyInfoScreen Instance;

	private bool wasPixelPerfect;

	[Header("Screen")]
	[SerializeField]
	private KButton closeButton;

	[Header("Header References")]
	[SerializeField]
	private GameObject explorerHeaderContainer;

	[SerializeField]
	private GameObject colonyHeaderContainer;

	[SerializeField]
	private LocText colonyName;

	[SerializeField]
	private LocText cycleCount;

	[Header("Timelapse References")]
	[SerializeField]
	private Slideshow slideshow;

	[SerializeField]
	private GameObject worldPrefab;

	private string focusedWorld;

	private string[] currentSlideshowFiles = new string[0];

	[Header("Main Layout")]
	[SerializeField]
	private GameObject coloniesSection;

	[SerializeField]
	private GameObject achievementsSection;

	[Header("Achievement References")]
	[SerializeField]
	private GameObject achievementsContainer;

	[SerializeField]
	private GameObject achievementsPrefab;

	[SerializeField]
	private GameObject victoryAchievementsPrefab;

	[SerializeField]
	private KInputTextField achievementSearch;

	[SerializeField]
	private KButton clearAchievementSearchButton;

	[SerializeField]
	private GameObject[] achievementVeils;

	[Header("Duplicant References")]
	[SerializeField]
	private GameObject duplicantPrefab;

	[Header("Building References")]
	[SerializeField]
	private GameObject buildingPrefab;

	[Header("Colony Stat References")]
	[SerializeField]
	private GameObject statsContainer;

	[SerializeField]
	private GameObject specialMediaBlock;

	[SerializeField]
	private GameObject tallFeatureBlock;

	[SerializeField]
	private GameObject standardStatBlock;

	[SerializeField]
	private GameObject lineGraphPrefab;

	public RetiredColonyData[] retiredColonyData;

	[Header("Explorer References")]
	[SerializeField]
	private GameObject colonyScroll;

	[SerializeField]
	private GameObject explorerRoot;

	[SerializeField]
	private GameObject explorerGrid;

	[SerializeField]
	private GameObject colonyDataRoot;

	[SerializeField]
	private GameObject colonyButtonPrefab;

	[SerializeField]
	private KInputTextField explorerSearch;

	[SerializeField]
	private KButton clearExplorerSearchButton;

	[Header("Navigation Buttons")]
	[SerializeField]
	private KButton closeScreenButton;

	[SerializeField]
	private KButton viewOtherColoniesButton;

	[SerializeField]
	private KButton quitToMainMenuButton;

	[SerializeField]
	private GameObject disabledPlatformUnlocks;

	private bool explorerGridConfigured;

	private Dictionary<string, GameObject> achievementEntries = new Dictionary<string, GameObject>();

	private List<GameObject> activeColonyWidgetContainers = new List<GameObject>();

	private Dictionary<string, GameObject> activeColonyWidgets = new Dictionary<string, GameObject>();

	private const float maxAchievementWidth = 830f;

	private Canvas canvasRef;

	private Dictionary<string, Color> statColors = new Dictionary<string, Color>
	{
		{
			RetiredColonyData.DataIDs.OxygenProduced,
			new Color(0.17f, 0.91f, 0.91f, 1f)
		},
		{
			RetiredColonyData.DataIDs.OxygenConsumed,
			new Color(0.17f, 0.91f, 0.91f, 1f)
		},
		{
			RetiredColonyData.DataIDs.CaloriesProduced,
			new Color(0.24f, 0.49f, 0.32f, 1f)
		},
		{
			RetiredColonyData.DataIDs.CaloriesRemoved,
			new Color(0.24f, 0.49f, 0.32f, 1f)
		},
		{
			RetiredColonyData.DataIDs.PowerProduced,
			new Color(0.98f, 0.69f, 0.23f, 1f)
		},
		{
			RetiredColonyData.DataIDs.PowerWasted,
			new Color(0.82f, 0.3f, 0.35f, 1f)
		},
		{
			RetiredColonyData.DataIDs.WorkTime,
			new Color(0.99f, 0.51f, 0.28f, 1f)
		},
		{
			RetiredColonyData.DataIDs.TravelTime,
			new Color(0.55f, 0.55f, 0.75f, 1f)
		},
		{
			RetiredColonyData.DataIDs.AverageWorkTime,
			new Color(0.99f, 0.51f, 0.28f, 1f)
		},
		{
			RetiredColonyData.DataIDs.AverageTravelTime,
			new Color(0.55f, 0.55f, 0.75f, 1f)
		},
		{
			RetiredColonyData.DataIDs.LiveDuplicants,
			new Color(0.98f, 0.69f, 0.23f, 1f)
		},
		{
			RetiredColonyData.DataIDs.RocketsInFlight,
			new Color(0.9f, 0.9f, 0.16f, 1f)
		},
		{
			RetiredColonyData.DataIDs.AverageStressCreated,
			new Color(0.8f, 0.32f, 0.33f, 1f)
		},
		{
			RetiredColonyData.DataIDs.AverageStressRemoved,
			new Color(0.8f, 0.32f, 0.33f, 1f)
		},
		{
			RetiredColonyData.DataIDs.AverageGerms,
			new Color(0.68f, 0.79f, 0.18f, 1f)
		},
		{
			RetiredColonyData.DataIDs.DomesticatedCritters,
			new Color(0.62f, 0.31f, 0.47f, 1f)
		},
		{
			RetiredColonyData.DataIDs.WildCritters,
			new Color(0.62f, 0.31f, 0.47f, 1f)
		}
	};

	private Dictionary<string, GameObject> explorerColonyWidgets = new Dictionary<string, GameObject>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		ConfigButtons();
		LoadExplorer();
		PopulateAchievements();
		base.ConsumeMouseScroll = true;
		explorerSearch.text = "";
		explorerSearch.onValueChanged.AddListener(delegate
		{
			if (colonyDataRoot.activeSelf)
			{
				FilterColonyData(explorerSearch.text);
			}
			else
			{
				FilterExplorer(explorerSearch.text);
			}
		});
		clearExplorerSearchButton.onClick += delegate
		{
			explorerSearch.text = "";
		};
		achievementSearch.text = "";
		achievementSearch.onValueChanged.AddListener(delegate
		{
			FilterAchievements(achievementSearch.text);
		});
		clearAchievementSearchButton.onClick += delegate
		{
			achievementSearch.text = "";
		};
		RefreshUIScale();
		Subscribe(-810220474, RefreshUIScale);
	}

	private void RefreshUIScale(object data = null)
	{
		StartCoroutine(DelayedRefreshScale());
	}

	private IEnumerator DelayedRefreshScale()
	{
		for (int i = 0; i < 3; i++)
		{
			yield return 0;
		}
		float num = 36f;
		if (GameObject.Find("ScreenSpaceOverlayCanvas") != null)
		{
			explorerRoot.transform.parent.localScale = Vector3.one * ((colonyScroll.rectTransform().rect.width - num) / explorerRoot.transform.parent.rectTransform().rect.width);
		}
		else
		{
			explorerRoot.transform.parent.localScale = Vector3.one * ((colonyScroll.rectTransform().rect.width - num) / explorerRoot.transform.parent.rectTransform().rect.width);
		}
	}

	private void ConfigButtons()
	{
		closeButton.ClearOnClick();
		closeButton.onClick += delegate
		{
			Show(show: false);
		};
		viewOtherColoniesButton.ClearOnClick();
		viewOtherColoniesButton.onClick += delegate
		{
			ToggleExplorer(active: true);
		};
		quitToMainMenuButton.ClearOnClick();
		quitToMainMenuButton.onClick += delegate
		{
			ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, OnQuitConfirm);
		};
		closeScreenButton.ClearOnClick();
		closeScreenButton.onClick += delegate
		{
			Show(show: false);
		};
		viewOtherColoniesButton.gameObject.SetActive(value: false);
		if (Game.Instance != null)
		{
			closeScreenButton.gameObject.SetActive(value: true);
			closeScreenButton.GetComponentInChildren<LocText>().SetText(UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.RETURN_TO_GAME);
			quitToMainMenuButton.gameObject.SetActive(value: true);
		}
		else
		{
			closeScreenButton.gameObject.SetActive(value: true);
			closeScreenButton.GetComponentInChildren<LocText>().SetText(UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.CLOSE);
			quitToMainMenuButton.gameObject.SetActive(value: false);
		}
	}

	private void ConfirmDecision(string text, System.Action onConfirm)
	{
		base.gameObject.SetActive(value: false);
		((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject)).PopupConfirmDialog(text, onConfirm, OnCancelPopup);
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnQuitConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			Deactivate();
			PauseScreen.TriggerQuitGame();
		});
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		GetCanvasRef();
		wasPixelPerfect = canvasRef.pixelPerfect;
		canvasRef.pixelPerfect = false;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				Show(show: false);
			}
			base.OnKeyDown(e);
		}
	}

	private void GetCanvasRef()
	{
		if (base.transform.parent.GetComponent<Canvas>() != null)
		{
			canvasRef = base.transform.parent.GetComponent<Canvas>();
		}
		else
		{
			canvasRef = base.transform.parent.parent.GetComponent<Canvas>();
		}
	}

	protected override void OnCmpDisable()
	{
		canvasRef.pixelPerfect = wasPixelPerfect;
		base.OnCmpDisable();
	}

	public RetiredColonyData GetColonyDataByBaseName(string name)
	{
		name = RetireColonyUtility.StripInvalidCharacters(name);
		for (int i = 0; i < retiredColonyData.Length; i++)
		{
			if (RetireColonyUtility.StripInvalidCharacters(retiredColonyData[i].colonyName) == name)
			{
				return retiredColonyData[i];
			}
		}
		return null;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			RefreshUIScale();
		}
		if (Game.Instance != null)
		{
			if (!show)
			{
				if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
				{
					MusicManager.instance.StopSong("Music_Victory_03_StoryAndSummary");
				}
			}
			else
			{
				retiredColonyData = RetireColonyUtility.LoadRetiredColonies(skipStats: true);
				if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
				{
					MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 2f);
				}
			}
		}
		else if (Game.Instance == null)
		{
			ToggleExplorer(active: true);
		}
		disabledPlatformUnlocks.SetActive(SaveGame.Instance != null);
		if (SaveGame.Instance != null)
		{
			disabledPlatformUnlocks.GetComponent<HierarchyReferences>().GetReference("enabled").gameObject.SetActive(!DebugHandler.InstantBuildMode && !SaveGame.Instance.sandboxEnabled && !Game.Instance.debugWasUsed);
			disabledPlatformUnlocks.GetComponent<HierarchyReferences>().GetReference("disabled").gameObject.SetActive(DebugHandler.InstantBuildMode || SaveGame.Instance.sandboxEnabled || Game.Instance.debugWasUsed);
		}
	}

	public void LoadColony(RetiredColonyData data)
	{
		colonyName.text = data.colonyName.ToUpper();
		cycleCount.text = string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, data.cycleCount.ToString());
		focusedWorld = data.startWorld;
		ToggleExplorer(active: false);
		RefreshUIScale();
		if (Game.Instance == null)
		{
			viewOtherColoniesButton.gameObject.SetActive(value: true);
		}
		ClearColony();
		if (SaveGame.Instance != null)
		{
			ColonyAchievementTracker component = SaveGame.Instance.GetComponent<ColonyAchievementTracker>();
			UpdateAchievementData(data, component.achievementsToDisplay.ToArray());
			component.ClearDisplayAchievements();
			PopulateAchievementProgress(component);
		}
		else
		{
			UpdateAchievementData(data);
		}
		DisplayStatistics(data);
		colonyDataRoot.transform.parent.rectTransform().SetPosition(new Vector3(colonyDataRoot.transform.parent.rectTransform().position.x, 0f, 0f));
	}

	private void PopulateAchievementProgress(ColonyAchievementTracker tracker)
	{
		if (!(tracker != null))
		{
			return;
		}
		foreach (KeyValuePair<string, GameObject> achievementEntry in achievementEntries)
		{
			tracker.achievements.TryGetValue(achievementEntry.Key, out var value);
			if (value == null)
			{
				continue;
			}
			AchievementWidget component = achievementEntry.Value.GetComponent<AchievementWidget>();
			if (component != null)
			{
				component.ShowProgress(value);
				if (value.failed)
				{
					component.SetFailed();
				}
			}
		}
	}

	private bool LoadSlideshow(RetiredColonyData data)
	{
		clearCurrentSlideshow();
		currentSlideshowFiles = RetireColonyUtility.LoadColonySlideshowFiles(data.colonyName, focusedWorld);
		slideshow.SetFiles(currentSlideshowFiles);
		if (currentSlideshowFiles != null)
		{
			return currentSlideshowFiles.Length != 0;
		}
		return false;
	}

	private void clearCurrentSlideshow()
	{
		currentSlideshowFiles = new string[0];
	}

	private bool LoadScreenshot(RetiredColonyData data, string world)
	{
		clearCurrentSlideshow();
		Sprite sprite = RetireColonyUtility.LoadRetiredColonyPreview(data.colonyName, world);
		if (sprite != null)
		{
			slideshow.setSlide(sprite);
			CorrectTimelapseImageSize(sprite);
		}
		return sprite != null;
	}

	private void ClearColony()
	{
		foreach (GameObject activeColonyWidgetContainer in activeColonyWidgetContainers)
		{
			UnityEngine.Object.Destroy(activeColonyWidgetContainer);
		}
		activeColonyWidgetContainers.Clear();
		activeColonyWidgets.Clear();
		UpdateAchievementData(null);
	}

	private void PopulateAchievements()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			GameObject gameObject = Util.KInstantiateUI(resource.isVictoryCondition ? victoryAchievementsPrefab : achievementsPrefab, achievementsContainer, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("nameLabel").SetText(resource.Name);
			component.GetReference<LocText>("descriptionLabel").SetText(resource.description);
			if (string.IsNullOrEmpty(resource.icon) || Assets.GetSprite(resource.icon) == null)
			{
				if (Assets.GetSprite(resource.Name) != null)
				{
					component.GetReference<Image>("icon").sprite = Assets.GetSprite(resource.Name);
				}
				else
				{
					component.GetReference<Image>("icon").sprite = Assets.GetSprite("check");
				}
			}
			else
			{
				component.GetReference<Image>("icon").sprite = Assets.GetSprite(resource.icon);
			}
			if (resource.isVictoryCondition)
			{
				gameObject.transform.SetAsFirstSibling();
			}
			bool flag = !DlcManager.IsValidForVanilla(resource.dlcIds);
			component.GetReference<KImage>("dlc_overlay").gameObject.SetActive(flag);
			gameObject.GetComponent<MultiToggle>().ChangeState(2);
			gameObject.GetComponent<AchievementWidget>().dlcAchievement = flag;
			achievementEntries.Add(resource.Id, gameObject);
		}
		UpdateAchievementData(null);
	}

	private IEnumerator ClearAchievementVeil(float delay = 0f)
	{
		yield return new WaitForSecondsRealtime(delay);
		GameObject[] array;
		for (float i = 0.7f; i >= 0f; i -= Time.unscaledDeltaTime)
		{
			array = achievementVeils;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].GetComponent<Image>().color = new Color(0f, 0f, 0f, i);
			}
			yield return 0;
		}
		array = achievementVeils;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: false);
		}
	}

	private IEnumerator ShowAchievementVeil()
	{
		float targetAlpha = 0.7f;
		GameObject[] array = achievementVeils;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: true);
		}
		for (float i = 0f; i <= targetAlpha; i += Time.unscaledDeltaTime)
		{
			array = achievementVeils;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].GetComponent<Image>().color = new Color(0f, 0f, 0f, i);
			}
			yield return 0;
		}
		for (float num = 0f; num <= targetAlpha; num += Time.unscaledDeltaTime)
		{
			array = achievementVeils;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].GetComponent<Image>().color = new Color(0f, 0f, 0f, targetAlpha);
			}
		}
	}

	private void UpdateAchievementData(RetiredColonyData data, string[] newlyAchieved = null)
	{
		int num = 0;
		float num2 = 2f;
		float num3 = 1f;
		if (newlyAchieved != null && newlyAchieved.Length != 0)
		{
			retiredColonyData = RetireColonyUtility.LoadRetiredColonies(skipStats: true);
		}
		foreach (KeyValuePair<string, GameObject> achievementEntry in achievementEntries)
		{
			bool flag = false;
			bool flag2 = false;
			if (data != null)
			{
				string[] achievements = data.achievements;
				for (int i = 0; i < achievements.Length; i++)
				{
					if (achievements[i] == achievementEntry.Key)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag && data == null && retiredColonyData != null)
			{
				RetiredColonyData[] array = retiredColonyData;
				for (int i = 0; i < array.Length; i++)
				{
					string[] achievements = array[i].achievements;
					for (int j = 0; j < achievements.Length; j++)
					{
						if (achievements[j] == achievementEntry.Key)
						{
							flag2 = true;
						}
					}
				}
			}
			bool flag3 = false;
			if (newlyAchieved != null)
			{
				for (int k = 0; k < newlyAchieved.Length; k++)
				{
					if (newlyAchieved[k] == achievementEntry.Key)
					{
						flag3 = true;
					}
				}
			}
			if (flag || flag3)
			{
				if (flag3)
				{
					achievementEntry.Value.GetComponent<AchievementWidget>().ActivateNewlyAchievedFlourish(num3 + (float)num * num2);
					num++;
				}
				else
				{
					achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedNow();
				}
			}
			else if (flag2)
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedBefore();
			}
			else if (data == null)
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetNeverAchieved();
			}
			else
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetNotAchieved();
			}
		}
		if (newlyAchieved != null && newlyAchieved.Length != 0)
		{
			StartCoroutine(ShowAchievementVeil());
			StartCoroutine(ClearAchievementVeil(num3 + (float)num * num2));
		}
	}

	private void DisplayInfoBlock(RetiredColonyData data, GameObject container)
	{
		container.GetComponent<HierarchyReferences>().GetReference<LocText>("ColonyNameLabel").SetText(data.colonyName);
		container.GetComponent<HierarchyReferences>().GetReference<LocText>("CycleCountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, data.cycleCount.ToString()));
	}

	private void CorrectTimelapseImageSize(Sprite sprite)
	{
		Vector2 sizeDelta = slideshow.transform.parent.GetComponent<RectTransform>().sizeDelta;
		Vector2 fittedSize = slideshow.GetFittedSize(sprite, sizeDelta.x, sizeDelta.y);
		LayoutElement component = slideshow.GetComponent<LayoutElement>();
		if (fittedSize.y > component.preferredHeight)
		{
			component.minHeight = component.preferredHeight / (fittedSize.y / fittedSize.x);
			component.minHeight = component.preferredHeight;
		}
		else
		{
			float num2 = (component.minWidth = (component.preferredWidth = fittedSize.x));
			num2 = (component.minHeight = (component.preferredHeight = fittedSize.y));
		}
	}

	private void DisplayTimelapse(RetiredColonyData data, GameObject container)
	{
		container.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.RETIRED_COLONY_INFO_SCREEN.TITLES.TIMELAPSE);
		RectTransform reference = container.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Worlds");
		DisplayWorlds(data, reference.gameObject);
		RectTransform reference2 = container.GetComponent<HierarchyReferences>().GetReference<RectTransform>("PlayIcon");
		slideshow = container.GetComponent<HierarchyReferences>().GetReference<Slideshow>("Slideshow");
		slideshow.updateType = SlideshowUpdateType.loadOnDemand;
		slideshow.SetPaused(state: true);
		slideshow.onBeforePlay = delegate
		{
			LoadSlideshow(data);
		};
		slideshow.onEndingPlay = delegate
		{
			LoadScreenshot(data, focusedWorld);
		};
		if (!LoadScreenshot(data, focusedWorld))
		{
			slideshow.gameObject.SetActive(value: false);
			reference2.gameObject.SetActive(value: false);
		}
		else
		{
			slideshow.gameObject.SetActive(value: true);
			reference2.gameObject.SetActive(value: true);
		}
	}

	private void DisplayDuplicants(RetiredColonyData data, GameObject container, int range_min = -1, int range_max = -1)
	{
		for (int num = container.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(container.transform.GetChild(num).gameObject);
		}
		for (int i = 0; i < data.Duplicants.Length; i++)
		{
			if (i < range_min || (i > range_max && range_max != -1))
			{
				new GameObject().transform.SetParent(container.transform);
				continue;
			}
			RetiredColonyData.RetiredDuplicantData retiredDuplicantData = data.Duplicants[i];
			GameObject gameObject = Util.KInstantiateUI(duplicantPrefab, container, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("NameLabel").SetText(retiredDuplicantData.name);
			component.GetReference<LocText>("AgeLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.DUPLICANT_AGE, retiredDuplicantData.age.ToString()));
			component.GetReference<LocText>("SkillLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.SKILL_LEVEL, retiredDuplicantData.skillPointsGained.ToString()));
			SymbolOverrideController reference = component.GetReference<SymbolOverrideController>("SymbolOverrideController");
			reference.RemoveAllSymbolOverrides();
			KBatchedAnimController componentInChildren = gameObject.GetComponentInChildren<KBatchedAnimController>();
			componentInChildren.SetSymbolVisiblity("snapTo_neck", is_visible: false);
			componentInChildren.SetSymbolVisiblity("snapTo_goggles", is_visible: false);
			componentInChildren.SetSymbolVisiblity("snapTo_hat", is_visible: false);
			componentInChildren.SetSymbolVisiblity("snapTo_headfx", is_visible: false);
			componentInChildren.SetSymbolVisiblity("snapTo_hat_hair", is_visible: false);
			foreach (KeyValuePair<string, string> accessory in retiredDuplicantData.accessories)
			{
				if (Db.Get().Accessories.Exists(accessory.Value))
				{
					KAnim.Build.Symbol symbol = Db.Get().Accessories.Get(accessory.Value).symbol;
					AccessorySlot accessorySlot = Db.Get().AccessorySlots.Get(accessory.Key);
					reference.AddSymbolOverride(accessorySlot.targetSymbolId, symbol);
					gameObject.GetComponentInChildren<KBatchedAnimController>().SetSymbolVisiblity(accessory.Key, is_visible: true);
				}
			}
			reference.ApplyOverrides();
		}
		StartCoroutine(ActivatePortraitsWhenReady(container));
	}

	private IEnumerator ActivatePortraitsWhenReady(GameObject container)
	{
		yield return 0;
		if (container == null)
		{
			Debug.LogError("RetiredColonyInfoScreen minion container is null");
			yield break;
		}
		for (int i = 0; i < container.transform.childCount; i++)
		{
			KBatchedAnimController componentInChildren = container.transform.GetChild(i).GetComponentInChildren<KBatchedAnimController>();
			if (componentInChildren != null)
			{
				componentInChildren.transform.localScale = Vector3.one;
			}
		}
	}

	private void DisplayBuildings(RetiredColonyData data, GameObject container)
	{
		for (int num = container.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(container.transform.GetChild(num).gameObject);
		}
		data.buildings.Sort(delegate(Tuple<string, int> a, Tuple<string, int> b)
		{
			if (a.second > b.second)
			{
				return 1;
			}
			return (a.second != b.second) ? (-1) : 0;
		});
		data.buildings.Reverse();
		foreach (Tuple<string, int> building in data.buildings)
		{
			GameObject prefab = Assets.GetPrefab(building.first);
			if (!(prefab == null))
			{
				HierarchyReferences component = Util.KInstantiateUI(buildingPrefab, container, force_active: true).GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("NameLabel").SetText(GameUtil.ApplyBoldString(prefab.GetProperName()));
				component.GetReference<LocText>("CountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.BUILDING_COUNT, building.second.ToString()));
				Tuple<Sprite, Color> uISprite = Def.GetUISprite(prefab);
				component.GetReference<Image>("Portrait").sprite = uISprite.first;
			}
		}
	}

	private void DisplayWorlds(RetiredColonyData data, GameObject container)
	{
		container.SetActive(data.worldIdentities.Count > 0);
		for (int num = container.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(container.transform.GetChild(num).gameObject);
		}
		if (data.worldIdentities.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, string> worldPair in data.worldIdentities)
		{
			GameObject obj = Util.KInstantiateUI(worldPrefab, container, force_active: true);
			HierarchyReferences component = obj.GetComponent<HierarchyReferences>();
			ProcGen.World worldData = SettingsCache.worlds.GetWorldData(worldPair.Value);
			Sprite sprite = ((worldData != null) ? ColonyDestinationAsteroidBeltData.GetUISprite(worldData.asteroidIcon) : null);
			if (sprite != null)
			{
				component.GetReference<Image>("Portrait").sprite = sprite;
			}
			obj.GetComponent<KButton>().onClick += delegate
			{
				focusedWorld = worldPair.Key;
				LoadScreenshot(data, focusedWorld);
			};
		}
	}

	private IEnumerator ComputeSizeStatGrid()
	{
		yield return new WaitForEndOfFrame();
		GridLayoutGroup component = statsContainer.GetComponent<GridLayoutGroup>();
		component.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		component.constraintCount = ((Screen.width < 1920) ? 2 : 3);
		yield return new WaitForEndOfFrame();
		float b = base.gameObject.rectTransform().rect.width - explorerRoot.transform.parent.rectTransform().rect.width - 50f;
		b = Mathf.Min(830f, b);
		achievementsSection.GetComponent<LayoutElement>().preferredWidth = b;
	}

	private IEnumerator ComputeSizeExplorerGrid()
	{
		yield return new WaitForEndOfFrame();
		GridLayoutGroup component = explorerGrid.GetComponent<GridLayoutGroup>();
		component.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		component.constraintCount = ((Screen.width < 1920) ? 2 : 3);
		yield return new WaitForEndOfFrame();
		float b = base.gameObject.rectTransform().rect.width - explorerRoot.transform.parent.rectTransform().rect.width - 50f;
		b = Mathf.Min(830f, b);
		achievementsSection.GetComponent<LayoutElement>().preferredWidth = b;
	}

	private void DisplayStatistics(RetiredColonyData data)
	{
		GameObject gameObject = Util.KInstantiateUI(specialMediaBlock, statsContainer, force_active: true);
		activeColonyWidgetContainers.Add(gameObject);
		activeColonyWidgets.Add("timelapse", gameObject);
		DisplayTimelapse(data, gameObject);
		GameObject duplicantBlock = Util.KInstantiateUI(tallFeatureBlock, statsContainer, force_active: true);
		activeColonyWidgetContainers.Add(duplicantBlock);
		activeColonyWidgets.Add("duplicants", duplicantBlock);
		duplicantBlock.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.RETIRED_COLONY_INFO_SCREEN.TITLES.DUPLICANTS);
		PageView pageView = duplicantBlock.GetComponentInChildren<PageView>();
		pageView.OnChangePage = delegate(int page)
		{
			DisplayDuplicants(data, duplicantBlock.GetComponent<HierarchyReferences>().GetReference("Content").gameObject, page * pageView.ChildrenPerPage, (page + 1) * pageView.ChildrenPerPage);
		};
		DisplayDuplicants(data, duplicantBlock.GetComponent<HierarchyReferences>().GetReference("Content").gameObject);
		GameObject gameObject2 = Util.KInstantiateUI(tallFeatureBlock, statsContainer, force_active: true);
		activeColonyWidgetContainers.Add(gameObject2);
		activeColonyWidgets.Add("buildings", gameObject2);
		gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.RETIRED_COLONY_INFO_SCREEN.TITLES.BUILDINGS);
		DisplayBuildings(data, gameObject2.GetComponent<HierarchyReferences>().GetReference("Content").gameObject);
		int num = 2;
		for (int i = 0; i < data.Stats.Length; i += num)
		{
			GameObject gameObject3 = Util.KInstantiateUI(standardStatBlock, statsContainer, force_active: true);
			activeColonyWidgetContainers.Add(gameObject3);
			for (int j = 0; j < num; j++)
			{
				if (i + j <= data.Stats.Length - 1)
				{
					RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic = data.Stats[i + j];
					ConfigureGraph(GetStatistic(retiredColonyStatistic.id, data), gameObject3);
				}
			}
		}
		StartCoroutine(ComputeSizeStatGrid());
	}

	private void ConfigureGraph(RetiredColonyData.RetiredColonyStatistic statistic, GameObject layoutBlockGameObject)
	{
		GameObject gameObject = Util.KInstantiateUI(lineGraphPrefab, layoutBlockGameObject, force_active: true);
		activeColonyWidgets.Add(statistic.name, gameObject);
		GraphBase componentInChildren = gameObject.GetComponentInChildren<GraphBase>();
		componentInChildren.graphName = statistic.name;
		componentInChildren.label_title.SetText(componentInChildren.graphName);
		componentInChildren.axis_x.name = statistic.nameX;
		componentInChildren.axis_y.name = statistic.nameY;
		componentInChildren.label_x.SetText(componentInChildren.axis_x.name);
		componentInChildren.label_y.SetText(componentInChildren.axis_y.name);
		LineLayer componentInChildren2 = gameObject.GetComponentInChildren<LineLayer>();
		componentInChildren.axis_y.min_value = 0f;
		componentInChildren.axis_y.max_value = statistic.GetByMaxValue().second * 1.2f;
		if (float.IsNaN(componentInChildren.axis_y.max_value))
		{
			componentInChildren.axis_y.max_value = 1f;
		}
		componentInChildren.axis_x.min_value = 0f;
		componentInChildren.axis_x.max_value = statistic.GetByMaxKey().first;
		componentInChildren.axis_x.guide_frequency = (componentInChildren.axis_x.max_value - componentInChildren.axis_x.min_value) / 10f;
		componentInChildren.axis_y.guide_frequency = (componentInChildren.axis_y.max_value - componentInChildren.axis_y.min_value) / 10f;
		componentInChildren.RefreshGuides();
		Tuple<float, float>[] value = statistic.value;
		GraphedLine graphedLine = componentInChildren2.NewLine(value, statistic.id);
		if (statColors.ContainsKey(statistic.id))
		{
			componentInChildren2.line_formatting[componentInChildren2.line_formatting.Length - 1].color = statColors[statistic.id];
		}
		graphedLine.line_renderer.color = componentInChildren2.line_formatting[componentInChildren2.line_formatting.Length - 1].color;
	}

	private RetiredColonyData.RetiredColonyStatistic GetStatistic(string id, RetiredColonyData data)
	{
		RetiredColonyData.RetiredColonyStatistic[] stats = data.Stats;
		foreach (RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic in stats)
		{
			if (retiredColonyStatistic.id == id)
			{
				return retiredColonyStatistic;
			}
		}
		return null;
	}

	private void ToggleExplorer(bool active)
	{
		if (active && Game.Instance == null)
		{
			WorldGen.LoadSettings();
		}
		ConfigButtons();
		explorerRoot.SetActive(active);
		colonyDataRoot.SetActive(!active);
		if (!explorerGridConfigured)
		{
			explorerGridConfigured = true;
			StartCoroutine(ComputeSizeExplorerGrid());
		}
		explorerHeaderContainer.SetActive(active);
		colonyHeaderContainer.SetActive(!active);
		if (active)
		{
			colonyDataRoot.transform.parent.rectTransform().SetPosition(new Vector3(colonyDataRoot.transform.parent.rectTransform().position.x, 0f, 0f));
		}
		UpdateAchievementData(null);
		explorerSearch.text = "";
	}

	private void LoadExplorer()
	{
		if (SaveGame.Instance != null)
		{
			return;
		}
		ToggleExplorer(active: true);
		this.retiredColonyData = RetireColonyUtility.LoadRetiredColonies();
		RetiredColonyData[] array = this.retiredColonyData;
		foreach (RetiredColonyData retiredColonyData in array)
		{
			RetiredColonyData data = retiredColonyData;
			GameObject gameObject = Util.KInstantiateUI(colonyButtonPrefab, explorerGrid, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			Sprite sprite = RetireColonyUtility.LoadRetiredColonyPreview(RetireColonyUtility.StripInvalidCharacters(data.colonyName), data.startWorld);
			Image reference = component.GetReference<Image>("ColonyImage");
			RectTransform reference2 = component.GetReference<RectTransform>("PreviewUnavailableText");
			if (sprite != null)
			{
				reference.enabled = true;
				reference.sprite = sprite;
				reference2.gameObject.SetActive(value: false);
			}
			else
			{
				reference.enabled = false;
				reference2.gameObject.SetActive(value: true);
			}
			component.GetReference<LocText>("ColonyNameLabel").SetText(retiredColonyData.colonyName);
			component.GetReference<LocText>("CycleCountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, retiredColonyData.cycleCount.ToString()));
			component.GetReference<LocText>("DateLabel").SetText(retiredColonyData.date);
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				LoadColony(data);
			};
			string key = retiredColonyData.colonyName;
			int num = 0;
			while (explorerColonyWidgets.ContainsKey(key))
			{
				num++;
				key = retiredColonyData.colonyName + "_" + num;
			}
			explorerColonyWidgets.Add(key, gameObject);
		}
	}

	private void FilterExplorer(string search)
	{
		foreach (KeyValuePair<string, GameObject> explorerColonyWidget in explorerColonyWidgets)
		{
			if (string.IsNullOrEmpty(search) || explorerColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
			{
				explorerColonyWidget.Value.SetActive(value: true);
			}
			else
			{
				explorerColonyWidget.Value.SetActive(value: false);
			}
		}
	}

	private void FilterColonyData(string search)
	{
		foreach (KeyValuePair<string, GameObject> activeColonyWidget in activeColonyWidgets)
		{
			if (string.IsNullOrEmpty(search) || activeColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
			{
				activeColonyWidget.Value.SetActive(value: true);
			}
			else
			{
				activeColonyWidget.Value.SetActive(value: false);
			}
		}
	}

	private void FilterAchievements(string search)
	{
		foreach (KeyValuePair<string, GameObject> achievementEntry in achievementEntries)
		{
			if (string.IsNullOrEmpty(search) || Db.Get().ColonyAchievements.Get(achievementEntry.Key).Name.ToUpper().Contains(search.ToUpper()))
			{
				achievementEntry.Value.SetActive(value: true);
			}
			else
			{
				achievementEntry.Value.SetActive(value: false);
			}
		}
	}
}
