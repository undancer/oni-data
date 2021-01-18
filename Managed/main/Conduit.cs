using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Conduit")]
public class Conduit : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr, IBridgedNetworkItem, IDisconnectable, FlowUtilityNetwork.IItem
{
	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	[SerializeField]
	private bool disconnected = true;

	public ConduitType type;

	private System.Action firstFrameCallback;

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnHighlightedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnHighlighted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitFrozenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitFrozen(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitBoilingDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitBoiling(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnStructureTemperatureRegistered(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	public FlowUtilityNetwork Network
	{
		set
		{
		}
	}

	public int Cell => Grid.PosToCell(this);

	public Endpoint EndpointType => Endpoint.Conduit;

	public ConduitType ConduitType => type;

	public GameObject GameObject => base.gameObject;

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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1201923725, OnHighlightedDelegate);
		Subscribe(-700727624, OnConduitFrozenDelegate);
		Subscribe(-1152799878, OnConduitBoilingDelegate);
		Subscribe(-1555603773, OnStructureTemperatureRegisteredDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
	}

	private void OnStructureTemperatureRegistered(object data)
	{
		int cell = Grid.PosToCell(this);
		GetNetworkManager().AddToNetworks(cell, this, is_endpoint: false);
		Connect();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		BuildingDef def = GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			GetFlowVisualizer().AddThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(774203113, OnBuildingBrokenDelegate);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		BuildingDef def = GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			GetFlowVisualizer().RemoveThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GetNetworkManager().RemoveFromNetworks(cell, this, is_endpoint: false);
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)component.Def.ReplacementLayer] == null)
		{
			GetNetworkManager().RemoveFromNetworks(cell, this, is_endpoint: false);
			GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
		}
		base.OnCleanUp();
	}

	private ConduitFlowVisualizer GetFlowVisualizer()
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidFlowVisualizer;
		}
		return Game.Instance.gasFlowVisualizer;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitSystem;
		}
		return Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetFlowManager()
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitFlow;
		}
		return Game.Instance.gasConduitFlow;
	}

	public static ConduitFlow GetFlowManager(ConduitType type)
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitFlow;
		}
		return Game.Instance.gasConduitFlow;
	}

	public static IUtilityNetworkMgr GetNetworkManager(ConduitType type)
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitSystem;
		}
		return Game.Instance.gasConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}

	private void OnHighlighted(object data)
	{
		int highlightedCell = (((bool)data) ? Grid.PosToCell(base.transform.GetPosition()) : (-1));
		GetFlowVisualizer().SetHighlightedCell(highlightedCell);
	}

	private void OnConduitFrozen(object data)
	{
		Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE,
			takeDamageEffect = ((ConduitType == ConduitType.Gas) ? SpawnFXHashes.BuildingLeakLiquid : SpawnFXHashes.BuildingFreeze),
			fullDamageEffectName = ((ConduitType == ConduitType.Gas) ? "water_damage_kanim" : "ice_damage_kanim")
		});
		GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnConduitBoiling(object data)
	{
		Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED,
			takeDamageEffect = SpawnFXHashes.BuildingLeakGas,
			fullDamageEffectName = "gas_damage_kanim"
		});
		GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
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
			GetNetworkManager().ForceRebuildNetworks();
		}
		return !disconnected;
	}

	public void Disconnect()
	{
		disconnected = true;
		GetNetworkManager().ForceRebuildNetworks();
	}
}
