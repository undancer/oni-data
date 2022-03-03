using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public struct PlanInfo
	{
		public HashedString category;

		public bool hideIfNotResearched;

		[Obsolete("Modders: Use ModUtil.AddBuildingToPlanScreen")]
		public List<string> data;

		public List<KeyValuePair<string, string>> buildingAndSubcategoryData;

		public string RequiredDlcId;

		public PlanInfo(HashedString category, bool hideIfNotResearched, List<string> listData, string RequiredDlcId = "")
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (string listDatum in listData)
			{
				list.Add(new KeyValuePair<string, string>(listDatum, TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(listDatum) ? TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[listDatum] : "uncategorized"));
			}
			this.category = category;
			this.hideIfNotResearched = hideIfNotResearched;
			data = listData;
			buildingAndSubcategoryData = list;
			this.RequiredDlcId = RequiredDlcId;
		}
	}

	[Serializable]
	public struct BuildingToolTipSettings
	{
		public TextStyleSetting BuildButtonName;

		public TextStyleSetting BuildButtonDescription;

		public TextStyleSetting MaterialRequirement;

		public TextStyleSetting ResearchRequirement;
	}

	[Serializable]
	public struct BuildingNameTextSetting
	{
		public TextStyleSetting ActiveSelected;

		public TextStyleSetting ActiveDeselected;

		public TextStyleSetting InactiveSelected;

		public TextStyleSetting InactiveDeselected;
	}

	private class ToggleEntry
	{
		public ToggleInfo toggleInfo;

		public HashedString planCategory;

		public List<BuildingDef> buildingDefs;

		public List<Tag> pendingResearchAttentions;

		private List<TechItem> requiredTechItems;

		public ImageToggleState[] toggleImages;

		public bool hideIfNotResearched;

		private bool _areAnyRequiredTechItemsAvailable;

		public ToggleEntry(ToggleInfo toggle_info, HashedString plan_category, List<BuildingDef> building_defs, bool hideIfNotResearched)
		{
			toggleInfo = toggle_info;
			planCategory = plan_category;
			buildingDefs = building_defs;
			this.hideIfNotResearched = hideIfNotResearched;
			pendingResearchAttentions = new List<Tag>();
			requiredTechItems = new List<TechItem>();
			toggleImages = null;
			foreach (BuildingDef building_def in building_defs)
			{
				TechItem techItem = Db.Get().TechItems.TryGet(building_def.PrefabID);
				if (techItem == null)
				{
					requiredTechItems.Clear();
					break;
				}
				if (!requiredTechItems.Contains(techItem))
				{
					requiredTechItems.Add(techItem);
				}
			}
			_areAnyRequiredTechItemsAvailable = false;
			Refresh();
		}

		public bool AreAnyRequiredTechItemsAvailable()
		{
			return _areAnyRequiredTechItemsAvailable;
		}

		public void Refresh()
		{
			if (_areAnyRequiredTechItemsAvailable)
			{
				return;
			}
			if (requiredTechItems.Count == 0)
			{
				_areAnyRequiredTechItemsAvailable = true;
				return;
			}
			foreach (TechItem requiredTechItem in requiredTechItems)
			{
				if (TechRequirementsUpcoming(requiredTechItem))
				{
					_areAnyRequiredTechItemsAvailable = true;
					break;
				}
			}
		}

		public void CollectToggleImages()
		{
			toggleImages = toggleInfo.toggle.gameObject.GetComponents<ImageToggleState>();
		}
	}

	public enum RequirementsState
	{
		Invalid,
		Tech,
		Materials,
		Complete,
		TelepadBuilt,
		UniquePerWorld,
		RocketInteriorOnly,
		RocketInteriorForbidden
	}

	[SerializeField]
	private GameObject planButtonPrefab;

	[SerializeField]
	private GameObject recipeInfoScreenParent;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	[SerializeField]
	private GameObject copyBuildingButton;

	private bool USE_SUB_CATEGORY_LAYOUT;

	private int refreshScaleHandle = -1;

	[SerializeField]
	private GameObject adjacentPinnedButtons;

	private static Dictionary<HashedString, string> iconNameMap = new Dictionary<HashedString, string>
	{
		{
			CacheHashedString("Base"),
			"icon_category_base"
		},
		{
			CacheHashedString("Oxygen"),
			"icon_category_oxygen"
		},
		{
			CacheHashedString("Power"),
			"icon_category_electrical"
		},
		{
			CacheHashedString("Food"),
			"icon_category_food"
		},
		{
			CacheHashedString("Plumbing"),
			"icon_category_plumbing"
		},
		{
			CacheHashedString("HVAC"),
			"icon_category_ventilation"
		},
		{
			CacheHashedString("Refining"),
			"icon_category_refinery"
		},
		{
			CacheHashedString("Medical"),
			"icon_category_medical"
		},
		{
			CacheHashedString("Furniture"),
			"icon_category_furniture"
		},
		{
			CacheHashedString("Equipment"),
			"icon_category_misc"
		},
		{
			CacheHashedString("Utilities"),
			"icon_category_utilities"
		},
		{
			CacheHashedString("Automation"),
			"icon_category_automation"
		},
		{
			CacheHashedString("Conveyance"),
			"icon_category_shipping"
		},
		{
			CacheHashedString("Rocketry"),
			"icon_category_rocketry"
		},
		{
			CacheHashedString("HEP"),
			"icon_category_radiation"
		}
	};

	private Dictionary<ToggleInfo, bool> CategoryInteractive = new Dictionary<ToggleInfo, bool>();

	private ProductInfoScreen productInfoScreen;

	[SerializeField]
	public BuildingToolTipSettings buildingToolTipSettings;

	public BuildingNameTextSetting buildingNameTextSettings;

	private ToggleInfo activeCategoryInfo;

	public Dictionary<BuildingDef, KToggle> ActiveToggles = new Dictionary<BuildingDef, KToggle>();

	private float timeSinceNotificationPing;

	private float notificationPingExpire = 0.5f;

	private float specialNotificationEmbellishDelay = 8f;

	private int notificationPingCount;

	private Dictionary<string, GameObject> subCategoryObjects = new Dictionary<string, GameObject>();

	private static Vector2 bigBuildingButtonSize = new Vector2(98f, 123f);

	private static Vector2 standarduildingButtonSize = bigBuildingButtonSize * 0.8f;

	private static int fontSizeBigMode = 16;

	private static int fontSizeStandardMode = 14;

	private GameObject selectedBuildingGameObject;

	[SerializeField]
	private GameObject subgroupPrefab;

	public Transform GroupsTransform;

	public Sprite Overlay_NeedTech;

	public RectTransform buildingGroupsRoot;

	public RectTransform BuildButtonBGPanel;

	public RectTransform BuildingGroupContentsRect;

	public Sprite defaultBuildingIconSprite;

	private KScrollRect planScreenScrollRect;

	public Material defaultUIMaterial;

	public Material desaturatedUIMaterial;

	public LocText PlanCategoryLabel;

	private List<ToggleEntry> toggleEntries = new List<ToggleEntry>();

	private int ignoreToolChangeMessages;

	private Dictionary<Def, RequirementsState> _buildableStates = new Dictionary<Def, RequirementsState>();

	private Dictionary<Def, bool> _researchedDefs = new Dictionary<Def, bool>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private float initTime;

	private Dictionary<Tag, HashedString> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private Building lastSelectedBuilding;

	private int buildable_state_update_idx;

	private int building_button_refresh_idx;

	private float buildGrid_bg_width = 320f;

	private float buildGrid_bg_borderHeight = 48f;

	private float buildGrid_bg_rowHeight;

	public static PlanScreen Instance { get; private set; }

	public static Dictionary<HashedString, string> IconNameMap => iconNameMap;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private static HashedString CacheHashedString(string str)
	{
		return HashCache.Get().Add(str);
	}

	public override float GetSortKey()
	{
		return 2f;
	}

	private RequirementsState BuildableState(BuildingDef def)
	{
		if (def == null || !_buildableStates.TryGetValue(def, out var value))
		{
			return RequirementsState.Materials;
		}
		return value;
	}

	private bool IsDefResearched(BuildingDef def)
	{
		bool value = false;
		if (!_researchedDefs.TryGetValue(def, out value))
		{
			value = UpdateDefResearched(def);
		}
		return value;
	}

	private bool UpdateDefResearched(BuildingDef def)
	{
		return _researchedDefs[def] = Db.Get().TechItems.IsTechItemComplete(def.PrefabID);
	}

	protected override void OnPrefabInit()
	{
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.OnPrefabInit();
			Instance = this;
			productInfoScreen = Util.KInstantiateUI<ProductInfoScreen>(productInfoScreenPrefab, recipeInfoScreenParent);
			productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			productInfoScreen.rectTransform().SetLocalPosition(new Vector3(326f, 0f, 0f));
			productInfoScreen.onElementsFullySelected = OnRecipeElementsFullySelected;
			KInputManager.InputChange.AddListener(RefreshToolTip);
			planScreenScrollRect = base.transform.parent.GetComponentInParent<KScrollRect>();
			Game.Instance.Subscribe(-107300940, OnResearchComplete);
			Game.Instance.Subscribe(1174281782, OnActiveToolChanged);
		}
		buildingGroupsRoot.gameObject.SetActive(value: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.ConsumeMouseScroll = true;
		initTime = KTime.Instance.UnscaledGameTime;
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.onSelect += OnClickCategory;
			Refresh();
			foreach (KToggle toggle in toggles)
			{
				toggle.group = GetComponent<ToggleGroup>();
			}
			GetBuildableStates(force_update: true);
			Game.Instance.Subscribe(288942073, OnUIClear);
		}
		copyBuildingButton.GetComponent<MultiToggle>().onClick = delegate
		{
			OnClickCopyBuilding();
		};
		RefreshCopyBuildingButton();
		Game.Instance.Subscribe(-1503271301, RefreshCopyBuildingButton);
		Game.Instance.Subscribe(1983128072, delegate
		{
			CloseRecipe();
		});
		pointerEnterActions = (PointerEnterActions)Delegate.Combine(pointerEnterActions, new PointerEnterActions(PointerEnter));
		pointerExitActions = (PointerExitActions)Delegate.Combine(pointerExitActions, new PointerExitActions(PointerExit));
		copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.COPY_BUILDING_TOOLTIP, Action.CopyBuilding));
		RefreshScale();
		refreshScaleHandle = Game.Instance.Subscribe(-442024484, RefreshScale);
	}

	private void RefreshScale(object data = null)
	{
		GetComponent<GridLayoutGroup>().cellSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(54f, 50f) : new Vector2(45f, 45f));
		toggles.ForEach(delegate(KToggle to)
		{
			to.GetComponentInChildren<LocText>().fontSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? fontSizeBigMode : fontSizeStandardMode);
		});
		LayoutElement component = copyBuildingButton.GetComponent<LayoutElement>();
		component.minWidth = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 58 : 54);
		component.minHeight = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 58 : 54);
		base.gameObject.rectTransform().anchoredPosition = new Vector2(0f, ScreenResolutionMonitor.UsingGamepadUIMode() ? (-68) : (-74));
		adjacentPinnedButtons.GetComponent<HorizontalLayoutGroup>().padding.bottom = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 14 : 6);
		foreach (KeyValuePair<string, GameObject> subCategoryObject in subCategoryObjects)
		{
			subCategoryObject.Value.GetComponentInChildren<GridLayoutGroup>().cellSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? bigBuildingButtonSize : standarduildingButtonSize);
		}
		Vector2 sizeDelta = buildingGroupsRoot.rectTransform().sizeDelta;
		Vector2 sizeDelta2 = (ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(320f, sizeDelta.y) : new Vector2(264f, sizeDelta.y));
		buildingGroupsRoot.rectTransform().sizeDelta = sizeDelta2;
		productInfoScreen.rectTransform().anchoredPosition = new Vector2(sizeDelta2.x + 8f, productInfoScreen.rectTransform().anchoredPosition.y);
	}

	protected override void OnCleanUp()
	{
		if (Game.Instance != null)
		{
			Game.Instance.Unsubscribe(refreshScaleHandle);
		}
		base.OnCleanUp();
	}

	private void OnClickCopyBuilding()
	{
		if (!(lastSelectedBuilding == null))
		{
			Instance.CopyBuildingOrder(lastSelectedBuilding);
		}
	}

	public void RefreshCopyBuildingButton(object data = null)
	{
		adjacentPinnedButtons.rectTransform().anchoredPosition = new Vector2(Mathf.Min(base.gameObject.rectTransform().sizeDelta.x, base.transform.parent.rectTransform().rect.width), 0f);
		MultiToggle component = copyBuildingButton.GetComponent<MultiToggle>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
		{
			Building component2 = SelectTool.Instance.selected.GetComponent<Building>();
			if (component2 != null && component2.Def.ShouldShowInBuildMenu() && component2.Def.IsAvailable())
			{
				lastSelectedBuilding = component2;
			}
		}
		if (lastSelectedBuilding != null)
		{
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(lastSelectedBuilding.gameObject);
			component.gameObject.SetActive(value: true);
			component.transform.Find("FG").GetComponent<Image>().sprite = uISprite.first;
			component.transform.Find("FG").GetComponent<Image>().color = Color.white;
			component.ChangeState(1);
		}
		else
		{
			component.gameObject.SetActive(value: false);
			component.ChangeState(0);
		}
	}

	public void RefreshToolTip()
	{
		for (int i = 0; i < TUNING.BUILDINGS.PLANORDER.Count; i++)
		{
			PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER[i];
			if (DlcManager.IsContentActive(planInfo.RequiredDlcId))
			{
				Action action = ((i < 14) ? ((Action)(36 + i)) : Action.NumActions);
				string text = HashCache.Get().Get(planInfo.category).ToUpper();
				toggleInfo[i].tooltip = GameUtil.ReplaceHotkeyString(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text + ".TOOLTIP"), action);
			}
		}
		copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.COPY_BUILDING_TOOLTIP, Action.CopyBuilding));
	}

	public void Refresh()
	{
		List<ToggleInfo> list = new List<ToggleInfo>();
		if (tagCategoryMap != null)
		{
			return;
		}
		int building_index = 0;
		tagCategoryMap = new Dictionary<Tag, HashedString>();
		tagOrderMap = new Dictionary<Tag, int>();
		if (TUNING.BUILDINGS.PLANORDER.Count > 14)
		{
			DebugUtil.LogWarningArgs("Insufficient keys to cover root plan menu", "Max of 14 keys supported but TUNING.BUILDINGS.PLANORDER has " + TUNING.BUILDINGS.PLANORDER.Count);
		}
		toggleEntries.Clear();
		for (int i = 0; i < TUNING.BUILDINGS.PLANORDER.Count; i++)
		{
			PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER[i];
			if (!DlcManager.IsContentActive(planInfo.RequiredDlcId))
			{
				continue;
			}
			Action action = ((i < 14) ? ((Action)(36 + i)) : Action.NumActions);
			string icon = iconNameMap[planInfo.category];
			string text = HashCache.Get().Get(planInfo.category).ToUpper();
			ToggleInfo toggleInfo = new ToggleInfo(UI.StripLinkFormatting(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text + ".NAME")), icon, planInfo.category, action, GameUtil.ReplaceHotkeyString(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text + ".TOOLTIP"), action));
			list.Add(toggleInfo);
			PopulateOrderInfo(planInfo.category, planInfo.buildingAndSubcategoryData, tagCategoryMap, tagOrderMap, ref building_index);
			List<BuildingDef> list2 = new List<BuildingDef>();
			foreach (BuildingDef buildingDef in Assets.BuildingDefs)
			{
				if (buildingDef.IsAvailable() && tagCategoryMap.TryGetValue(buildingDef.Tag, out var value) && !(value != planInfo.category))
				{
					list2.Add(buildingDef);
				}
			}
			toggleEntries.Add(new ToggleEntry(toggleInfo, planInfo.category, list2, planInfo.hideIfNotResearched));
		}
		Setup(list);
		toggles.ForEach(delegate(KToggle to)
		{
			ImageToggleState[] components = to.GetComponents<ImageToggleState>();
			foreach (ImageToggleState imageToggleState in components)
			{
				if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
				{
					imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
				}
			}
			to.GetComponent<KToggle>().soundPlayer.Enabled = false;
			to.GetComponentInChildren<LocText>().fontSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? fontSizeBigMode : fontSizeStandardMode);
		});
		for (int j = 0; j < toggleEntries.Count; j++)
		{
			ToggleEntry toggleEntry = toggleEntries[j];
			toggleEntry.CollectToggleImages();
			toggleEntries[j] = toggleEntry;
		}
	}

	public void CopyBuildingOrder(Building building)
	{
		foreach (PlanInfo item in TUNING.BUILDINGS.PLANORDER)
		{
			foreach (KeyValuePair<string, string> buildingAndSubcategoryDatum in item.buildingAndSubcategoryData)
			{
				if (building.Def.PrefabID == buildingAndSubcategoryDatum.Key)
				{
					OpenCategoryByName(HashCache.Get().Get(item.category));
					OnSelectBuilding(ActiveToggles[building.Def].gameObject, building.Def);
					productInfoScreen.materialSelectionPanel.SelectSourcesMaterials(building);
					Rotatable component = building.GetComponent<Rotatable>();
					if (component != null)
					{
						BuildTool.Instance.SetToolOrientation(component.GetOrientation());
					}
					break;
				}
			}
		}
	}

	private static void PopulateOrderInfo(HashedString category, object data, Dictionary<Tag, HashedString> category_map, Dictionary<Tag, int> order_map, ref int building_index)
	{
		if (data.GetType() == typeof(PlanInfo))
		{
			PlanInfo planInfo = (PlanInfo)data;
			PopulateOrderInfo(planInfo.category, planInfo.buildingAndSubcategoryData, category_map, order_map, ref building_index);
			return;
		}
		foreach (KeyValuePair<string, string> item in (List<KeyValuePair<string, string>>)data)
		{
			Tag key = new Tag(item.Key);
			category_map[key] = category;
			order_map[key] = building_index;
			building_index++;
		}
	}

	protected override void OnCmpEnable()
	{
		Refresh();
		RefreshCopyBuildingButton();
	}

	protected override void OnCmpDisable()
	{
		ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<string, GameObject> subCategoryObject in subCategoryObjects)
		{
			UnityEngine.Object.DestroyImmediate(subCategoryObject.Value.gameObject);
		}
		subCategoryObjects.Clear();
		ActiveToggles.Clear();
		copyBuildingButton.gameObject.SetActive(value: false);
		copyBuildingButton.GetComponent<MultiToggle>().ChangeState(0);
	}

	public void OnSelectBuilding(GameObject button_go, BuildingDef def)
	{
		if (button_go == null)
		{
			Debug.Log("Button gameObject is null", base.gameObject);
			return;
		}
		if (button_go == selectedBuildingGameObject)
		{
			CloseRecipe(playSound: true);
			return;
		}
		ignoreToolChangeMessages++;
		selectedBuildingGameObject = button_go;
		currentlySelectedToggle = button_go.GetComponent<KToggle>();
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
		HashedString category = tagCategoryMap[def.Tag];
		if (GetToggleEntryForCategory(category, out var toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
		{
			toggleEntry.pendingResearchAttentions.Remove(def.Tag);
			button_go.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: false);
			if (toggleEntry.pendingResearchAttentions.Count == 0)
			{
				toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: false);
			}
		}
		productInfoScreen.ClearProduct(deactivateTool: false);
		ToolMenu.Instance.ClearSelection();
		PrebuildTool.Instance.Activate(def, GetTooltipForBuildable(def));
		lastSelectedBuilding = def.BuildingComplete.GetComponent<Building>();
		RefreshCopyBuildingButton();
		productInfoScreen.Show();
		productInfoScreen.ConfigureScreen(def);
		ignoreToolChangeMessages--;
	}

	private void GetBuildableStates(bool force_update)
	{
		if (Assets.BuildingDefs == null || Assets.BuildingDefs.Count == 0)
		{
			return;
		}
		if (timeSinceNotificationPing < specialNotificationEmbellishDelay)
		{
			timeSinceNotificationPing += Time.unscaledDeltaTime;
		}
		if (timeSinceNotificationPing >= notificationPingExpire)
		{
			notificationPingCount = 0;
		}
		int num = 10;
		if (force_update)
		{
			num = Assets.BuildingDefs.Count;
			buildable_state_update_idx = 0;
		}
		ListPool<HashedString, PlanScreen>.PooledList pooledList = ListPool<HashedString, PlanScreen>.Allocate();
		for (int i = 0; i < num; i++)
		{
			buildable_state_update_idx = (buildable_state_update_idx + 1) % Assets.BuildingDefs.Count;
			BuildingDef buildingDef = Assets.BuildingDefs[buildable_state_update_idx];
			RequirementsState buildableStateForDef = GetBuildableStateForDef(buildingDef);
			if (!tagCategoryMap.TryGetValue(buildingDef.Tag, out var value) || (_buildableStates.ContainsKey(buildingDef) && _buildableStates[buildingDef] == buildableStateForDef))
			{
				continue;
			}
			_buildableStates[buildingDef] = buildableStateForDef;
			if (productInfoScreen.currentDef == buildingDef)
			{
				ignoreToolChangeMessages++;
				productInfoScreen.ClearProduct(deactivateTool: false);
				productInfoScreen.Show();
				productInfoScreen.ConfigureScreen(buildingDef);
				ignoreToolChangeMessages--;
			}
			if (buildableStateForDef != RequirementsState.Complete)
			{
				continue;
			}
			foreach (ToggleInfo item in toggleInfo)
			{
				if (!((HashedString)item.userData == value))
				{
					continue;
				}
				Bouncer component = item.toggle.GetComponent<Bouncer>();
				if (!(component != null) || component.IsBouncing() || pooledList.Contains(value))
				{
					continue;
				}
				pooledList.Add(value);
				component.Bounce();
				if (KTime.Instance.UnscaledGameTime - initTime > 1.5f)
				{
					if (timeSinceNotificationPing >= specialNotificationEmbellishDelay)
					{
						string sound = GlobalAssets.GetSound("NewBuildable_Embellishment");
						if (sound != null)
						{
							SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition()));
						}
					}
					string sound2 = GlobalAssets.GetSound("NewBuildable");
					if (sound2 != null)
					{
						EventInstance instance = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
						instance.setParameterByName("playCount", notificationPingCount);
						SoundEvent.EndOneShot(instance);
					}
				}
				timeSinceNotificationPing = 0f;
				notificationPingCount++;
			}
		}
		pooledList.Recycle();
	}

	private RequirementsState GetBuildableStateForDef(BuildingDef def)
	{
		if (!def.IsAvailable())
		{
			return RequirementsState.Invalid;
		}
		RequirementsState result = RequirementsState.Complete;
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !IsDefResearched(def))
		{
			result = RequirementsState.Tech;
		}
		else if (def.BuildingComplete.GetComponent<Telepad>() != null && ClusterUtil.ActiveWorldHasPrinter())
		{
			result = RequirementsState.TelepadBuilt;
		}
		else if (def.BuildingComplete.HasTag(GameTags.RocketInteriorBuilding) && !ClusterUtil.ActiveWorldIsRocketInterior())
		{
			result = RequirementsState.RocketInteriorOnly;
		}
		else if (def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && ClusterUtil.ActiveWorldIsRocketInterior())
		{
			result = RequirementsState.RocketInteriorForbidden;
		}
		else if (def.BuildingComplete.HasTag(GameTags.UniquePerWorld) && BuildingInventory.Instance.BuildingCountForWorld_BAD_PERF(def.Tag, ClusterManager.Instance.activeWorldId) > 0)
		{
			result = RequirementsState.UniquePerWorld;
		}
		else if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
		{
			result = RequirementsState.Materials;
		}
		return result;
	}

	private void SetCategoryButtonState()
	{
		foreach (ToggleEntry toggleEntry in toggleEntries)
		{
			ToggleInfo toggleInfo = toggleEntry.toggleInfo;
			toggleInfo.toggle.ActivateFlourish(activeCategoryInfo != null && toggleInfo.userData == activeCategoryInfo.userData);
			bool flag = false;
			bool flag2 = true;
			if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				foreach (BuildingDef buildingDef in toggleEntry.buildingDefs)
				{
					if (BuildableState(buildingDef) == RequirementsState.Complete)
					{
						flag = true;
						flag2 = false;
						break;
					}
				}
				if (flag2 && toggleEntry.AreAnyRequiredTechItemsAvailable())
				{
					flag2 = false;
				}
			}
			CategoryInteractive[toggleInfo] = !flag2;
			GameObject gameObject = toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
			if (!flag)
			{
				if (flag2 && toggleEntry.hideIfNotResearched)
				{
					toggleInfo.toggle.gameObject.SetActive(value: false);
				}
				else if (flag2)
				{
					toggleInfo.toggle.gameObject.SetActive(value: true);
					toggleInfo.toggle.fgImage.SetAlpha(64f / 255f);
					gameObject.gameObject.SetActive(value: true);
				}
				else
				{
					toggleInfo.toggle.gameObject.SetActive(value: true);
					toggleInfo.toggle.fgImage.SetAlpha(1f);
					gameObject.gameObject.SetActive(value: false);
				}
				ImageToggleState.State state = ((activeCategoryInfo != null && toggleInfo.userData == activeCategoryInfo.userData) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Disabled);
				ImageToggleState[] toggleImages = toggleEntry.toggleImages;
				for (int i = 0; i < toggleImages.Length; i++)
				{
					toggleImages[i].SetState(state);
				}
			}
			else
			{
				toggleInfo.toggle.gameObject.SetActive(value: true);
				toggleInfo.toggle.fgImage.SetAlpha(1f);
				gameObject.gameObject.SetActive(value: false);
				ImageToggleState.State state2 = ((activeCategoryInfo == null || toggleInfo.userData != activeCategoryInfo.userData) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active);
				ImageToggleState[] toggleImages = toggleEntry.toggleImages;
				for (int i = 0; i < toggleImages.Length; i++)
				{
					toggleImages[i].SetState(state2);
				}
			}
		}
		RefreshCopyBuildingButton();
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if (activeTool != null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type) || type == typeof(PrebuildTool))
			{
				activeTool.DeactivateTool();
				PlayerController.Instance.ActivateTool(SelectTool.Instance);
			}
		}
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
		}
		if (PlayerController.Instance.ActiveTool is PrebuildTool || PlayerController.Instance.ActiveTool is BuildTool)
		{
			ToolMenu.Instance.ClearSelection();
		}
		DeactivateBuildTools();
		if (productInfoScreen != null)
		{
			productInfoScreen.ClearProduct();
		}
		if (activeCategoryInfo != null)
		{
			UpdateBuildingButtonList(activeCategoryInfo);
		}
		selectedBuildingGameObject = null;
	}

	private void CloseCategoryPanel(bool playSound = true)
	{
		activeCategoryInfo = null;
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
		}
		buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Collapse(delegate
		{
			ClearButtons();
			buildingGroupsRoot.gameObject.SetActive(value: false);
		});
		PlanCategoryLabel.text = "";
	}

	private void OnClickCategory(ToggleInfo toggle_info)
	{
		CloseRecipe();
		if (!CategoryInteractive.ContainsKey(toggle_info) || !CategoryInteractive[toggle_info])
		{
			CloseCategoryPanel(playSound: false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
			return;
		}
		if (activeCategoryInfo == toggle_info)
		{
			CloseCategoryPanel();
		}
		else
		{
			OpenCategoryPanel(toggle_info);
		}
		ConfigurePanelSize();
		SetScrollPoint(0f);
	}

	private void OpenCategoryPanel(ToggleInfo toggle_info, bool play_sound = true)
	{
		HashedString plan_category = (HashedString)toggle_info.userData;
		ClearButtons();
		buildingGroupsRoot.gameObject.SetActive(value: true);
		activeCategoryInfo = toggle_info;
		if (play_sound)
		{
			UISounds.PlaySound(UISounds.Sound.ClickObject);
		}
		BuildButtonList(plan_category, GroupsTransform.gameObject);
		PlanCategoryLabel.text = activeCategoryInfo.text.ToUpper();
		buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Expand(null);
	}

	public void OpenCategoryByName(string category)
	{
		if (GetToggleEntryForCategory(category, out var toggleEntry))
		{
			OpenCategoryPanel(toggleEntry.toggleInfo, play_sound: false);
		}
	}

	private void UpdateBuildingButtonList(ToggleInfo toggle_info)
	{
		KToggle toggle = toggle_info.toggle;
		if (toggle == null)
		{
			foreach (ToggleInfo item in toggleInfo)
			{
				if (item.userData == toggle_info.userData)
				{
					toggle = item.toggle;
				}
			}
		}
		int num = 2;
		if (toggle != null && ActiveToggles.Count != 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (building_button_refresh_idx >= ActiveToggles.Count)
				{
					building_button_refresh_idx = 0;
				}
				RefreshBuildingButton(ActiveToggles.ElementAt(building_button_refresh_idx).Key, ActiveToggles.ElementAt(building_button_refresh_idx).Value, (HashedString)toggle_info.userData);
				building_button_refresh_idx++;
			}
		}
		if (productInfoScreen.gameObject.activeSelf)
		{
			productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		GetBuildableStates(force_update: false);
		SetCategoryButtonState();
		if (activeCategoryInfo != null)
		{
			UpdateBuildingButtonList(activeCategoryInfo);
		}
	}

	private void BuildButtonList(HashedString plan_category, GameObject parent)
	{
		ActiveToggles.Clear();
		int num = 0;
		string plan_category2 = plan_category.ToString();
		Dictionary<string, List<BuildingDef>> dictionary = new Dictionary<string, List<BuildingDef>>();
		foreach (KeyValuePair<string, string> buildingAndSubcategoryDatum in TUNING.BUILDINGS.PLANORDER.Find((PlanInfo match) => match.category == plan_category).buildingAndSubcategoryData)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(buildingAndSubcategoryDatum.Key);
			if (!buildingDef.IsAvailable() || !buildingDef.ShowInBuildMenu)
			{
				continue;
			}
			if (USE_SUB_CATEGORY_LAYOUT)
			{
				if (!dictionary.ContainsKey(buildingAndSubcategoryDatum.Value))
				{
					dictionary.Add(buildingAndSubcategoryDatum.Value, new List<BuildingDef>());
				}
				dictionary[buildingAndSubcategoryDatum.Value].Add(buildingDef);
			}
			else
			{
				if (!dictionary.ContainsKey("default"))
				{
					dictionary.Add("default", new List<BuildingDef>());
				}
				dictionary["default"].Add(buildingDef);
			}
		}
		subCategoryObjects.Clear();
		foreach (KeyValuePair<string, List<BuildingDef>> item in dictionary)
		{
			subCategoryObjects.Add(item.Key, Util.KInstantiateUI(subgroupPrefab, parent, force_active: true));
			GameObject parent2 = subCategoryObjects[item.Key].GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid").gameObject;
			subCategoryObjects[item.Key].GetComponent<HierarchyReferences>().GetReference<RectTransform>("Header").gameObject.SetActive(USE_SUB_CATEGORY_LAYOUT);
			foreach (BuildingDef item2 in item.Value)
			{
				CreateButton(item2, parent2, plan_category2, num);
				num++;
			}
		}
		RefreshScale();
	}

	private void ConfigurePanelSize()
	{
		buildGrid_bg_rowHeight = (ScreenResolutionMonitor.UsingGamepadUIMode() ? bigBuildingButtonSize.y : standarduildingButtonSize.y);
		GridLayoutGroup reference = subgroupPrefab.GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid");
		buildGrid_bg_rowHeight += reference.spacing.y;
		int num = 0;
		for (int i = 0; i < GroupsTransform.childCount; i++)
		{
			int num2 = 0;
			HierarchyReferences component = GroupsTransform.GetChild(i).GetComponent<HierarchyReferences>();
			if (component == null)
			{
				continue;
			}
			GridLayoutGroup reference2 = component.GetReference<GridLayoutGroup>("Grid");
			if (reference2 == null)
			{
				continue;
			}
			for (int j = 0; j < reference2.transform.childCount; j++)
			{
				if (reference2.transform.GetChild(j).gameObject.activeSelf)
				{
					num2++;
				}
			}
			num += num2 / reference2.constraintCount;
			if (num2 % reference2.constraintCount != 0)
			{
				num++;
			}
		}
		int num3 = num;
		int val = Math.Max(1, Screen.height / (int)buildGrid_bg_rowHeight - 3);
		val = Math.Min(val, 6);
		BuildingGroupContentsRect.GetComponent<ScrollRect>().verticalScrollbar.gameObject.SetActive(num3 >= val - 1);
		float num4 = buildGrid_bg_borderHeight + (float)Mathf.Clamp(num3, 0, val) * buildGrid_bg_rowHeight;
		if (USE_SUB_CATEGORY_LAYOUT)
		{
			float minHeight = subgroupPrefab.GetComponent<HierarchyReferences>().GetReference("HeaderLabel").transform.parent.GetComponent<LayoutElement>().minHeight;
			num4 += minHeight;
		}
		buildingGroupsRoot.sizeDelta = new Vector2(buildGrid_bg_width, num4);
		RefreshScale();
	}

	private void SetScrollPoint(float targetY)
	{
		BuildingGroupContentsRect.anchoredPosition = new Vector2(BuildingGroupContentsRect.anchoredPosition.x, targetY);
	}

	private GameObject CreateButton(BuildingDef def, GameObject parent, string plan_category, int btnIndex)
	{
		GameObject button_go = Util.KInstantiateUI(planButtonPrefab, parent, force_active: true);
		button_go.name = UI.StripLinkFormatting(def.name) + " Group:" + plan_category;
		KToggle componentInChildren = button_go.GetComponentInChildren<KToggle>();
		componentInChildren.soundPlayer.Enabled = false;
		ActiveToggles.Add(def, componentInChildren);
		RefreshBuildingButton(def, componentInChildren, plan_category);
		componentInChildren.onClick += delegate
		{
			OnSelectBuilding(button_go, def);
		};
		return button_go;
	}

	private static bool TechRequirementsMet(TechItem techItem)
	{
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && techItem != null)
		{
			return techItem.IsComplete();
		}
		return true;
	}

	private static bool TechRequirementsUpcoming(TechItem techItem)
	{
		return TechRequirementsMet(techItem);
	}

	private bool GetToggleEntryForCategory(HashedString category, out ToggleEntry toggleEntry)
	{
		toggleEntry = null;
		foreach (ToggleEntry toggleEntry2 in toggleEntries)
		{
			if (toggleEntry2.planCategory == category)
			{
				toggleEntry = toggleEntry2;
				return true;
			}
		}
		return false;
	}

	public void RefreshBuildingButton(BuildingDef def, KToggle toggle, HashedString buildingCategory)
	{
		if (toggle == null)
		{
			return;
		}
		if (GetToggleEntryForCategory(buildingCategory, out var toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
		{
			toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: true);
		}
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		bool flag = TechRequirementsMet(techItem);
		bool flag2 = TechRequirementsUpcoming(techItem);
		if (toggle.gameObject.activeSelf != flag2)
		{
			toggle.gameObject.SetActive(flag2);
			ConfigurePanelSize();
			SetScrollPoint(0f);
		}
		if (!toggle.gameObject.activeInHierarchy || toggle.bgImage == null)
		{
			return;
		}
		Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
		Sprite uISprite = def.GetUISprite();
		if (uISprite == null)
		{
			uISprite = defaultBuildingIconSprite;
		}
		image.sprite = uISprite;
		image.SetNativeSize();
		float num = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 3.25f : 4f);
		image.rectTransform().sizeDelta /= num;
		ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
		PositionTooltip(toggle, component);
		component.ClearMultiStringTooltip();
		string newString = def.Name;
		string effect = def.Effect;
		component.AddMultiStringTooltip(newString, buildingToolTipSettings.BuildButtonName);
		component.AddMultiStringTooltip(effect, buildingToolTipSettings.BuildButtonDescription);
		LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.fontSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? fontSizeBigMode : fontSizeStandardMode);
			componentInChildren.text = def.Name;
		}
		RequirementsState requirementsState = BuildableState(def);
		bool flag3 = requirementsState == RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool flag4 = toggle.gameObject == selectedBuildingGameObject;
		ImageToggleState.State state = ((requirementsState == RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		if (flag4 && flag3)
		{
			state = ImageToggleState.State.Active;
		}
		else if (!flag4 && flag3)
		{
			state = ImageToggleState.State.Inactive;
		}
		else if (flag4 && !flag3)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (!flag4 && !flag3)
		{
			state = ImageToggleState.State.Disabled;
		}
		toggle.GetComponent<ImageToggleState>().SetState(state);
		Material material = (flag3 ? defaultUIMaterial : desaturatedUIMaterial);
		if (image.material != material)
		{
			image.material = material;
			if (!flag3)
			{
				if (flag)
				{
					image.color = new Color(1f, 1f, 1f, 0.6f);
				}
				else
				{
					image.color = new Color(1f, 1f, 1f, 0.15f);
				}
			}
			else
			{
				image.color = Color.white;
			}
		}
		Image fgImage = toggle.gameObject.GetComponent<KToggle>().fgImage;
		if (requirementsState == RequirementsState.Tech)
		{
			fgImage.sprite = Overlay_NeedTech;
			fgImage.gameObject.SetActive(value: true);
		}
		else
		{
			fgImage.gameObject.SetActive(value: false);
		}
		string tooltipForRequirementsState = GetTooltipForRequirementsState(def, requirementsState);
		if (tooltipForRequirementsState != null)
		{
			component.AddMultiStringTooltip("\n", buildingToolTipSettings.ResearchRequirement);
			component.AddMultiStringTooltip(tooltipForRequirementsState, buildingToolTipSettings.ResearchRequirement);
		}
	}

	public bool IsDefBuildable(BuildingDef def)
	{
		return BuildableState(def) == RequirementsState.Complete;
	}

	public string GetTooltipForBuildable(BuildingDef def)
	{
		RequirementsState state = BuildableState(def);
		return GetTooltipForRequirementsState(def, state);
	}

	public static string GetTooltipForRequirementsState(BuildingDef def, RequirementsState state)
	{
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		string result = null;
		if (Game.Instance.SandboxModeActive)
		{
			result = string.Concat(UIConstants.ColorPrefixYellow, UI.SANDBOXTOOLS.SETTINGS.INSTANT_BUILD.NAME, UIConstants.ColorSuffix);
		}
		else if (DebugHandler.InstantBuildMode)
		{
			result = string.Concat(UIConstants.ColorPrefixYellow, UI.DEBUG_TOOLS.DEBUG_ACTIVE, UIConstants.ColorSuffix);
		}
		else
		{
			switch (state)
			{
			case RequirementsState.Tech:
				result = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.ParentTech.Name);
				break;
			case RequirementsState.TelepadBuilt:
				result = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case RequirementsState.RocketInteriorOnly:
				result = UI.PRODUCTINFO_ROCKET_INTERIOR;
				break;
			case RequirementsState.RocketInteriorForbidden:
				result = UI.PRODUCTINFO_ROCKET_NOT_INTERIOR;
				break;
			case RequirementsState.UniquePerWorld:
				result = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case RequirementsState.Materials:
				result = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
				{
					foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
					{
						string text = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount));
						result = result + "\n" + text;
					}
					return result;
				}
			}
		}
		return result;
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		tip.overrideParentObject = buildingGroupsRoot;
		tip.tooltipPivot = Vector2.zero;
		tip.parentPositionAnchor = new Vector2(1f, 0f);
		tip.tooltipPositionOffset = (productInfoScreen.gameObject.activeSelf ? new Vector2(16f + productInfoScreen.rectTransform().sizeDelta.x, 0f) : new Vector2(-40f, 0f));
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if (component != null)
		{
			component.SetState(!disabled);
		}
	}

	private void PointerEnter(PointerEventData data)
	{
		planScreenScrollRect.mouseIsOver = true;
	}

	private void PointerExit(PointerEventData data)
	{
		planScreenScrollRect.mouseIsOver = false;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (mouseOver && base.ConsumeMouseScroll)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				if (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut))
				{
					planScreenScrollRect.OnKeyDown(e);
				}
			}
			else if (!e.TryConsume(Action.ZoomIn))
			{
				e.TryConsume(Action.ZoomOut);
			}
		}
		if (e.IsAction(Action.CopyBuilding) && e.TryConsume(Action.CopyBuilding))
		{
			OnClickCopyBuilding();
		}
		if (toggles != null)
		{
			if (!e.Consumed && activeCategoryInfo != null && e.TryConsume(Action.Escape))
			{
				OnClickCategory(activeCategoryInfo);
				SelectTool.Instance.Activate();
				ClearSelection();
			}
			else if (!e.Consumed)
			{
				base.OnKeyDown(e);
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (mouseOver && base.ConsumeMouseScroll)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				if (e.IsAction(Action.ZoomIn) || e.IsAction(Action.ZoomOut))
				{
					planScreenScrollRect.OnKeyUp(e);
				}
			}
			else if (!e.TryConsume(Action.ZoomIn))
			{
				e.TryConsume(Action.ZoomOut);
			}
		}
		if (!e.Consumed)
		{
			if (selectedBuildingGameObject != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				CloseRecipe();
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			}
			else if (activeCategoryInfo != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				OnUIClear(null);
			}
			if (!e.Consumed)
			{
				base.OnKeyUp(e);
			}
		}
	}

	private void OnRecipeElementsFullySelected()
	{
		BuildingDef buildingDef = null;
		foreach (KeyValuePair<BuildingDef, KToggle> activeToggle in ActiveToggles)
		{
			if (activeToggle.Value == currentlySelectedToggle)
			{
				buildingDef = activeToggle.Key;
				break;
			}
		}
		DebugUtil.DevAssert(buildingDef, "def is null");
		if ((bool)buildingDef)
		{
			if (buildingDef.isKAnimTile && buildingDef.isUtility)
			{
				IList<Tag> getSelectedElementAsList = productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
				((buildingDef.BuildingComplete.GetComponent<Wire>() != null) ? ((BaseUtilityBuildTool)WireBuildTool.Instance) : ((BaseUtilityBuildTool)UtilityBuildTool.Instance)).Activate(buildingDef, getSelectedElementAsList);
			}
			else
			{
				BuildTool.Instance.Activate(buildingDef, productInfoScreen.materialSelectionPanel.GetSelectedElementAsList);
			}
		}
	}

	public void OnResearchComplete(object tech)
	{
		foreach (TechItem unlockedItem in ((Tech)tech).unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(unlockedItem.Id);
			if (!(buildingDef != null))
			{
				continue;
			}
			UpdateDefResearched(buildingDef);
			if (tagCategoryMap.ContainsKey(buildingDef.Tag))
			{
				HashedString category = tagCategoryMap[buildingDef.Tag];
				if (GetToggleEntryForCategory(category, out var toggleEntry))
				{
					toggleEntry.pendingResearchAttentions.Add(buildingDef.Tag);
					toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: true);
					toggleEntry.Refresh();
				}
			}
		}
	}

	private void OnUIClear(object data)
	{
		if (activeCategoryInfo != null)
		{
			selected = -1;
			OnClickCategory(activeCategoryInfo);
			SelectTool.Instance.Activate();
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
			SelectTool.Instance.Select(null, skipSound: true);
		}
	}

	private void OnActiveToolChanged(object data)
	{
		if (data != null && ignoreToolChangeMessages <= 0)
		{
			Type type = data.GetType();
			if (!typeof(BuildTool).IsAssignableFrom(type) && !typeof(PrebuildTool).IsAssignableFrom(type) && !typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
			{
				CloseRecipe();
				CloseCategoryPanel(playSound: false);
			}
		}
	}

	public PrioritySetting GetBuildingPriority()
	{
		return productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();
	}
}
