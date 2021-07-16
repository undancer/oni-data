using System;
using KSerialization;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestDesignatable")]
public class HarvestDesignatable : KMonoBehaviour
{
	public bool defaultHarvestStateWhenPlanted = true;

	public OccupyArea area;

	[Serialize]
	protected bool isMarkedForHarvest;

	[Serialize]
	private bool isInPlanterBox;

	public bool showUserMenuButtons = true;

	[Serialize]
	protected bool harvestWhenReady;

	public RectTransform HarvestWhenReadyOverlayIcon;

	private Action<object> onEnableOverlayDelegate;

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnCancelDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> SetInPlanterBoxTrueDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.SetInPlanterBox(state: true);
	});

	public bool InPlanterBox => isInPlanterBox;

	public bool MarkedForHarvest
	{
		get
		{
			return isMarkedForHarvest;
		}
		set
		{
			isMarkedForHarvest = value;
		}
	}

	public bool HarvestWhenReady => harvestWhenReady;

	protected HarvestDesignatable()
	{
		onEnableOverlayDelegate = OnEnableOverlay;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1309017699, SetInPlanterBoxTrueDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		Components.HarvestDesignatables.Add(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, OnCancelDelegate);
		Game.Instance.Subscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Subscribe(1798162660, onEnableOverlayDelegate);
		Game.Instance.Subscribe(2015652040, OnDisableOverlay);
		area = GetComponent<OccupyArea>();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.HarvestDesignatables.Remove(this);
		DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Unsubscribe(2015652040, OnDisableOverlay);
		Game.Instance.Unsubscribe(1798162660, onEnableOverlayDelegate);
	}

	private void DestroyOverlayIcon()
	{
		if (HarvestWhenReadyOverlayIcon != null)
		{
			UnityEngine.Object.Destroy(HarvestWhenReadyOverlayIcon.gameObject);
			HarvestWhenReadyOverlayIcon = null;
		}
	}

	private void CreateOverlayIcon()
	{
		if (!(HarvestWhenReadyOverlayIcon != null) && GetComponent<AttackableBase>() == null)
		{
			HarvestWhenReadyOverlayIcon = Util.KInstantiate(Assets.UIPrefabs.HarvestWhenReadyOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas).GetComponent<RectTransform>();
			Extents extents = GetComponent<OccupyArea>().GetExtents();
			TransformExtensions.SetPosition(position: GetComponent<KPrefabID>().HasTag(GameTags.Hanging) ? new Vector3((float)(extents.x + extents.width / 2) + 0.5f, extents.y + extents.height) : new Vector3((float)(extents.x + extents.width / 2) + 0.5f, extents.y), transform: HarvestWhenReadyOverlayIcon.transform);
			RefreshOverlayIcon();
		}
	}

	private void OnDisableOverlay(object data)
	{
		DestroyOverlayIcon();
	}

	private void OnEnableOverlay(object data)
	{
		if ((HashedString)data == OverlayModes.Harvest.ID)
		{
			CreateOverlayIcon();
		}
		else
		{
			DestroyOverlayIcon();
		}
	}

	private void RefreshOverlayIcon(object data = null)
	{
		if (!(HarvestWhenReadyOverlayIcon != null))
		{
			return;
		}
		if (Grid.IsVisible(Grid.PosToCell(base.gameObject)) || (CameraController.Instance != null && CameraController.Instance.FreeCameraEnabled))
		{
			if (!HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
			{
				HarvestWhenReadyOverlayIcon.gameObject.SetActive(value: true);
			}
		}
		else if (HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
		{
			HarvestWhenReadyOverlayIcon.gameObject.SetActive(value: false);
		}
		HierarchyReferences component = HarvestWhenReadyOverlayIcon.GetComponent<HierarchyReferences>();
		if (harvestWhenReady)
		{
			Image obj = (Image)component.GetReference("On");
			obj.gameObject.SetActive(value: true);
			obj.color = GlobalAssets.Instance.colorSet.harvestEnabled;
			component.GetReference("Off").gameObject.SetActive(value: false);
		}
		else
		{
			component.GetReference("On").gameObject.SetActive(value: false);
			Image obj2 = (Image)component.GetReference("Off");
			obj2.gameObject.SetActive(value: true);
			obj2.color = GlobalAssets.Instance.colorSet.harvestDisabled;
		}
	}

	public bool CanBeHarvested()
	{
		Harvestable component = GetComponent<Harvestable>();
		if (component != null)
		{
			return component.CanBeHarvested;
		}
		return true;
	}

	public void SetInPlanterBox(bool state)
	{
		if (state)
		{
			if (!isInPlanterBox)
			{
				isInPlanterBox = true;
				SetHarvestWhenReady(defaultHarvestStateWhenPlanted);
			}
		}
		else
		{
			isInPlanterBox = false;
		}
	}

	public void SetHarvestWhenReady(bool state)
	{
		harvestWhenReady = state;
		if (harvestWhenReady && CanBeHarvested() && !isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		if (isMarkedForHarvest && !harvestWhenReady)
		{
			OnCancel();
			if (CanBeHarvested() && isInPlanterBox)
			{
				GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		Trigger(-266953818);
		RefreshOverlayIcon();
	}

	protected virtual void OnCancel(object data = null)
	{
	}

	public virtual void MarkForHarvest()
	{
		if (CanBeHarvested())
		{
			isMarkedForHarvest = true;
			Harvestable component = GetComponent<Harvestable>();
			if (component != null)
			{
				component.OnMarkedForHarvest();
			}
		}
	}

	protected virtual void OnClickHarvestWhenReady()
	{
		SetHarvestWhenReady(state: true);
	}

	protected virtual void OnClickCancelHarvestWhenReady()
	{
		SetHarvestWhenReady(state: false);
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		if (showUserMenuButtons)
		{
			KIconButtonMenu.ButtonInfo button = (harvestWhenReady ? new KIconButtonMenu.ButtonInfo("action_harvest", UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.NAME, delegate
			{
				OnClickCancelHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.GAMEOBJECTEFFECTS.PLANT_DO_NOT_HARVEST, base.transform);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_harvest", UI.USERMENUACTIONS.HARVEST_WHEN_READY.NAME, delegate
			{
				OnClickHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.PLANT_MARK_FOR_HARVEST, base.transform);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.HARVEST_WHEN_READY.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}
}
