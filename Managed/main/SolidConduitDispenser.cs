using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitDispenser")]
public class SolidConduitDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
	[SerializeField]
	public SimHashes[] elementFilter = null;

	[SerializeField]
	public bool invertElementFilter = false;

	[SerializeField]
	public bool alwaysDispense = false;

	[SerializeField]
	public bool useSecondaryOutput = false;

	[SerializeField]
	public bool solidOnly = false;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private bool dispensing;

	private int round_robin_index;

	private const float MaxMass = 20f;

	public Storage Storage => storage;

	public ConduitType ConduitType => ConduitType.Solid;

	public SolidConduitFlow.ConduitContents ConduitContents => GetConduitFlow().GetContents(utilityCell);

	public bool IsDispensing => dispensing;

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, 20];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	public SolidConduitFlow GetConduitFlow()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		utilityCell = GetOutputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[20];
		partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitFlow().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Dispense);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitFlow().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		dispensing = dispensing && IsConnected;
		Trigger(-2094018600, IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		bool flag = false;
		operational.SetFlag(outputConduitFlag, IsConnected);
		if (operational.IsOperational || alwaysDispense)
		{
			SolidConduitFlow conduitFlow = GetConduitFlow();
			if (conduitFlow.HasConduit(utilityCell) && conduitFlow.IsConduitEmpty(utilityCell))
			{
				Pickupable pickupable = FindSuitableItem();
				if ((bool)pickupable)
				{
					if (pickupable.PrimaryElement.Mass > 20f)
					{
						pickupable = pickupable.Take(20f);
					}
					conduitFlow.AddPickupable(utilityCell, pickupable);
					flag = true;
				}
			}
		}
		storage.storageNetworkID = GetConnectedNetworkID();
		dispensing = flag;
	}

	private bool isSolid(GameObject o)
	{
		PrimaryElement component = o.GetComponent<PrimaryElement>();
		return component == null || component.Element.IsLiquid || component.Element.IsGas;
	}

	private Pickupable FindSuitableItem()
	{
		List<GameObject> list = storage.items;
		if (solidOnly)
		{
			List<GameObject> list2 = new List<GameObject>(list);
			list2.RemoveAll(isSolid);
			list = list2;
		}
		if (list.Count < 1)
		{
			return null;
		}
		round_robin_index %= list.Count;
		GameObject gameObject = list[round_robin_index];
		round_robin_index++;
		return gameObject ? gameObject.GetComponent<Pickupable>() : null;
	}

	private int GetConnectedNetworkID()
	{
		GameObject gameObject = Grid.Objects[utilityCell, 20];
		SolidConduit solidConduit = ((gameObject != null) ? gameObject.GetComponent<SolidConduit>() : null);
		return ((solidConduit != null) ? solidConduit.GetNetwork() : null)?.id ?? (-1);
	}

	private int GetOutputCell()
	{
		Building component = GetComponent<Building>();
		if (useSecondaryOutput)
		{
			ISecondaryOutput component2 = GetComponent<ISecondaryOutput>();
			return Grid.OffsetCell(component.NaturalBuildingCell(), component2.GetSecondaryConduitOffset(ConduitType.Solid));
		}
		return component.GetUtilityOutputCell();
	}
}
