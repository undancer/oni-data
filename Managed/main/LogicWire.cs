using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/LogicWire")]
public class LogicWire : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr, IBridgedNetworkItem, IBitRating, IDisconnectable
{
	public enum BitDepth
	{
		OneBit,
		FourBit,
		NumRatings
	}

	[SerializeField]
	public BitDepth MaxBitDepth;

	[SerializeField]
	private bool disconnected = true;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicWire>(delegate(LogicWire component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicWire>(delegate(LogicWire component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private System.Action firstFrameCallback;

	public bool IsConnected
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			return Game.Instance.logicCircuitSystem.GetNetworkForCell(cell) is LogicCircuitNetwork;
		}
	}

	public static int GetBitDepthAsInt(BitDepth rating)
	{
		return rating switch
		{
			BitDepth.OneBit => 1, 
			BitDepth.FourBit => 4, 
			_ => 0, 
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.logicCircuitSystem.AddToNetworks(cell, this, is_endpoint: false);
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		Connect();
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity(OutlineSymbol, is_visible: false);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.logicCircuitSystem.RemoveFromNetworks(cell, this, is_endpoint: false);
		}
		Unsubscribe(774203113, OnBuildingBrokenDelegate);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		base.OnCleanUp();
	}

	public bool IsDisconnected()
	{
		return disconnected;
	}

	public bool Connect()
	{
		BuildingHP component = GetComponent<BuildingHP>();
		if (component == null || component.HitPoints > 0)
		{
			disconnected = false;
			Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
		}
		return !disconnected;
	}

	public void Disconnect()
	{
		disconnected = true;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected);
		Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
	}

	public UtilityConnections GetWireConnections()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return Game.Instance.logicCircuitSystem.GetConnections(cell, is_physical_building: true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = GetWireConnections();
		return Game.Instance.logicCircuitSystem.GetVisualizerString(wireConnections);
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
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

	public BitDepth GetMaxBitRating()
	{
		return MaxBitDepth;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.logicCircuitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(cell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(cell);
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}
}
