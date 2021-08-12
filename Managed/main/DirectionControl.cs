using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DirectionControl")]
public class DirectionControl : KMonoBehaviour
{
	private struct DirectionInfo
	{
		public bool allowLeft;

		public bool allowRight;

		public string iconName;

		public string name;

		public string tooltip;
	}

	[Serialize]
	public WorkableReactable.AllowedDirection allowedDirection;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private DirectionInfo[] directionInfos;

	public Action<WorkableReactable.AllowedDirection> onDirectionChanged;

	private static readonly EventSystem.IntraObjectHandler<DirectionControl> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DirectionControl>(delegate(DirectionControl component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DirectionControl> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DirectionControl>(delegate(DirectionControl component, object data)
	{
		component.OnCopySettings(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		allowedDirection = WorkableReactable.AllowedDirection.Any;
		DirectionInfo[] array = new DirectionInfo[3];
		DirectionInfo directionInfo = new DirectionInfo
		{
			allowLeft = true,
			allowRight = true,
			iconName = "action_direction_both",
			name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.NAME,
			tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.TOOLTIP
		};
		array[0] = directionInfo;
		directionInfo = new DirectionInfo
		{
			allowLeft = true,
			allowRight = false,
			iconName = "action_direction_left",
			name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.NAME,
			tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.TOOLTIP
		};
		array[1] = directionInfo;
		directionInfo = new DirectionInfo
		{
			allowLeft = false,
			allowRight = true,
			iconName = "action_direction_right",
			name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.NAME,
			tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.TOOLTIP
		};
		array[2] = directionInfo;
		directionInfos = array;
		GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DirectionControl, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetAllowedDirection(allowedDirection);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void SetAllowedDirection(WorkableReactable.AllowedDirection new_direction)
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		DirectionInfo directionInfo = directionInfos[(int)new_direction];
		bool flag = directionInfo.allowLeft && directionInfo.allowRight;
		bool is_visible = !flag && directionInfo.allowLeft;
		bool is_visible2 = !flag && directionInfo.allowRight;
		component.SetSymbolVisiblity("arrow2", flag);
		component.SetSymbolVisiblity("arrow_left", is_visible);
		component.SetSymbolVisiblity("arrow_right", is_visible2);
		if (new_direction != allowedDirection)
		{
			allowedDirection = new_direction;
			if (onDirectionChanged != null)
			{
				onDirectionChanged(allowedDirection);
			}
		}
	}

	private void OnChangeWorkableDirection()
	{
		SetAllowedDirection((WorkableReactable.AllowedDirection)((int)(1 + allowedDirection) % directionInfos.Length));
	}

	private void OnCopySettings(object data)
	{
		DirectionControl component = ((GameObject)data).GetComponent<DirectionControl>();
		SetAllowedDirection(component.allowedDirection);
	}

	private void OnRefreshUserMenu(object data)
	{
		int num = (int)(1 + allowedDirection) % directionInfos.Length;
		DirectionInfo directionInfo = directionInfos[num];
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo(directionInfo.iconName, directionInfo.name, OnChangeWorkableDirection, Action.NumActions, null, null, null, directionInfo.tooltip), 0.4f);
	}
}
