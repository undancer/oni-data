using System.Collections;
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

	protected override void OnForcedCleanUp()
	{
		instance = null;
		base.OnForcedCleanUp();
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
			Vector2 vector2 = Vector2.zero;
			if (SelectModuleSideScreen.Instance != null)
			{
				vector2 = SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.rectTransform().anchoredPosition;
			}
			ClickAddNew(vector2.y);
		};
		removeModuleButton.onClick += ClickRemove;
		moveModuleUpButton.onClick += ClickSwapUp;
		moveModuleDownButton.onClick += ClickSwapDown;
		changeModuleButton.onClick += delegate
		{
			Vector2 vector = Vector2.zero;
			if (SelectModuleSideScreen.Instance != null)
			{
				vector = SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.rectTransform().anchoredPosition;
			}
			ClickChangeModule(vector.y);
		};
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
		moduleDescriptionLabel.SetText(reorderable.GetComponent<Building>().Desc);
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
				changeModuleButton.isInteractable = false;
				addNewModuleButton.isInteractable = false;
				removeModuleButton.isInteractable = false;
				moveModuleDownButton.isInteractable = false;
				moveModuleUpButton.isInteractable = false;
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

	public void ClickAddNew(float scrollViewPosition, BuildingDef autoSelectDef = null)
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.addingNewModule = true;
		selectModuleSideScreen.SetTarget(reorderable.gameObject);
		if (autoSelectDef != null)
		{
			selectModuleSideScreen.SelectModule(autoSelectDef);
		}
		ScrollToTargetPoint(scrollViewPosition);
	}

	private void ScrollToTargetPoint(float scrollViewPosition)
	{
		if (SelectModuleSideScreen.Instance != null)
		{
			SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.anchoredPosition = new Vector2(0f, scrollViewPosition);
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(DelayedScrollToTargetPoint(scrollViewPosition));
			}
		}
	}

	private IEnumerator DelayedScrollToTargetPoint(float scrollViewPosition)
	{
		if (SelectModuleSideScreen.Instance != null)
		{
			yield return new WaitForEndOfFrame();
			SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.anchoredPosition = new Vector2(0f, scrollViewPosition);
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

	private void ClickChangeModule(float scrollViewPosition)
	{
		SelectModuleSideScreen obj = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		obj.addingNewModule = false;
		obj.SetTarget(reorderable.gameObject);
		ScrollToTargetPoint(scrollViewPosition);
	}

	private void ClickViewInterior()
	{
		ClustercraftExteriorDoor component = reorderable.GetComponent<ClustercraftExteriorDoor>();
		PassengerRocketModule component2 = reorderable.GetComponent<PassengerRocketModule>();
		WorldContainer targetWorld = component.GetTargetWorld();
		WorldContainer myWorld = component.GetMyWorld();
		if (ClusterManager.Instance.activeWorld == targetWorld)
		{
			if (myWorld.id != ClusterManager.INVALID_WORLD_IDX)
			{
				AudioMixer.instance.Stop(component2.interiorReverbSnapshot);
				ClusterManager.Instance.SetActiveWorld(myWorld.id);
			}
		}
		else
		{
			AudioMixer.instance.Start(component2.interiorReverbSnapshot);
			ClusterManager.Instance.SetActiveWorld(targetWorld.id);
		}
		DetailsScreen.Instance.ClearSecondarySideScreen();
		UpdateButtonStates();
	}
}
