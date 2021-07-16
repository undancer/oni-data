using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RocketModuleSideScreen : SideScreenContent
{
	public static RocketModuleSideScreen instance;

	private ReorderableBuilding reorderable;

	public KScreen changeModuleSideScreen;

	public Image moduleIcon;

	[Header("Buttons")]
	public KButton addNewModuleButton;

	public KButton removeModuleButton;

	public KButton changeModuleButton;

	public KButton moveModuleUpButton;

	public KButton moveModuleDownButton;

	public KButton viewInteriorButton;

	[Header("Labels")]
	public LocText moduleNameLabel;

	public LocText moduleDescriptionLabel;

	public TextStyleSetting nameSetting;

	public TextStyleSetting descriptionSetting;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public override int GetSideScreenSortOrder()
	{
		return 500;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		addNewModuleButton.onClick += delegate
		{
			ClickAddNew();
		};
		removeModuleButton.onClick += ClickRemove;
		moveModuleUpButton.onClick += ClickSwapUp;
		moveModuleDownButton.onClick += ClickSwapDown;
		changeModuleButton.onClick += ClickChangeModule;
		viewInteriorButton.onClick += ClickViewInterior;
		moduleNameLabel.textStyleSetting = nameSetting;
		moduleDescriptionLabel.textStyleSetting = descriptionSetting;
		moduleNameLabel.ApplySettings();
		moduleDescriptionLabel.ApplySettings();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		DetailsScreen.Instance.ClearSecondarySideScreen();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ReorderableBuilding>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		reorderable = new_target.GetComponent<ReorderableBuilding>();
		moduleIcon.sprite = Def.GetUISprite(reorderable.gameObject).first;
		moduleNameLabel.SetText(reorderable.GetProperName());
		BuildingDef buildingDef = ((!(reorderable.GetComponent<Building>() != null)) ? reorderable.GetComponent<BuildingUnderConstruction>().Def : reorderable.GetComponent<Building>().Def);
		moduleDescriptionLabel.SetText(buildingDef.Desc);
		UpdateButtonStates();
	}

	public void UpdateButtonStates()
	{
		changeModuleButton.isInteractable = reorderable.CanChangeModule();
		changeModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(changeModuleButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.INVALID.text);
		addNewModuleButton.isInteractable = true;
		addNewModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.ADDMODULE.DESC.text);
		removeModuleButton.isInteractable = reorderable.CanRemoveModule();
		removeModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(removeModuleButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.INVALID.text);
		moveModuleDownButton.isInteractable = reorderable.CanSwapDown();
		moveModuleDownButton.GetComponent<ToolTip>().SetSimpleTooltip(moveModuleDownButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.INVALID.text);
		moveModuleUpButton.isInteractable = reorderable.CanSwapUp();
		moveModuleUpButton.GetComponent<ToolTip>().SetSimpleTooltip(moveModuleUpButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.INVALID.text);
		ClustercraftExteriorDoor component = reorderable.GetComponent<ClustercraftExteriorDoor>();
		if (component != null && component.HasTargetWorld())
		{
			if (ClusterManager.Instance.activeWorld == component.GetTargetWorld())
			{
				viewInteriorButton.isInteractable = component.GetMyWorldId() != ClusterManager.INVALID_WORLD_IDX;
				viewInteriorButton.GetComponentInChildren<LocText>().SetText(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL);
				viewInteriorButton.GetComponent<ToolTip>().SetSimpleTooltip(viewInteriorButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.INVALID.text);
			}
			else
			{
				viewInteriorButton.isInteractable = reorderable.GetComponent<PassengerRocketModule>() != null;
				viewInteriorButton.GetComponentInChildren<LocText>().SetText(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.LABEL);
				viewInteriorButton.GetComponent<ToolTip>().SetSimpleTooltip(viewInteriorButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.INVALID.text);
			}
		}
		else
		{
			viewInteriorButton.isInteractable = false;
			viewInteriorButton.GetComponentInChildren<LocText>().SetText(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.LABEL);
			viewInteriorButton.GetComponent<ToolTip>().SetSimpleTooltip(viewInteriorButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.INVALID.text);
		}
	}

	public void ClickAddNew(BuildingDef autoSelectDef = null)
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.addingNewModule = true;
		selectModuleSideScreen.SetTarget(reorderable.gameObject);
		if (autoSelectDef != null)
		{
			selectModuleSideScreen.SelectModule(autoSelectDef);
		}
	}

	private void ClickRemove()
	{
		reorderable.Trigger(-790448070);
		UpdateButtonStates();
	}

	private void ClickSwapUp()
	{
		reorderable.SwapWithAbove();
		UpdateButtonStates();
	}

	private void ClickSwapDown()
	{
		reorderable.SwapWithBelow();
		UpdateButtonStates();
	}

	private void ClickChangeModule()
	{
		SelectModuleSideScreen obj = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		obj.addingNewModule = false;
		obj.SetTarget(reorderable.gameObject);
	}

	private void ClickViewInterior()
	{
		if (ClusterManager.Instance.activeWorld == reorderable.GetComponent<ClustercraftExteriorDoor>().GetTargetWorld())
		{
			if (reorderable.GetComponent<ClustercraftExteriorDoor>().GetMyWorld().id != ClusterManager.INVALID_WORLD_IDX)
			{
				ClusterManager.Instance.SetActiveWorld(reorderable.GetComponent<ClustercraftExteriorDoor>().GetMyWorld().id);
			}
		}
		else
		{
			ClusterManager.Instance.SetActiveWorld(reorderable.GetComponent<ClustercraftExteriorDoor>().GetTargetWorld().id);
		}
		UpdateButtonStates();
	}
}
