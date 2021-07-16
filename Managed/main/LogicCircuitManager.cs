using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitManager
{
	private struct Signal
	{
		public int cell;

		public int value;

		public Signal(int cell, int value)
		{
			this.cell = cell;
			this.value = value;
		}
	}

	public static float ClockTickInterval = 0.1f;

	private float elapsedTime;

	private UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduitSystem;

	private List<ILogicUIElement> uiVisElements = new List<ILogicUIElement>();

	public static float BridgeRefreshInterval = 1f;

	private List<LogicUtilityNetworkLink>[] bridgeGroups = new List<LogicUtilityNetworkLink>[2];

	private bool updateEvenBridgeGroups;

	private float timeSinceBridgeRefresh;

	public System.Action onLogicTick;

	public Action<ILogicUIElement> onElemAdded;

	public Action<ILogicUIElement> onElemRemoved;

	public LogicCircuitManager(UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduit_system)
	{
		conduitSystem = conduit_system;
		timeSinceBridgeRefresh = 0f;
		elapsedTime = 0f;
		for (int i = 0; i < 2; i++)
		{
			bridgeGroups[i] = new List<LogicUtilityNetworkLink>();
		}
	}

	public void RenderEveryTick(float dt)
	{
		Refresh(dt);
	}

	private void Refresh(float dt)
	{
		if (conduitSystem.IsDirty)
		{
			conduitSystem.Update();
			LogicCircuitNetwork.logicSoundRegister.Clear();
			PropagateSignals(force_send_events: true);
			elapsedTime = 0f;
		}
		else if (SpeedControlScreen.Instance != null && !SpeedControlScreen.Instance.IsPaused)
		{
			elapsedTime += dt;
			timeSinceBridgeRefresh += dt;
			while (elapsedTime > ClockTickInterval)
			{
				elapsedTime -= ClockTickInterval;
				PropagateSignals(force_send_events: false);
				if (onLogicTick != null)
				{
					onLogicTick();
				}
			}
			if (timeSinceBridgeRefresh > BridgeRefreshInterval)
			{
				UpdateCircuitBridgeLists();
				timeSinceBridgeRefresh = 0f;
			}
		}
		foreach (LogicCircuitNetwork network in Game.Instance.logicCircuitSystem.GetNetworks())
		{
			CheckCircuitOverloaded(dt, network.id, network.GetBitsUsed());
		}
	}

	private void PropagateSignals(bool force_send_events)
	{
		IList<UtilityNetwork> networks = Game.Instance.logicCircuitSystem.GetNetworks();
		foreach (LogicCircuitNetwork item in networks)
		{
			item.UpdateLogicValue();
		}
		foreach (LogicCircuitNetwork item2 in networks)
		{
			item2.SendLogicEvents(force_send_events, item2.id);
		}
	}

	public LogicCircuitNetwork GetNetworkForCell(int cell)
	{
		return conduitSystem.GetNetworkForCell(cell) as LogicCircuitNetwork;
	}

	public void AddVisElem(ILogicUIElement elem)
	{
		uiVisElements.Add(elem);
		if (onElemAdded != null)
		{
			onElemAdded(elem);
		}
	}

	public void RemoveVisElem(ILogicUIElement elem)
	{
		if (onElemRemoved != null)
		{
			onElemRemoved(elem);
		}
		uiVisElements.Remove(elem);
	}

	public ReadOnlyCollection<ILogicUIElement> GetVisElements()
	{
		return uiVisElements.AsReadOnly();
	}

	public static void ToggleNoWireConnected(bool show_missing_wire, GameObject go)
	{
		go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoLogicWireConnected, show_missing_wire);
	}

	private void CheckCircuitOverloaded(float dt, int id, int bits_used)
	{
		UtilityNetwork networkByID = Game.Instance.logicCircuitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			((LogicCircuitNetwork)networkByID)?.UpdateOverloadTime(dt, bits_used);
		}
	}

	public void Connect(LogicUtilityNetworkLink bridge)
	{
		bridgeGroups[(int)bridge.bitDepth].Add(bridge);
	}

	public void Disconnect(LogicUtilityNetworkLink bridge)
	{
		bridgeGroups[(int)bridge.bitDepth].Remove(bridge);
	}

	private void UpdateCircuitBridgeLists()
	{
		foreach (LogicCircuitNetwork network in Game.Instance.logicCircuitSystem.GetNetworks())
		{
			if (updateEvenBridgeGroups)
			{
				if (network.id % 2 == 0)
				{
					network.UpdateRelevantBridges(bridgeGroups);
				}
			}
			else if (network.id % 2 == 1)
			{
				network.UpdateRelevantBridges(bridgeGroups);
			}
		}
		updateEvenBridgeGroups = !updateEvenBridgeGroups;
	}
}
