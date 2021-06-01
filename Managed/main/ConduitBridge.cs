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
			float num2 = num / contents.mass;
			int disease_count = (int)(num2 * (float)contents.diseaseCount);
			float num3 = flowManager.AddElement(outputCell, contents.element, num, contents.temperature, contents.diseaseIdx, disease_count);
			if (num3 > 0f)
			{
				flowManager.RemoveElement(inputCell, num3);
				Game.Instance.accumulators.Accumulate(accumulator, contents.mass);
				if (OnMassTransfer != null)
				{
					OnMassTransfer(contents.element, num3, contents.temperature, contents.diseaseIdx, disease_count, null);
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
		bool flag = false;
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(type);
		return flag || networks.Contains(networkManager.GetNetworkForCell(inputCell)) || networks.Contains(networkManager.GetNetworkForCell(outputCell));
	}

	public int GetNetworkCell()
	{
		return inputCell;
	}
}
