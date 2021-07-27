using System.Collections.Generic;
using UnityEngine;

public class WireUtilityNetworkLink : UtilityNetworkLink, IWattageRating, IHaveUtilityNetworkMgr, IBridgedNetworkItem, ICircuitConnected
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.RemoveLink(cell1, cell2);
		Game.Instance.circuitManager.Disconnect(this);
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.AddLink(cell1, cell2);
		Game.Instance.circuitManager.Connect(this);
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
