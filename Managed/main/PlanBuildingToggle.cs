using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanBuildingToggle : KToggle
{
	private BuildingDef def;

	private HashedString buildingCategory;

	private TechItem techItem;

	private List<int> gameSubscriptions = new List<int>();

	private bool researchComplete;

	private Sprite sprite;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ImageToggleState imageToggleState;

	[SerializeField]
	private Image buildingIcon;

	[SerializeField]
	private Image fgIcon;

	public void Config(BuildingDef def)
	{
		this.def = def;
		techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		gameSubscriptions.Add(Game.Instance.Subscribe(-107300940, CheckResearch));
		gameSubscriptions.Add(Game.Instance.Subscribe(-1948169901, CheckResearch));
		gameSubscriptions.Add(Game.Instance.Subscribe(1557339983, CheckResearch));
		sprite = def.GetUISprite();
		base.onClick += delegate
		{
			PlanScreen.Instance.OnSelectBuilding(base.gameObject, def);
		};
		CheckResearch();
		Refresh();
	}

	protected override void OnDestroy()
	{
		if (Game.Instance != null)
		{
			foreach (int gameSubscription in gameSubscriptions)
			{
				Game.Instance.Unsubscribe(gameSubscription);
			}
		}
		gameSubscriptions.Clear();
		base.OnDestroy();
	}

	private void CheckResearch(object data = null)
	{
		researchComplete = PlanScreen.TechRequirementsMet(techItem);
	}

	public bool Refresh()
	{
		bool flag = researchComplete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool result = false;
		if (base.gameObject.activeSelf != flag)
		{
			base.gameObject.SetActive(flag);
			result = true;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return result;
		}
		if (bgImage == null)
		{
			return result;
		}
		PositionTooltip();
		RefreshLabel();
		RefreshDisplay();
		return result;
	}

	private void RefreshLabel()
	{
		if (text != null)
		{
			text.fontSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.fontSizeBigMode : PlanScreen.fontSizeStandardMode);
			text.text = def.Name;
		}
	}

	private void RefreshDisplay()
	{
		PlanScreen.RequirementsState buildableState = PlanScreen.Instance.GetBuildableState(def);
		bool flag = buildableState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool flag2 = base.gameObject == PlanScreen.Instance.SelectedBuildingGameObject;
		ImageToggleState.State state = ((buildableState == PlanScreen.RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		if (flag2 && flag)
		{
			state = ImageToggleState.State.Active;
		}
		else if (!flag2 && flag)
		{
			state = ImageToggleState.State.Inactive;
		}
		else if (flag2 && !flag)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (!flag2 && !flag)
		{
			state = ImageToggleState.State.Disabled;
		}
		imageToggleState.SetState(state);
		RefreshBuildingButtonIconAndColors(flag);
		RefreshFG(buildableState);
	}

	private void PositionTooltip()
	{
		tooltip.overrideParentObject = PlanScreen.Instance.buildingGroupsRoot;
		tooltip.tooltipPivot = Vector2.zero;
		tooltip.parentPositionAnchor = new Vector2(1f, 0f);
		tooltip.tooltipPositionOffset = (PlanScreen.Instance.ProductInfoScreen.gameObject.activeSelf ? new Vector2(16f + PlanScreen.Instance.ProductInfoScreen.rectTransform().sizeDelta.x, 0f) : new Vector2(-40f, 0f));
		tooltip.ClearMultiStringTooltip();
		string newString = def.Name;
		string effect = def.Effect;
		tooltip.AddMultiStringTooltip(newString, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
		tooltip.AddMultiStringTooltip(effect, PlanScreen.Instance.buildingToolTipSettings.BuildButtonDescription);
	}

	private void RefreshBuildingButtonIconAndColors(bool buttonAvailable)
	{
		if (sprite == null)
		{
			sprite = PlanScreen.Instance.defaultBuildingIconSprite;
		}
		buildingIcon.sprite = sprite;
		buildingIcon.SetNativeSize();
		float num = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 3.25f : 4f);
		buildingIcon.rectTransform().sizeDelta /= num;
		Material material = (buttonAvailable ? PlanScreen.Instance.defaultUIMaterial : PlanScreen.Instance.desaturatedUIMaterial);
		if (!(buildingIcon.material != material))
		{
			return;
		}
		buildingIcon.material = material;
		if (!buttonAvailable)
		{
			if (researchComplete)
			{
				buildingIcon.color = new Color(1f, 1f, 1f, 0.6f);
			}
			else
			{
				buildingIcon.color = new Color(1f, 1f, 1f, 0.15f);
			}
		}
		else
		{
			buildingIcon.color = Color.white;
		}
	}

	private void RefreshFG(PlanScreen.RequirementsState requirementsState)
	{
		if (requirementsState == PlanScreen.RequirementsState.Tech)
		{
			fgImage.sprite = PlanScreen.Instance.Overlay_NeedTech;
			fgImage.gameObject.SetActive(value: true);
		}
		else
		{
			fgImage.gameObject.SetActive(value: false);
		}
		string tooltipForRequirementsState = PlanScreen.GetTooltipForRequirementsState(def, requirementsState);
		if (tooltipForRequirementsState != null)
		{
			tooltip.AddMultiStringTooltip("\n", PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
			tooltip.AddMultiStringTooltip(tooltipForRequirementsState, PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
		}
	}
}
