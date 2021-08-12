using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CircuitSwitch : Switch, IPlayerControlledToggle, ISim33ms
{
	[SerializeField]
	public ObjectLayer objectLayer;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<CircuitSwitch> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CircuitSwitch>(delegate(CircuitSwitch component, object data)
	{
		component.OnCopySettings(data);
	});

	private Wire attachedWire;

	private Guid wireConnectedGUID;

	private bool wasOn;

	public string SideScreenTitleKey => "STRINGS.BUILDINGS.PREFABS.SWITCH.SIDESCREEN_TITLE";

	public bool ToggleRequested { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += CircuitOnToggle;
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[cell, (int)objectLayer];
		Wire wire = ((gameObject != null) ? gameObject.GetComponent<Wire>() : null);
		if (wire == null)
		{
			wireConnectedGUID = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected);
		}
		AttachWire(wire);
		wasOn = switchedOn;
		UpdateCircuit();
		GetComponent<KBatchedAnimController>().Play(switchedOn ? "on" : "off");
	}

	protected override void OnCleanUp()
	{
		if (attachedWire != null)
		{
			UnsubscribeFromWire(attachedWire);
		}
		bool flag = switchedOn;
		switchedOn = true;
		UpdateCircuit(should_update_anim: false);
		switchedOn = flag;
	}

	private void OnCopySettings(object data)
	{
		CircuitSwitch component = ((GameObject)data).GetComponent<CircuitSwitch>();
		if (component != null)
		{
			switchedOn = component.switchedOn;
			UpdateCircuit();
		}
	}

	public bool IsConnected()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[cell, (int)objectLayer];
		if (gameObject != null)
		{
			return gameObject.GetComponent<IDisconnectable>() != null;
		}
		return false;
	}

	private void CircuitOnToggle(bool on)
	{
		UpdateCircuit();
	}

	public void AttachWire(Wire wire)
	{
		if (!(wire == attachedWire))
		{
			if (attachedWire != null)
			{
				UnsubscribeFromWire(attachedWire);
			}
			attachedWire = wire;
			if (attachedWire != null)
			{
				SubscribeToWire(attachedWire);
				UpdateCircuit();
				wireConnectedGUID = GetComponent<KSelectable>().RemoveStatusItem(wireConnectedGUID);
			}
			else if (wireConnectedGUID == Guid.Empty)
			{
				wireConnectedGUID = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected);
			}
		}
	}

	private void OnWireDestroyed(object data)
	{
		if (attachedWire != null)
		{
			attachedWire.Unsubscribe(1969584890, OnWireDestroyed);
		}
	}

	private void OnWireStateChanged(object data)
	{
		UpdateCircuit();
	}

	private void SubscribeToWire(Wire wire)
	{
		wire.Subscribe(1969584890, OnWireDestroyed);
		wire.Subscribe(-1735440190, OnWireStateChanged);
		wire.Subscribe(774203113, OnWireStateChanged);
	}

	private void UnsubscribeFromWire(Wire wire)
	{
		wire.Unsubscribe(1969584890, OnWireDestroyed);
		wire.Unsubscribe(-1735440190, OnWireStateChanged);
		wire.Unsubscribe(774203113, OnWireStateChanged);
	}

	private void UpdateCircuit(bool should_update_anim = true)
	{
		if (attachedWire != null)
		{
			if (switchedOn)
			{
				attachedWire.Connect();
			}
			else
			{
				attachedWire.Disconnect();
			}
		}
		if (should_update_anim && wasOn != switchedOn)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play(switchedOn ? "on_pre" : "on_pst");
			component.Queue(switchedOn ? "on" : "off");
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
		wasOn = switchedOn;
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
