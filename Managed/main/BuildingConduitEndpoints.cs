using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingConduitEndpoints")]
public class BuildingConduitEndpoints : KMonoBehaviour
{
	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		BuildingDef def = component.Def;
		if (def.InputConduitType != 0)
		{
			int utilityInputCell = component.GetUtilityInputCell();
			itemInput = new FlowUtilityNetwork.NetworkItem(def.InputConduitType, Endpoint.Sink, utilityInputCell, base.gameObject);
			if (def.InputConduitType == ConduitType.Solid)
			{
				Game.Instance.solidConduitSystem.AddToNetworks(utilityInputCell, itemInput, is_endpoint: true);
			}
			else
			{
				Conduit.GetNetworkManager(def.InputConduitType).AddToNetworks(utilityInputCell, itemInput, is_endpoint: true);
			}
		}
		if (def.OutputConduitType != 0)
		{
			int utilityOutputCell = component.GetUtilityOutputCell();
			itemOutput = new FlowUtilityNetwork.NetworkItem(def.OutputConduitType, Endpoint.Source, utilityOutputCell, base.gameObject);
			if (def.OutputConduitType == ConduitType.Solid)
			{
				Game.Instance.solidConduitSystem.AddToNetworks(utilityOutputCell, itemOutput, is_endpoint: true);
			}
			else
			{
				Conduit.GetNetworkManager(def.OutputConduitType).AddToNetworks(utilityOutputCell, itemOutput, is_endpoint: true);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (itemInput != null)
		{
			if (itemInput.ConduitType == ConduitType.Solid)
			{
				Game.Instance.solidConduitSystem.RemoveFromNetworks(itemInput.Cell, itemInput, is_endpoint: true);
			}
			else
			{
				Conduit.GetNetworkManager(itemInput.ConduitType).RemoveFromNetworks(itemInput.Cell, itemInput, is_endpoint: true);
			}
		}
		if (itemOutput != null)
		{
			if (itemOutput.ConduitType == ConduitType.Solid)
			{
				Game.Instance.solidConduitSystem.RemoveFromNetworks(itemOutput.Cell, itemOutput, is_endpoint: true);
			}
			else
			{
				Conduit.GetNetworkManager(itemOutput.ConduitType).RemoveFromNetworks(itemOutput.Cell, itemOutput, is_endpoint: true);
			}
		}
		base.OnCleanUp();
	}
}
