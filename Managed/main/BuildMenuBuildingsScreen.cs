using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBuildingsScreen : KIconToggleMenu
{
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

	private class UserData
	{
		public BuildingDef def;

		public PlanScreen.RequirementsState requirementsState;

		public UserData(BuildingDef def, PlanScreen.RequirementsState state)
		{
			this.def = def;
			requirementsState = state;
		}
	}

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	public Action<BuildingDef> onBuildingSelected;

	[SerializeField]
	private LocText titleLabel;

	[SerializeField]
	private BuildingToolTipSettings buildingToolTipSettings;

	[SerializeField]
	private LayoutElement contentSizeLayout;

	[SerializeField]
	private GridLayoutGroup gridSizer;

	[SerializeField]
	private Sprite Overlay_NeedTech;

	[SerializeField]
	private Material defaultUIMaterial;

	[SerializeField]
	private Material desaturatedUIMaterial;

	private BuildingDef selectedBuilding;

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateBuildableStates();
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
		base.onSelect += OnClickBuilding;
		Game.Instance.Subscribe(-1190690038, OnBuildToolDeactivated);
	}

	public void Configure(HashedString category, IList<BuildMenu.BuildingInfo> building_infos)
	{
		ClearButtons();
		SetHasFocus(has_focus: true);
		List<ToggleInfo> list = new List<ToggleInfo>();
		string text = HashCache.Get().Get(category).ToUpper();
		text = text.Replace(" ", "");
		titleLabel.text = Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".BUILDMENUTITLE");
		foreach (BuildMenu.BuildingInfo building_info in building_infos)
		{
			BuildingDef def = Assets.GetBuildingDef(building_info.id);
			if (def.ShowInBuildMenu && !def.Deprecated && (!def.DebugOnly || Game.Instance.DebugOnlyBuildingsAllowed))
			{
				ToggleInfo item = new ToggleInfo(def.Name, new UserData(def, PlanScreen.RequirementsState.Tech), def.HotKey, () => def.GetUISprite());
				list.Add(item);
			}
		}
		Setup(list);
		for (int i = 0; i < toggleInfo.Count; i++)
		{
			RefreshToggle(toggleInfo[i]);
		}
		int num = 0;
		foreach (Transform item2 in gridSizer.transform)
		{
			if (item2.gameObject.activeSelf)
			{
				num++;
			}
		}
		gridSizer.constraintCount = Mathf.Min(num, 3);
		int num2 = Mathf.Min(num, gridSizer.constraintCount);
		int num3 = (num + gridSizer.constraintCount - 1) / gridSizer.constraintCount;
		int num4 = num2 - 1;
		int num5 = num3 - 1;
		Vector2 vector = new Vector2((float)num2 * gridSizer.cellSize.x + (float)num4 * gridSizer.spacing.x + (float)gridSizer.padding.left + (float)gridSizer.padding.right, (float)num3 * gridSizer.cellSize.y + (float)num5 * gridSizer.spacing.y + (float)gridSizer.padding.top + (float)gridSizer.padding.bottom);
		contentSizeLayout.minWidth = vector.x;
		contentSizeLayout.minHeight = vector.y;
	}

	private void ConfigureToolTip(ToolTip tooltip, BuildingDef def)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(def.Name, buildingToolTipSettings.BuildButtonName);
		tooltip.AddMultiStringTooltip(def.Effect, buildingToolTipSettings.BuildButtonDescription);
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
		}
		ToolMenu.Instance.ClearSelection();
		DeactivateBuildTools();
		if (PlayerController.Instance.ActiveTool == PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		selectedBuilding = null;
		onBuildingSelected(selectedBuilding);
	}

	private void RefreshToggle(ToggleInfo info)
	{
		if (info == null || info.toggle == null)
		{
			return;
		}
		BuildingDef def = (info.userData as UserData).def;
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		bool flag = DebugHandler.InstantBuildMode || techItem == null || techItem.IsComplete();
		bool flag2 = flag || techItem == null || techItem.parentTech.ArePrerequisitesComplete();
		KToggle toggle = info.toggle;
		if (toggle.gameObject.activeSelf != flag2)
		{
			toggle.gameObject.SetActive(flag2);
		}
		if (toggle.bgImage == null)
		{
			return;
		}
		Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
		Sprite sprite = (image.sprite = def.GetUISprite());
		image.SetNativeSize();
		image.rectTransform().sizeDelta /= 4f;
		ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		string text = def.Name;
		string effect = def.Effect;
		if (def.HotKey != Action.NumActions)
		{
			text = GameUtil.AppendHotkeyString(text, def.HotKey);
		}
		component.AddMultiStringTooltip(text, buildingToolTipSettings.BuildButtonName);
		component.AddMultiStringTooltip(effect, buildingToolTipSettings.BuildButtonDescription);
		LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = def.Name;
		}
		PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
		ImageToggleState.State state = ((requirementsState == PlanScreen.RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		state = ((!(def == selectedBuilding) || (requirementsState != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode)) ? ((requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled) : ImageToggleState.State.Active);
		if (def == selectedBuilding && state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.Disabled;
		}
		toggle.GetComponent<ImageToggleState>().SetState(state);
		Material material;
		Color color;
		if (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode)
		{
			material = defaultUIMaterial;
			color = Color.white;
		}
		else
		{
			material = desaturatedUIMaterial;
			Color color4;
			if (!flag)
			{
				Color color3 = (image.color = new Color(1f, 1f, 1f, 0.15f));
				color4 = color3;
			}
			else
			{
				color4 = new Color(1f, 1f, 1f, 0.6f);
			}
			color = color4;
		}
		if (image.material != material)
		{
			image.material = material;
			image.color = color;
		}
		Image fgImage = toggle.gameObject.GetComponent<KToggle>().fgImage;
		fgImage.gameObject.SetActive(value: false);
		if (!flag)
		{
			fgImage.sprite = Overlay_NeedTech;
			fgImage.gameObject.SetActive(value: true);
			string newString = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
			component.AddMultiStringTooltip("\n", buildingToolTipSettings.ResearchRequirement);
			component.AddMultiStringTooltip(newString, buildingToolTipSettings.ResearchRequirement);
		}
		else
		{
			if (requirementsState == PlanScreen.RequirementsState.Complete)
			{
				return;
			}
			fgImage.gameObject.SetActive(value: false);
			component.AddMultiStringTooltip("\n", buildingToolTipSettings.ResearchRequirement);
			string newString2 = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
			component.AddMultiStringTooltip(newString2, buildingToolTipSettings.ResearchRequirement);
			foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
			{
				string newString3 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount));
				component.AddMultiStringTooltip(newString3, buildingToolTipSettings.ResearchRequirement);
			}
			component.AddMultiStringTooltip("", buildingToolTipSettings.ResearchRequirement);
		}
	}

	public void ClearUI()
	{
		Show(show: false);
		ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KToggle toggle in toggles)
		{
			toggle.gameObject.SetActive(value: false);
			toggle.gameObject.transform.SetParent(null);
			UnityEngine.Object.DestroyImmediate(toggle.gameObject);
		}
		if (toggles != null)
		{
			toggles.Clear();
		}
		if (toggleInfo != null)
		{
			toggleInfo.Clear();
		}
	}

	private void OnClickBuilding(ToggleInfo toggle_info)
	{
		UserData userData = toggle_info.userData as UserData;
		OnSelectBuilding(userData.def);
	}

	private void OnSelectBuilding(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
		if ((uint)(requirementsState - 1) <= 1u)
		{
			if (def != selectedBuilding)
			{
				selectedBuilding = def;
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
			}
			else
			{
				selectedBuilding = null;
				ClearSelection();
				CloseRecipe(playSound: true);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
			}
		}
		else
		{
			selectedBuilding = null;
			ClearSelection();
			CloseRecipe(playSound: true);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
		}
		onBuildingSelected(selectedBuilding);
	}

	public void UpdateBuildableStates()
	{
		if (toggleInfo == null || toggleInfo.Count <= 0)
		{
			return;
		}
		BuildingDef buildingDef = null;
		foreach (ToggleInfo item in toggleInfo)
		{
			RefreshToggle(item);
			UserData userData = item.userData as UserData;
			BuildingDef def = userData.def;
			if (def.Deprecated)
			{
				continue;
			}
			PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
			if (requirementsState != userData.requirementsState)
			{
				if (def == BuildMenu.Instance.SelectedBuildingDef)
				{
					buildingDef = def;
				}
				RefreshToggle(item);
				userData.requirementsState = requirementsState;
			}
		}
		if (buildingDef != null)
		{
			BuildMenu.Instance.RefreshProductInfoScreen(buildingDef);
		}
	}

	private void OnResearchComplete(object data)
	{
		UpdateBuildableStates();
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if (activeTool != null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type) || typeof(PrebuildTool).IsAssignableFrom(type))
			{
				activeTool.DeactivateTool();
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (mouseOver && base.ConsumeMouseScroll && !e.TryConsume(Action.ZoomIn))
		{
			e.TryConsume(Action.ZoomOut);
		}
		if (!HasFocus)
		{
			return;
		}
		if (e.TryConsume(Action.Escape))
		{
			Game.Instance.Trigger(288942073);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			return;
		}
		base.OnKeyDown(e);
		if (!e.Consumed)
		{
			Action action = e.GetAction();
			if (action >= Action.BUILD_MENU_START_INTERCEPT)
			{
				e.TryConsume(action);
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!HasFocus)
		{
			return;
		}
		if (selectedBuilding != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
			Game.Instance.Trigger(288942073);
			return;
		}
		base.OnKeyUp(e);
		if (!e.Consumed)
		{
			Action action = e.GetAction();
			if (action >= Action.BUILD_MENU_START_INTERCEPT)
			{
				e.TryConsume(action);
			}
		}
	}

	public override void Close()
	{
		ToolMenu.Instance.ClearSelection();
		DeactivateBuildTools();
		if (PlayerController.Instance.ActiveTool == PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		selectedBuilding = null;
		ClearButtons();
		base.gameObject.SetActive(value: false);
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if (focusIndicator != null)
		{
			focusIndicator.color = (has_focus ? focusedColour : unfocusedColour);
		}
	}

	private void OnBuildToolDeactivated(object data)
	{
		CloseRecipe();
	}
}
