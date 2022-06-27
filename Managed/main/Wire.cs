using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Wire")]
public class Wire : KMonoBehaviour, IDisconnectable, IFirstFrameCallback, IWattageRating, IHaveUtilityNetworkMgr, IBridgedNetworkItem
{
	public enum WattageRating
	{
		Max500,
		Max1000,
		Max2000,
		Max20000,
		Max50000,
		NumRatings
	}

	[SerializeField]
	public WattageRating MaxWattageRating;

	[SerializeField]
	private bool disconnected = true;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	public float circuitOverloadTime;

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private static StatusItem WireCircuitStatus = null;

	private static StatusItem WireMaxWattageStatus = null;

	private System.Action firstFrameCallback;

	public bool IsConnected
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			return Game.Instance.electricalConduitSystem.GetNetworkForCell(cell) is ElectricalUtilityNetwork;
		}
	}

	public ushort NetworkID
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			if (!(Game.Instance.electricalConduitSystem.GetNetworkForCell(cell) is ElectricalUtilityNetwork electricalUtilityNetwork))
			{
				return ushort.MaxValue;
			}
			return (ushort)electricalUtilityNetwork.id;
		}
	}

	public static float GetMaxWattageAsFloat(WattageRating rating)
	{
		return rating switch
		{
			WattageRating.Max500 => 500f, 
			WattageRating.Max1000 => 1000f, 
			WattageRating.Max2000 => 2000f, 
			WattageRating.Max20000 => 20000f, 
			WattageRating.Max50000 => 50000f, 
			_ => 0f, 
		};
	}

	protected override void OnSpawn()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.electricalConduitSystem.AddToNetworks(cell, this, is_endpoint: false);
		InitializeSwitchState();
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		GetComponent<KSelectable>().AddStatusItem(WireCircuitStatus, this);
		GetComponent<KSelectable>().AddStatusItem(WireMaxWattageStatus, this);
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity(OutlineSymbol, is_visible: false);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.electricalConduitSystem.RemoveFromNetworks(cell, this, is_endpoint: false);
		}
		Unsubscribe(774203113, OnBuildingBrokenDelegate);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		base.OnCleanUp();
	}

	private void InitializeSwitchState()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		bool flag = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			CircuitSwitch component = gameObject.GetComponent<CircuitSwitch>();
			if (component != null)
			{
				flag = true;
				component.AttachWire(this);
			}
		}
		if (!flag)
		{
			Connect();
		}
	}

	public UtilityConnections GetWireConnections()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return Game.Instance.electricalConduitSystem.GetConnections(cell, is_physical_building: true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = GetWireConnections();
		return Game.Instance.electricalConduitSystem.GetVisualizerString(wireConnections);
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		InitializeSwitchState();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<KPrefabID>().AddTag(GameTags.Wires);
		if (WireCircuitStatus == null)
		{
			WireCircuitStatus = new StatusItem("WireCircuitStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID).SetResolveStringCallback(delegate(string str, object data)
			{
				Wire obj2 = (Wire)data;
				int cell2 = Grid.PosToCell(obj2.transform.GetPosition());
				CircuitManager circuitManager2 = Game.Instance.circuitManager;
				ushort circuitID2 = circuitManager2.GetCircuitID(cell2);
				float wattsUsedByCircuit = circuitManager2.GetWattsUsedByCircuit(circuitID2);
				GameUtil.WattageFormatterUnit unit2 = GameUtil.WattageFormatterUnit.Watts;
				if (obj2.MaxWattageRating >= WattageRating.Max20000)
				{
					unit2 = GameUtil.WattageFormatterUnit.Kilowatts;
				}
				float maxWattageAsFloat2 = GetMaxWattageAsFloat(obj2.MaxWattageRating);
				float wattsNeededWhenActive2 = circuitManager2.GetWattsNeededWhenActive(circuitID2);
				string wireLoadColor = GameUtil.GetWireLoadColor(wattsUsedByCircuit, maxWattageAsFloat2, wattsNeededWhenActive2);
				str = str.Replace("{CurrentLoadAndColor}", (wireLoadColor == Color.white.ToHexString()) ? GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit2) : ("<color=#" + wireLoadColor + ">" + GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit2) + "</color>"));
				str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat2, unit2));
				str = str.Replace("{WireType}", this.GetProperName());
				return str;
			});
		}
		if (WireMaxWattageStatus != null)
		{
			return;
		}
		WireMaxWattageStatus = new StatusItem("WireMaxWattageStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID).SetResolveStringCallback(delegate(string str, object data)
		{
			Wire obj = (Wire)data;
			GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
			if (obj.MaxWattageRating >= WattageRating.Max20000)
			{
				unit = GameUtil.WattageFormatterUnit.Kilowatts;
			}
			int cell = Grid.PosToCell(obj.transform.GetPosition());
			CircuitManager circuitManager = Game.Instance.circuitManager;
			ushort circuitID = circuitManager.GetCircuitID(cell);
			float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
			float maxWattageAsFloat = GetMaxWattageAsFloat(obj.MaxWattageRating);
			str = str.Replace("{TotalPotentialLoadAndColor}", (wattsNeededWhenActive > maxWattageAsFloat) ? ("<color=#" + new Color(0.9843137f, 0.6901961f, 0.23137255f).ToHexString() + ">" + GameUtil.GetFormattedWattage(wattsNeededWhenActive, unit) + "</color>") : GameUtil.GetFormattedWattage(wattsNeededWhenActive, unit));
			str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
			return str;
		});
	}

	public WattageRating GetMaxWattageRating()
	{
		return MaxWattageRating;
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
			Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
		}
		return !disconnected;
	}

	public void Disconnect()
	{
		disconnected = true;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected);
		Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
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

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell);
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}
}
