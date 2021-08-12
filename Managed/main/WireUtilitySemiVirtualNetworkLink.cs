using System.Collections.Generic;
using UnityEngine;

public class WireUtilitySemiVirtualNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr, ICircuitConnected
{
	[SerializeField]
	public Wire.WattageRating maxWattageRating;

	public bool IsVirtual { get; private set; }

	public int PowerCell => GetNetworkCell();

	public object VirtualCircuitKey { get; private set; }

	public Wire.WattageRating GetMaxWattageRating()
	{
		return maxWattageRating;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			VirtualCircuitKey = component.CraftInterface;
		}
		else
		{
			CraftModuleInterface component2 = this.GetMyWorld().GetComponent<CraftModuleInterface>();
			if (component2 != null)
			{
				VirtualCircuitKey = component2;
			}
		}
		Game.Instance.electricalConduitSystem.AddToVirtualNetworks(VirtualCircuitKey, this, is_endpoint: true);
		base.OnSpawn();
	}

	public void SetLinkConnected(bool connect)
	{
		if (connect && visualizeOnly)
		{
			visualizeOnly = false;
			if (base.isSpawned)
			{
				Connect();
			}
		}
		else if (!connect && !visualizeOnly)
		{
			if (base.isSpawned)
			{
				Disconnect();
			}
			visualizeOnly = true;
		}
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(cell1, VirtualCircuitKey);
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.AddSemiVirtualLink(cell1, VirtualCircuitKey);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int networkCell = GetNetworkCell();
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(networkCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int networkCell = GetNetworkCell();
		UtilityNetwork networkForCell = GetNetworkManager().GetNetworkForCell(networkCell);
		return networks.Contains(networkForCell);
	}
}
