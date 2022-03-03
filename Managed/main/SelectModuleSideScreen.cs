using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectModuleSideScreen : KScreen
{
	public RocketModule module;

	private LaunchPad launchPad;

	public GameObject mainContents;

	[Header("Category")]
	public GameObject categoryPrefab;

	public GameObject moduleButtonPrefab;

	public GameObject categoryContent;

	private BuildingDef selectedModuleDef;

	public List<GameObject> categories = new List<GameObject>();

	public Dictionary<BuildingDef, GameObject> buttons = new Dictionary<BuildingDef, GameObject>();

	private Dictionary<BuildingDef, bool> moduleBuildableState = new Dictionary<BuildingDef, bool>();

	public static SelectModuleSideScreen Instance;

	public bool addingNewModule;

	public GameObject materialSelectionPanelPrefab;

	private MaterialSelectionPanel materialSelectionPanel;

	public KButton buildSelectedModuleButton;

	public ColorStyleSetting colorStyleButton;

	public ColorStyleSetting colorStyleButtonSelected;

	public ColorStyleSetting colorStyleButtonInactive;

	public ColorStyleSetting colorStyleButtonInactiveSelected;

	private List<int> gameSubscriptionHandles = new List<int>();

	public static List<string> moduleButtonSortOrder = new List<string>
	{
		"CO2Engine", "SugarEngine", "SteamEngineCluster", "KeroseneEngineClusterSmall", "KeroseneEngineCluster", "HEPEngine", "HydrogenEngineCluster", "HabitatModuleSmall", "HabitatModuleMedium", "NoseconeBasic",
		"NoseconeHarvest", "OrbitalCargoModule", "ScoutModule", "PioneerModule", "LiquidFuelTankCluster", "SmallOxidizerTank", "OxidizerTankCluster", "OxidizerTankLiquidCluster", "SolidCargoBaySmall", "LiquidCargoBaySmall",
		"GasCargoBaySmall", "CargoBayCluster", "LiquidCargoBayCluster", "GasCargoBayCluster", "BatteryModule", "SolarPanelModule", "ArtifactCargoBay", "ScannerModule"
	};

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
		SpawnButtons();
		buildSelectedModuleButton.onClick += OnClickBuildSelectedModule;
	}

	protected override void OnCmpDisable()
	{
		ClearSubscriptionHandles();
		module = null;
		base.OnCmpDisable();
	}

	private void ClearSubscriptionHandles()
	{
		foreach (int gameSubscriptionHandle in gameSubscriptionHandles)
		{
			Game.Instance.Unsubscribe(gameSubscriptionHandle);
		}
		gameSubscriptionHandles.Clear();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		ClearSubscriptionHandles();
		gameSubscriptionHandles.Add(Game.Instance.Subscribe(-107300940, UpdateBuildableStates));
		gameSubscriptionHandles.Add(Game.Instance.Subscribe(-1948169901, UpdateBuildableStates));
	}

	protected override void OnCleanUp()
	{
		foreach (int gameSubscriptionHandle in gameSubscriptionHandles)
		{
			Game.Instance.Unsubscribe(gameSubscriptionHandle);
		}
		gameSubscriptionHandles.Clear();
		base.OnCleanUp();
	}

	public void SetLaunchPad(LaunchPad pad)
	{
		launchPad = pad;
		module = null;
		UpdateBuildableStates();
		foreach (KeyValuePair<BuildingDef, GameObject> button in buttons)
		{
			SetupBuildingTooltip(button.Value.GetComponent<ToolTip>(), button.Key);
		}
	}

	public void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		module = new_target.GetComponent<RocketModuleCluster>();
		if (module == null)
		{
			Debug.LogError("The gameObject received does not contain a RocketModuleCluster component");
			return;
		}
		launchPad = null;
		foreach (KeyValuePair<BuildingDef, GameObject> button in buttons)
		{
			SetupBuildingTooltip(button.Value.GetComponent<ToolTip>(), button.Key);
		}
		UpdateBuildableStates();
		buildSelectedModuleButton.isInteractable = false;
		if (selectedModuleDef != null)
		{
			SelectModule(selectedModuleDef);
		}
	}

	private void UpdateBuildableStates(object data = null)
	{
		foreach (KeyValuePair<BuildingDef, GameObject> button in buttons)
		{
			if (!moduleBuildableState.ContainsKey(button.Key))
			{
				moduleBuildableState.Add(button.Key, value: false);
			}
			TechItem techItem = Db.Get().TechItems.TryGet(button.Key.PrefabID);
			if (techItem != null)
			{
				bool active = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem.IsComplete();
				button.Value.SetActive(active);
			}
			else
			{
				button.Value.SetActive(value: true);
			}
			moduleBuildableState[button.Key] = TestBuildable(button.Key);
		}
		if (selectedModuleDef != null)
		{
			ConfigureMaterialSelector();
		}
		SetButtonColors();
	}

	private void OnClickBuildSelectedModule()
	{
		if (selectedModuleDef != null)
		{
			OrderBuildSelectedModule();
		}
	}

	private void ConfigureMaterialSelector()
	{
		buildSelectedModuleButton.isInteractable = false;
		if (materialSelectionPanel == null)
		{
			materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(materialSelectionPanelPrefab.gameObject, base.gameObject, force_active: true);
			materialSelectionPanel.transform.SetSiblingIndex(buildSelectedModuleButton.transform.GetSiblingIndex());
		}
		materialSelectionPanel.ClearSelectActions();
		materialSelectionPanel.ConfigureScreen(selectedModuleDef.CraftRecipe, IsDefBuildable, GetErrorTooltips);
		materialSelectionPanel.ToggleShowDescriptorPanels(show: false);
		materialSelectionPanel.AddSelectAction(UpdateBuildButton);
		materialSelectionPanel.AutoSelectAvailableMaterial();
	}

	private bool IsDefBuildable(BuildingDef def)
	{
		if (moduleBuildableState.ContainsKey(def))
		{
			return moduleBuildableState[def];
		}
		return false;
	}

	private void UpdateBuildButton()
	{
		buildSelectedModuleButton.isInteractable = materialSelectionPanel != null && materialSelectionPanel.AllSelectorsSelected() && selectedModuleDef != null && moduleBuildableState[selectedModuleDef];
	}

	public void SetButtonColors()
	{
		foreach (KeyValuePair<BuildingDef, GameObject> button in buttons)
		{
			MultiToggle component = button.Value.GetComponent<MultiToggle>();
			HierarchyReferences component2 = button.Value.GetComponent<HierarchyReferences>();
			if (!moduleBuildableState[button.Key])
			{
				component2.GetReference<Image>("FG").material = PlanScreen.Instance.desaturatedUIMaterial;
				if (button.Key == selectedModuleDef)
				{
					component.ChangeState(1);
				}
				else
				{
					component.ChangeState(0);
				}
			}
			else
			{
				component2.GetReference<Image>("FG").material = PlanScreen.Instance.defaultUIMaterial;
				if (button.Key == selectedModuleDef)
				{
					component.ChangeState(3);
				}
				else
				{
					component.ChangeState(2);
				}
			}
		}
		UpdateBuildButton();
	}

	private bool TestBuildable(BuildingDef def)
	{
		GameObject buildingComplete = def.BuildingComplete;
		SelectModuleCondition.SelectionContext selectionContext = GetSelectionContext(def);
		if (selectionContext == SelectModuleCondition.SelectionContext.AddModuleAbove && module != null)
		{
			BuildingAttachPoint component = module.GetComponent<BuildingAttachPoint>();
			if (component != null && component.points[0].attachedBuilding != null && !component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(def.HeightInCells))
			{
				return false;
			}
		}
		if (selectionContext == SelectModuleCondition.SelectionContext.AddModuleBelow && !module.GetComponent<ReorderableBuilding>().CanMoveVertically(def.HeightInCells))
		{
			return false;
		}
		if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule && module != null && def != null && module.GetComponent<Building>().Def == def)
		{
			return false;
		}
		foreach (SelectModuleCondition buildCondition in buildingComplete.GetComponent<ReorderableBuilding>().buildConditions)
		{
			if ((!buildCondition.IgnoreInSanboxMode() || (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)) && !buildCondition.EvaluateCondition((module == null) ? launchPad.gameObject : module.gameObject, def, selectionContext))
			{
				return false;
			}
		}
		return true;
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<BuildingDef, GameObject> button in buttons)
		{
			Util.KDestroyGameObject(button.Value);
		}
		for (int num = categories.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(categories[num]);
		}
		categories.Clear();
		buttons.Clear();
	}

	public void SpawnButtons(object data = null)
	{
		ClearButtons();
		GameObject gameObject = Util.KInstantiateUI(categoryPrefab, categoryContent, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		categories.Add(gameObject);
		component.GetReference<LocText>("label");
		Transform reference = component.GetReference<Transform>("content");
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModuleCluster>();
		foreach (string id in moduleButtonSortOrder)
		{
			GameObject part = prefabsWithComponent.Find((GameObject p) => p.PrefabID().Name == id);
			if (part == null)
			{
				Debug.LogWarning("Found an id [" + id + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!");
				continue;
			}
			GameObject gameObject2 = Util.KInstantiateUI(moduleButtonPrefab, reference.gameObject, force_active: true);
			gameObject2.GetComponentsInChildren<Image>()[1].sprite = Def.GetUISprite(part).first;
			LocText componentInChildren = gameObject2.GetComponentInChildren<LocText>();
			componentInChildren.text = part.GetProperName();
			componentInChildren.alignment = TextAlignmentOptions.Bottom;
			componentInChildren.enableWordWrapping = true;
			MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
			component2.onClick = (System.Action)Delegate.Combine(component2.onClick, (System.Action)delegate
			{
				SelectModule(part.GetComponent<Building>().Def);
			});
			buttons.Add(part.GetComponent<Building>().Def, gameObject2);
			if (selectedModuleDef != null)
			{
				SelectModule(selectedModuleDef);
			}
		}
		UpdateBuildableStates();
	}

	private void SetupBuildingTooltip(ToolTip tooltip, BuildingDef def)
	{
		tooltip.ClearMultiStringTooltip();
		string newString = def.Name;
		string text = def.Effect;
		RocketModuleCluster component = def.BuildingComplete.GetComponent<RocketModuleCluster>();
		BuildingDef buildingDef = ((GetSelectionContext(def) == SelectModuleCondition.SelectionContext.ReplaceModule) ? module.GetComponent<Building>().Def : null);
		if (component != null)
		{
			text = text + "\n\n" + UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.TITLE;
			float burden = component.performanceStats.burden;
			float fuelKilogramPerDistance = component.performanceStats.FuelKilogramPerDistance;
			float enginePower = component.performanceStats.enginePower;
			int heightInCells = component.GetComponent<Building>().Def.HeightInCells;
			CraftModuleInterface craftModuleInterface = null;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			if (GetComponentInParent<DetailsScreen>() != null && GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModuleCluster>() != null)
			{
				craftModuleInterface = GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModuleCluster>().CraftInterface;
			}
			int num6 = -1;
			if (craftModuleInterface != null)
			{
				num6 = craftModuleInterface.MaxHeight;
			}
			RocketEngineCluster component2 = component.GetComponent<RocketEngineCluster>();
			if (component2 != null)
			{
				num6 = component2.maxHeight;
			}
			float num7;
			if (craftModuleInterface == null)
			{
				num = burden;
				num2 = fuelKilogramPerDistance;
				num3 = enginePower;
				num4 = num3 / num;
				num7 = num4;
				num5 = heightInCells;
			}
			else
			{
				if (buildingDef != null)
				{
					RocketModulePerformance performanceStats = module.GetComponent<RocketModuleCluster>().performanceStats;
					num -= performanceStats.burden;
					num2 -= performanceStats.fuelKilogramPerDistance;
					num3 -= performanceStats.enginePower;
					num5 -= buildingDef.HeightInCells;
				}
				num = burden + craftModuleInterface.TotalBurden;
				num2 = fuelKilogramPerDistance + craftModuleInterface.Range;
				num3 = component.performanceStats.enginePower + craftModuleInterface.EnginePower;
				num4 = (component.performanceStats.enginePower + craftModuleInterface.EnginePower) / num;
				num7 = num4 - craftModuleInterface.EnginePower / craftModuleInterface.TotalBurden;
				num5 = craftModuleInterface.RocketHeight + heightInCells;
			}
			string arg = ((burden >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, burden), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, burden));
			string arg2 = ((fuelKilogramPerDistance >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, Math.Round(fuelKilogramPerDistance, 2)), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, Math.Round(fuelKilogramPerDistance, 2)));
			string arg3 = ((enginePower >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, enginePower), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, enginePower));
			string arg4 = ((num7 >= num4) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, Math.Round(num7, 3)), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, Math.Round(num7, 2)));
			string arg5 = ((heightInCells >= 0) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, heightInCells), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, heightInCells));
			text = ((num6 == -1) ? (text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.HEIGHT_NOMAX, num5, arg5)) : (text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.HEIGHT, num5, arg5, num6)));
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.BURDEN, num, arg);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.RANGE, Math.Round(num2, 2), arg2);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.ENGINEPOWER, num3, arg3);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.SPEED, Math.Round(num4, 3), arg4);
			if (component.GetComponent<RocketEngineCluster>() != null)
			{
				text = text + "\n\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.ENGINE_MAX_HEIGHT, num6);
			}
		}
		tooltip.AddMultiStringTooltip(newString, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
		tooltip.AddMultiStringTooltip(text, PlanScreen.Instance.buildingToolTipSettings.BuildButtonDescription);
		AddErrorTooltips(tooltip, def);
	}

	private SelectModuleCondition.SelectionContext GetSelectionContext(BuildingDef def)
	{
		SelectModuleCondition.SelectionContext result = SelectModuleCondition.SelectionContext.AddModuleAbove;
		if (launchPad == null)
		{
			if (!addingNewModule)
			{
				result = SelectModuleCondition.SelectionContext.ReplaceModule;
			}
			else
			{
				List<SelectModuleCondition> buildConditions = Assets.GetPrefab(module.GetComponent<KPrefabID>().PrefabID()).GetComponent<ReorderableBuilding>().buildConditions;
				ReorderableBuilding component = def.BuildingComplete.GetComponent<ReorderableBuilding>();
				if (buildConditions.Find((SelectModuleCondition match) => match is TopOnly) != null || component.buildConditions.Find((SelectModuleCondition match) => match is EngineOnBottom) != null)
				{
					result = SelectModuleCondition.SelectionContext.AddModuleBelow;
				}
			}
		}
		return result;
	}

	private string GetErrorTooltips(BuildingDef def)
	{
		List<SelectModuleCondition> buildConditions = def.BuildingComplete.GetComponent<ReorderableBuilding>().buildConditions;
		SelectModuleCondition.SelectionContext selectionContext = GetSelectionContext(def);
		string text = "";
		for (int i = 0; i < buildConditions.Count; i++)
		{
			if (buildConditions[i].IgnoreInSanboxMode() && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive))
			{
				continue;
			}
			GameObject gameObject = ((module == null) ? launchPad.gameObject : module.gameObject);
			if (!buildConditions[i].EvaluateCondition(gameObject, def, selectionContext))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += buildConditions[i].GetStatusTooltip(ready: false, gameObject, def);
			}
		}
		return text;
	}

	private void AddErrorTooltips(ToolTip tooltip, BuildingDef def, bool clearFirst = false)
	{
		if (clearFirst)
		{
			tooltip.ClearMultiStringTooltip();
		}
		if (!clearFirst)
		{
			tooltip.AddMultiStringTooltip("\n", PlanScreen.Instance.buildingToolTipSettings.MaterialRequirement);
		}
		tooltip.AddMultiStringTooltip(GetErrorTooltips(def), PlanScreen.Instance.buildingToolTipSettings.MaterialRequirement);
	}

	public void SelectModule(BuildingDef def)
	{
		selectedModuleDef = def;
		ConfigureMaterialSelector();
		SetButtonColors();
		UpdateBuildButton();
		AddErrorTooltips(buildSelectedModuleButton.GetComponent<ToolTip>(), selectedModuleDef, clearFirst: true);
	}

	private void OrderBuildSelectedModule()
	{
		BuildingDef previousSelectedDef = selectedModuleDef;
		GameObject gameObject = null;
		if (module != null)
		{
			_ = module.gameObject;
			gameObject = ((!addingNewModule) ? module.GetComponent<ReorderableBuilding>().ConvertModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList) : module.GetComponent<ReorderableBuilding>().AddModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList));
		}
		else
		{
			gameObject = launchPad.AddBaseModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList);
		}
		if (gameObject != null)
		{
			Vector2 anchoredPosition = mainContents.GetComponent<KScrollRect>().content.anchoredPosition;
			SelectTool.Instance.StartCoroutine(SelectNextFrame(gameObject.GetComponent<KSelectable>(), previousSelectedDef, anchoredPosition.y));
		}
	}

	private IEnumerator SelectNextFrame(KSelectable selectable, BuildingDef previousSelectedDef, float scrollPosition)
	{
		yield return 0;
		SelectTool.Instance.Select(selectable);
		RocketModuleSideScreen.instance.ClickAddNew(scrollPosition, previousSelectedDef);
	}
}
