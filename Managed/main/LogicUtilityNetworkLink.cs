using System.Collections.Generic;

public class LogicUtilityNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr, IBridgedNetworkItem
{
	public LogicWire.BitDepth bitDepth;

	public int cell_one;

	public int cell_two;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		cell_one = cell1;
		cell_two = cell2;
		Game.Instance.logicCircuitSystem.AddLink(cell1, cell2);
		Game.Instance.logicCircuitManager.Connect(this);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.logicCircuitSystem.RemoveLink(cell1, cell2);
		Game.Instance.logicCircuitManager.Disconnect(this);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.logicCircuitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int networkCell = GetNetworkCell();
		IUtilityNetworkMgr networkManager = GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(networkCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int networkCell = GetNetworkCell();
		IUtilityNetworkMgr networkManager = GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(networkCell);
		return networks.Contains(networkForCell);
	}
}
