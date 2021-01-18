using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Switch")]
public class Switch : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	[SerializeField]
	public bool manuallyControlled = true;

	[SerializeField]
	public bool defaultState = true;

	[Serialize]
	protected bool switchedOn = true;

	[MyCmpAdd]
	private Toggleable openSwitch;

	private int openToggleIndex;

	private static readonly EventSystem.IntraObjectHandler<Switch> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Switch>(delegate(Switch component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsSwitchedOn => switchedOn;

	public event Action<bool> OnToggle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		switchedOn = defaultState;
	}

	protected override void OnSpawn()
	{
		openToggleIndex = openSwitch.SetTarget(this);
		if (this.OnToggle != null)
		{
			this.OnToggle(switchedOn);
		}
		if (manuallyControlled)
		{
			Subscribe(493375141, OnRefreshUserMenuDelegate);
		}
		UpdateSwitchStatus();
	}

	public void HandleToggle()
	{
		Toggle();
	}

	public bool IsHandlerOn()
	{
		return switchedOn;
	}

	private void OnMinionToggle()
	{
		if (!DebugHandler.InstantBuildMode)
		{
			openSwitch.Toggle(openToggleIndex);
		}
		else
		{
			Toggle();
		}
	}

	protected virtual void Toggle()
	{
		SetState(!switchedOn);
	}

	protected virtual void SetState(bool on)
	{
		if (switchedOn != on)
		{
			switchedOn = on;
			UpdateSwitchStatus();
			if (this.OnToggle != null)
			{
				this.OnToggle(switchedOn);
			}
			if (manuallyControlled)
			{
				Game.Instance.userMenu.Refresh(base.gameObject);
			}
		}
	}

	protected virtual void OnRefreshUserMenu(object data)
	{
		LocString loc_string = (switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF : BUILDINGS.PREFABS.SWITCH.TURN_ON);
		LocString loc_string2 = (switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF_TOOLTIP : BUILDINGS.PREFABS.SWITCH.TURN_ON_TOOLTIP);
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_power", loc_string, OnMinionToggle, Action.ToggleEnabled, null, null, null, loc_string2));
	}

	protected virtual void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.SwitchStatusActive : Db.Get().BuildingStatusItems.SwitchStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}
}
