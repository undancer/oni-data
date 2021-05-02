using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public struct PlanInfo
	{
		public HashedString category;

		public bool hideIfNotResearched;

		public List<string> data;

		public string RequiredDlcId;

		public PlanInfo(HashedString category, bool hideIfNotResearched, List<string> data, string RequiredDlcId = "")
		{
			this.category = category;
			this.hideIfNotResearched = hideIfNotResearched;
			this.data = data;
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

	private float timeSinceNotificationPing = 0f;

	private float notificationPingExpire = 0.5f;

	private float specialNotificationEmbellishDelay = 8f;

	private int notificationPingCount = 0;

	private GameObject selectedBuildingGameObject;

	public Transform GroupsTransform;

	public Sprite Overlay_NeedTech;

	public RectTransform buildingGroupsRoot;

	public RectTransform BuildButtonBGPanel;

	public RectTransform BuildingGroupContentsRect;

	public Sprite defaultBuildingIconSprite;

	public Material defaultUIMaterial;

	public Material desaturatedUIMaterial;

	public LocText PlanCategoryLabel;

	private List<ToggleEntry> toggleEntries = new List<ToggleEntry>();

	private int ignoreToolChangeMessages = 0;

	private Dictionary<Def, RequirementsState> _buildableStates = new Dictionary<Def, RequirementsState>();

	private Dictionary<Def, bool> _researchedDefs = new Dictionary<Def, bool>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private float initTime;

	private Dictionary<Tag, HashedString> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private Building lastSelectedBuilding;

	private int buildable_state_update_idx = 0;

	private int building_button_refresh_idx = 0;

	private float buildGrid_bg_width = 274f;

	private float buildGrid_bg_borderHeight = 32f;

	private float buildGrid_bg_rowHeight;

	private int buildGrid_maxRowsBeforeScroll = 5;

	public static PlanScreen Instance
	{
		get;
		private set;
	}

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
			productInfoScreen.rectTransform().SetLocalPosition(new Vector3(280f, 0f, 0f));
			productInfoScreen.onElementsFullySelected = OnRecipeElementsFullySelected;
			Game.Instance.Subscribe(-107300940, OnResearchComplete);
			Game.Instance.Subscribe(1174281782, OnActiveToolChanged);
		}
		buildingGroupsRoot.gameObject.SetActive(value: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
		copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.COPY_BUILDING_TOOLTIP, Action.CopyBuilding));
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
			Action action = (Action)((i < 14) ? (36 + i) : 266);
			string icon = iconNameMap[planInfo.category];
			string str = HashCache.Get().Get(planInfo.category).ToUpper();
			ToggleInfo toggleInfo = new ToggleInfo(UI.StripLinkFormatting(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + str + ".NAME")), icon, planInfo.category, action, GameUtil.ReplaceHotkeyString(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + str + ".TOOLTIP"), action));
			list.Add(toggleInfo);
			PopulateOrderInfo(planInfo.category, planInfo.data, tagCategoryMap, tagOrderMap, ref building_index);
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
			ImageToggleState[] array = components;
			foreach (ImageToggleState imageToggleState in array)
			{
				if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
				{
					imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
				}
			}
			to.GetComponent<KToggle>().soundPlayer.Enabled = false;
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
			foreach (string datum in item.data)
			{
				if (building.Def.PrefabID == datum)
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
			PopulateOrderInfo(planInfo.category, planInfo.data, category_map, order_map, ref building_index);
			return;
		}
		IList<string> list = (IList<string>)data;
		foreach (string item in list)
		{
			Tag key = new Tag(item);
			category_map[key] = category;
			order_map[key] = building_index;
			building_index++;
		}
	}

	protected override void OnCmpEnable()
	{
		Refresh();
	}

	protected override void OnCmpDisable()
	{
		ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<BuildingDef, KToggle> activeToggle in ActiveToggles)
		{
			activeToggle.Value.gameObject.SetActive(value: false);
			activeToggle.Value.transform.SetParent(null);
			UnityEngine.Object.DestroyImmediate(activeToggle.Value.gameObject);
		}
		ActiveToggles.Clear();
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
				HashedString x = (HashedString)item.userData;
				if (!(x == value))
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
							EventInstance instance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
							SoundEvent.EndOneShot(instance);
						}
					}
					string sound2 = GlobalAssets.GetSound("NewBuildable");
					if (sound2 != null)
					{
						EventInstance instance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
						instance2.setParameterByName("playCount", notificationPingCount);
						SoundEvent.EndOneShot(instance2);
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
					RequirementsState requirementsState = BuildableState(buildingDef);
					if (requirementsState == RequirementsState.Complete)
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
				foreach (ImageToggleState imageToggleState in toggleImages)
				{
					imageToggleState.SetState(state);
				}
			}
			else
			{
				toggleInfo.toggle.gameObject.SetActive(value: true);
				toggleInfo.toggle.fgImage.SetAlpha(1f);
				gameObject.gameObject.SetActive(value: false);
				ImageToggleState.State state2 = ((activeCategoryInfo == null || toggleInfo.userData != activeCategoryInfo.userData) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active);
				ImageToggleState[] toggleImages2 = toggleEntry.toggleImages;
				foreach (ImageToggleState imageToggleState2 in toggleImages2)
				{
					imageToggleState2.SetState(state2);
				}
			}
		}
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
		IOrderedEnumerable<BuildingDef> orderedEnumerable = from def in Assets.BuildingDefs
			where tagCategoryMap.ContainsKey(def.Tag) && tagCategoryMap[def.Tag] == plan_category && def.IsAvailable()
			orderby tagOrderMap[def.Tag]
			select def;
		ActiveToggles.Clear();
		int num = 0;
		string plan_category2 = plan_category.ToString();
		foreach (BuildingDef item in orderedEnumerable)
		{
			if (item.ShouldShowInBuildMenu())
			{
				CreateButton(item, parent, plan_category2, num);
				num++;
			}
		}
	}

	private void ConfigurePanelSize()
	{
		GridLayoutGroup component = GroupsTransform.GetComponent<GridLayoutGroup>();
		buildGrid_bg_rowHeight = component.cellSize.y + component.spacing.y;
		int num = GroupsTransform.childCount;
		for (int i = 0; i < GroupsTransform.childCount; i++)
		{
			if (!GroupsTransform.GetChild(i).gameObject.activeSelf)
			{
				num--;
			}
		}
		int num2 = Mathf.CeilToInt((float)num / 3f);
		BuildingGroupContentsRect.GetComponent<ScrollRect>().verticalScrollbar.gameObject.SetActive(num2 >= 4);
		buildingGroupsRoot.sizeDelta = new Vector2(buildGrid_bg_width, buildGrid_bg_borderHeight + (float)Mathf.Clamp(num2, 0, buildGrid_maxRowsBeforeScroll) * buildGrid_bg_rowHeight);
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
		return DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem == null || techItem.IsComplete();
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
		image.rectTransform().sizeDelta /= 4f;
		ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
		PositionTooltip(toggle, component);
		component.ClearMultiStringTooltip();
		string name = def.Name;
		string effect = def.Effect;
		component.AddMultiStringTooltip(name, buildingToolTipSettings.BuildButtonName);
		component.AddMultiStringTooltip(effect, buildingToolTipSettings.BuildButtonDescription);
		LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
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
		RequirementsState requirementsState2 = requirementsState;
		if (requirementsState2 == RequirementsState.Tech)
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
		string text = null;
		if (Game.Instance.SandboxModeActive)
		{
			text = string.Concat(UIConstants.ColorPrefixYellow, UI.SANDBOXTOOLS.SETTINGS.INSTANT_BUILD.NAME, UIConstants.ColorSuffix);
		}
		else if (DebugHandler.InstantBuildMode)
		{
			text = string.Concat(UIConstants.ColorPrefixYellow, UI.DEBUG_TOOLS.DEBUG_ACTIVE, UIConstants.ColorSuffix);
		}
		else
		{
			switch (state)
			{
			case RequirementsState.Tech:
				text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.ParentTech.Name);
				break;
			case RequirementsState.TelepadBuilt:
				text = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case RequirementsState.RocketInteriorOnly:
				text = UI.PRODUCTINFO_ROCKET_INTERIOR;
				break;
			case RequirementsState.RocketInteriorForbidden:
				text = UI.PRODUCTINFO_ROCKET_NOT_INTERIOR;
				break;
			case RequirementsState.UniquePerWorld:
				text = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case RequirementsState.Materials:
				text = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
				foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
				{
					string str = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount));
					text = text + "\n" + str;
				}
				break;
			}
		}
		return text;
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		tip.overrideParentObject = (productInfoScreen.gameObject.activeSelf ? productInfoScreen.rectTransform() : buildingGroupsRoot);
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if (component != null)
		{
			component.SetState(!disabled);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (!mouseOver || !base.ConsumeMouseScroll || e.TryConsume(Action.ZoomIn) || e.TryConsume(Action.ZoomOut))
		{
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
				BaseUtilityBuildTool baseUtilityBuildTool = ((buildingDef.BuildingComplete.GetComponent<Wire>() != null) ? ((BaseUtilityBuildTool)WireBuildTool.Instance) : ((BaseUtilityBuildTool)UtilityBuildTool.Instance));
				baseUtilityBuildTool.Activate(buildingDef, getSelectedElementAsList);
			}
			else
			{
				BuildTool.Instance.Activate(buildingDef, productInfoScreen.materialSelectionPanel.GetSelectedElementAsList);
			}
		}
	}

	public void OnResearchComplete(object tech)
	{
		Tech tech2 = (Tech)tech;
		foreach (TechItem unlockedItem in tech2.unlockedItems)
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
