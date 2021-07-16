using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitBridge")]
public class ConduitBridge : ConduitBridgeBase, IBridgedNetworkItem
{
	[SerializeField]
	public ConduitType type;

	private int inputCell;

	private int outputCell;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		accumulator = Game.Instance.accumulators.Add("Flow", this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(type).AddConduitUpdater(ConduitUpdate);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(type).RemoveConduitUpdater(ConduitUpdate);
		Game.Instance.accumulators.Remove(accumulator);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(type);
		if (!flowManager.HasConduit(inputCell) || !flowManager.HasConduit(outputCell))
		{
			SendEmptyOnMassTransfer();
			return;
		}
		ConduitFlow.ConduitContents contents = flowManager.GetContents(inputCell);
		float num = contents.mass;
		if (desiredMassTransfer != null)
		{
			num = desiredMassTransfer(dt, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount, null);
		}
		if (num > 0f)
		{
			int disease_count = (int)(num / contents.mass * (float)contents.diseaseCount);
			float num2 = flowManager.AddElement(outputCell, contents.element, num, contents.temperature, contents.diseaseIdx, disease_count);
			if (num2 > 0f)
			{
				flowManager.RemoveElement(inputCell, num2);
				Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
				if (OnMassTransfer != null)
				{
					OnMassTransfer(contents.element, num2, contents.temperature, contents.diseaseIdx, disease_count, null);
				}
			}
			else
			{
				SendEmptyOnMassTransfer();
			}
		}
		else
		{
			SendEmptyOnMassTransfer();
		}
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(type);
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(inputCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
		networkForCell = networkManager.GetNetworkForCell(outputCell);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(type);
		if (0 == 0 && !networks.Contains(networkManager.GetNetworkForCell(inputCell)))
		{
			return networks.Contains(networkManager.GetNetworkForCell(outputCell));
		}
		return true;
	}

	public int GetNetworkCell()
	{
		return inputCell;
	}
}
