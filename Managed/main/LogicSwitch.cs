using System;
using System.Collections;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicSwitch : Switch, IPlayerControlledToggle, ISim33ms
{
	public static readonly HashedString PORT_ID = "LogicSwitch";

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicSwitch> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicSwitch>(delegate(LogicSwitch component, object data)
	{
		component.OnCopySettings(data);
	});

	private bool wasOn;

	private System.Action firstFrameCallback;

	public string SideScreenTitleKey => "STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.SIDESCREEN_TITLE";

	public bool ToggleRequested
	{
		get;
		set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		wasOn = switchedOn;
		UpdateLogicCircuit();
		GetComponent<KBatchedAnimController>().Play(switchedOn ? "on" : "off");
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		LogicSwitch component = ((GameObject)data).GetComponent<LogicSwitch>();
		if (component != null && switchedOn != component.switchedOn)
		{
			switchedOn = component.switchedOn;
			UpdateVisualization();
			UpdateLogicCircuit();
		}
	}

	protected override void Toggle()
	{
		base.Toggle();
		UpdateVisualization();
		UpdateLogicCircuit();
	}

	private void UpdateVisualization()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (wasOn != switchedOn)
		{
			component.Play(switchedOn ? "on_pre" : "on_pst");
			component.Queue(switchedOn ? "on" : "off");
		}
		wasOn = switchedOn;
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(PORT_ID, switchedOn ? 1 : 0);
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSwitchStatusActive : Db.Get().BuildingStatusItems.LogicSwitchStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}

	public void Sim33ms(float dt)
	{
		if (ToggleRequested)
		{
			Toggle();
			ToggleRequested = false;
			GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, null);
		}
	}

	public void SetFirstFrameCallback(System.Action ffCb)
	{
		firstFrameCallback = ffCb;
		StartCoroutine(RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return null;
		if (firstFrameCallback != null)
		{
			firstFrameCallback();
			firstFrameCallback = null;
		}
		yield return null;
	}

	public void ToggledByPlayer()
	{
		Toggle();
	}

	public bool ToggledOn()
	{
		return switchedOn;
	}

	public KSelectable GetSelectable()
	{
		return GetComponent<KSelectable>();
	}
}
