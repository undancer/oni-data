using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicClusterLocationSensor : Switch, ISaveLoadable, ISim200ms
{
	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private List<AxialI> activeLocations = new List<AxialI>();

	[Serialize]
	private bool activeInSpace = true;

	private bool wasOn;

	private static readonly EventSystem.IntraObjectHandler<LogicClusterLocationSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicClusterLocationSensor>(delegate(LogicClusterLocationSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public bool ActiveInSpace => activeInSpace;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		_ = ((GameObject)data).GetComponent<LogicTimeOfDaySensor>() != null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
		SetLocationEnabled(new AxialI(0, 0), setting: true);
	}

	public void SetLocationEnabled(AxialI location, bool setting)
	{
		if (!setting)
		{
			activeLocations.Remove(location);
		}
		else if (!activeLocations.Contains(location))
		{
			activeLocations.Add(location);
		}
	}

	public void SetSpaceEnabled(bool setting)
	{
		activeInSpace = setting;
	}

	public void Sim200ms(float dt)
	{
		bool state = CheckCurrentLocationSelected();
		SetState(state);
	}

	private bool CheckCurrentLocationSelected()
	{
		AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
		if (activeLocations.Contains(myWorldLocation))
		{
			return true;
		}
		if (activeInSpace && CheckInEmptySpace())
		{
			return true;
		}
		return false;
	}

	private bool CheckInEmptySpace()
	{
		bool result = true;
		AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsModuleInterior && worldContainer.GetMyWorldLocation() == myWorldLocation)
			{
				return false;
			}
		}
		return result;
	}

	public bool CheckLocationSelected(AxialI location)
	{
		return activeLocations.Contains(location);
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (!(wasOn != switchedOn || force))
		{
			return;
		}
		wasOn = switchedOn;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
		bool flag = true;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsModuleInterior && worldContainer.GetMyWorldLocation() == myWorldLocation)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			component.Play(switchedOn ? "on_space_pre" : "on_space_pst");
			component.Queue(switchedOn ? "on_space" : "off_space");
		}
		else
		{
			component.Play(switchedOn ? "on_asteroid_pre" : "on_asteroid_pst");
			component.Queue(switchedOn ? "on_asteroid" : "off_asteroid");
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}
}
