#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
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

	private List<string> moduleButtonSortOrder = new List<string>
	{
		"CO2Engine",
		"SugarEngine",
		"HabitatModuleSmall",
		"HabitatModuleMedium",
		"NoseconeBasic",
		"OrbitalCargoModule",
		"ScoutModule",
		"PioneerModule",
		"SmallOxidizerTank",
		"SolidCargoBaySmall",
		"GasCargoBaySmall",
		"LiquidCargoBaySmall"
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
	}

	public void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		module = new_target.GetComponent<RocketModule>();
		if (module == null)
		{
			Debug.LogError("The gameObject received does not contain a RocketModule component");
			return;
		}
		launchPad = null;
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
			if ((buildCondition.IgnoreInSanboxMode() && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)) || buildCondition.EvaluateCondition((module == null) ? launchPad.gameObject : module.gameObject, def, selectionContext))
			{
				continue;
			}
			return false;
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
		LocText reference = component.GetReference<LocText>("label");
		Transform reference2 = component.GetReference<Transform>("content");
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModule>();
		foreach (string id in moduleButtonSortOrder)
		{
			GameObject part = prefabsWithComponent.Find((GameObject p) => p.PrefabID().Name == id);
			if (part == null)
			{
				Debug.LogWarning("Found an id [" + id + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!");
				continue;
			}
			GameObject gameObject2 = Util.KInstantiateUI(moduleButtonPrefab, reference2.gameObject, force_active: true);
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
			BuildingDef def = part.GetComponent<Building>().Def;
			SetupBuildingTooltip(component2.GetComponent<ToolTip>(), def);
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
		string name = def.Name;
		string text = def.Effect;
		RocketModule component = def.BuildingComplete.GetComponent<RocketModule>();
		if (component != null)
		{
			text = text + "\n\n" + UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.TITLE;
			float burden = component.performanceStats.burden;
			float fuelKilogramPerDistance = component.performanceStats.FuelKilogramPerDistance;
			float enginePower = component.performanceStats.enginePower;
			CraftModuleInterface craftModuleInterface = null;
			if (GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModule>() != null)
			{
				craftModuleInterface = GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModule>().CraftInterface;
			}
			float num;
			float num2;
			float num3;
			float num4;
			float num5;
			if (craftModuleInterface == null)
			{
				num = burden;
				num2 = fuelKilogramPerDistance;
				num3 = enginePower;
				num4 = num3 / num;
				num5 = num4;
			}
			else
			{
				num = burden + craftModuleInterface.TotalBurden;
				num2 = fuelKilogramPerDistance + craftModuleInterface.Range;
				num3 = component.performanceStats.enginePower + craftModuleInterface.EnginePower;
				num4 = (component.performanceStats.enginePower + craftModuleInterface.EnginePower) / num;
				num5 = num4 - craftModuleInterface.EnginePower / craftModuleInterface.TotalBurden;
			}
			string arg = ((burden >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, burden), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, burden));
			string arg2 = ((fuelKilogramPerDistance >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, Math.Round(fuelKilogramPerDistance, 2)), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, Math.Round(fuelKilogramPerDistance, 2)));
			string arg3 = ((enginePower >= 0f) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, enginePower), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, enginePower));
			string arg4 = ((num5 >= num4) ? GameUtil.AddPositiveSign(string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, Math.Round(num5, 2)), positive: true) : string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, Math.Round(num5, 2)));
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.BURDEN, num, arg);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.RANGE, Math.Round(num2, 2), arg2);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.ENGINEPOWER, num3, arg3);
			text = text + "\n" + string.Format(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.SPEED, Math.Round(num4, 2), arg4);
		}
		tooltip.AddMultiStringTooltip(name, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
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
			else if (Assets.GetPrefab(module.GetComponent<KPrefabID>().PrefabID()).GetComponent<ReorderableBuilding>().buildConditions.Find((SelectModuleCondition match) => match is TopOnly) != null || def.BuildingComplete.GetComponent<ReorderableBuilding>().buildConditions.Find((SelectModuleCondition match) => match is EngineOnBottom) != null)
			{
				result = SelectModuleCondition.SelectionContext.AddModuleBelow;
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
			Profiler.BeginSample("CONDITION: " + buildConditions[i].GetType().Name);
			if (buildConditions[i].IgnoreInSanboxMode() && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive))
			{
				Profiler.EndSample();
				continue;
			}
			if (!buildConditions[i].EvaluateCondition((module == null) ? launchPad.gameObject : module.gameObject, def, selectionContext))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += buildConditions[i].GetStatusTooltip(ready: false, def);
			}
			Profiler.EndSample();
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
		BuildingDef autoSelectDef = selectedModuleDef;
		GameObject gameObject = null;
		if (module != null)
		{
			GameObject gameObject2 = module.gameObject;
			gameObject = ((!addingNewModule) ? module.GetComponent<ReorderableBuilding>().ConvertModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList) : module.GetComponent<ReorderableBuilding>().AddModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList));
		}
		else
		{
			gameObject = launchPad.AddBaseModule(selectedModuleDef, materialSelectionPanel.GetSelectedElementAsList);
		}
		if (gameObject != null)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
			RocketModuleSideScreen.instance.ClickAddNew(autoSelectDef);
		}
	}
}
