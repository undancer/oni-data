using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SimulatedTemperatureAdjuster
{
	private float temperature;

	private float heatCapacity;

	private float thermalConductivity;

	private bool active;

	private Storage storage;

	public SimulatedTemperatureAdjuster(float simulated_temperature, float heat_capacity, float thermal_conductivity, Storage storage)
	{
		temperature = simulated_temperature;
		heatCapacity = heat_capacity;
		thermalConductivity = thermal_conductivity;
		this.storage = storage;
		storage.gameObject.Subscribe(824508782, OnActivechanged);
		storage.gameObject.Subscribe(-1697596308, OnStorageChanged);
		Operational component = storage.gameObject.GetComponent<Operational>();
		OnActivechanged(component);
	}

	public List<Descriptor> GetDescriptors()
	{
		return GetDescriptors(temperature);
	}

	public static List<Descriptor> GetDescriptors(float temperature)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(temperature);
		Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.ITEM_TEMPERATURE_ADJUST, formattedTemperature), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ITEM_TEMPERATURE_ADJUST, formattedTemperature));
		list.Add(item);
		return list;
	}

	private void Register(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Combine(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			OnItemSimRegistered(stt);
		}
	}

	private void Unregister(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, 0f, 0f, 0f);
		}
	}

	private void OnItemSimRegistered(SimTemperatureTransfer stt)
	{
		if (!(stt == null) && Sim.IsValidHandle(stt.SimHandle))
		{
			float num = temperature;
			float heat_capacity = heatCapacity;
			float thermal_conductivity = thermalConductivity;
			if (!active)
			{
				num = 0f;
				heat_capacity = 0f;
				thermal_conductivity = 0f;
			}
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, num, heat_capacity, thermal_conductivity);
		}
	}

	private void OnActivechanged(object data)
	{
		Operational operational = (Operational)data;
		active = operational.IsActive;
		if (active)
		{
			foreach (GameObject item in storage.items)
			{
				if (item != null)
				{
					SimTemperatureTransfer component = item.GetComponent<SimTemperatureTransfer>();
					OnItemSimRegistered(component);
				}
			}
			return;
		}
		foreach (GameObject item2 in storage.items)
		{
			if (item2 != null)
			{
				SimTemperatureTransfer component2 = item2.GetComponent<SimTemperatureTransfer>();
				Unregister(component2);
			}
		}
	}

	public void CleanUp()
	{
		storage.gameObject.Unsubscribe(-1697596308, OnStorageChanged);
		foreach (GameObject item in storage.items)
		{
			if (item != null)
			{
				SimTemperatureTransfer component = item.GetComponent<SimTemperatureTransfer>();
				Unregister(component);
			}
		}
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		if (component == null)
		{
			return;
		}
		Pickupable component2 = gameObject.GetComponent<Pickupable>();
		if (!(component2 == null))
		{
			if (active && component2.storage == storage)
			{
				Register(component);
			}
			else
			{
				Unregister(component);
			}
		}
	}
}
