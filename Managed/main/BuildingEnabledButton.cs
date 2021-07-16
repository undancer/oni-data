using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingEnabledButton")]
public class BuildingEnabledButton : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	[MyCmpAdd]
	private Toggleable Toggleable;

	[MyCmpReq]
	private Operational Operational;

	private int ToggleIdx;

	[Serialize]
	private bool buildingEnabled = true;

	[Serialize]
	private bool queuedToggle;

	public static readonly Operational.Flag EnabledFlag = new Operational.Flag("building_enabled", Operational.Flag.Type.Functional);

	private static readonly EventSystem.IntraObjectHandler<BuildingEnabledButton> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BuildingEnabledButton>(delegate(BuildingEnabledButton component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsEnabled
	{
		get
		{
			if (Operational != null)
			{
				return Operational.GetFlag(EnabledFlag);
			}
			return false;
		}
		set
		{
			Operational.SetFlag(EnabledFlag, value);
			Game.Instance.userMenu.Refresh(base.gameObject);
			buildingEnabled = value;
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.BuildingDisabled, !buildingEnabled);
			Trigger(1088293757, buildingEnabled);
		}
	}

	public bool WaitingForDisable
	{
		get
		{
			if (IsEnabled)
			{
				return Toggleable.IsToggleQueued(ToggleIdx);
			}
			return false;
		}
	}

	protected override void OnPrefabInit()
	{
		ToggleIdx = Toggleable.SetTarget(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		IsEnabled = buildingEnabled;
		if (queuedToggle)
		{
			OnMenuToggle();
		}
	}

	public void HandleToggle()
	{
		queuedToggle = false;
		Prioritizable.RemoveRef(base.gameObject);
		OnToggle();
	}

	public bool IsHandlerOn()
	{
		return IsEnabled;
	}

	private void OnToggle()
	{
		IsEnabled = !IsEnabled;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnMenuToggle()
	{
		if (!Toggleable.IsToggleQueued(ToggleIdx))
		{
			if (IsEnabled)
			{
				Trigger(2108245096, "BuildingDisabled");
			}
			queuedToggle = true;
			Prioritizable.AddRef(base.gameObject);
		}
		else
		{
			queuedToggle = false;
			Prioritizable.RemoveRef(base.gameObject);
		}
		Toggleable.Toggle(ToggleIdx);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		bool isEnabled = IsEnabled;
		bool flag = Toggleable.IsToggleQueued(ToggleIdx);
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		buttonInfo = (((!isEnabled || flag) && !(!isEnabled && flag)) ? new KIconButtonMenu.ButtonInfo("action_building_disabled", UI.USERMENUACTIONS.ENABLEBUILDING.NAME_OFF, OnMenuToggle, Action.ToggleEnabled, null, null, null, UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_building_disabled", UI.USERMENUACTIONS.ENABLEBUILDING.NAME, OnMenuToggle, Action.ToggleEnabled, null, null, null, UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo);
	}
}
