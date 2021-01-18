using UnityEngine;

public class RailGunPayloadOpener : StateMachineComponent<RailGunPayloadOpener.StatesInstance>, ISecondaryOutput
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RailGunPayloadOpener, object>.GameInstance
	{
		private FetchChore chore;

		public StatesInstance(RailGunPayloadOpener master)
			: base(master)
		{
		}

		public bool HasPayload()
		{
			return base.smi.master.payloadStorage.items.Count > 0;
		}

		public bool HasResources()
		{
			return base.smi.master.resourceStorage.MassStored() > 0f;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RailGunPayloadOpener>
	{
		public State waiting;

		public State working;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waiting;
			base.serializable = SerializeType.Both_DEPRECATED;
			waiting.PlayAnim("on").EventTransition(GameHashes.OnStorageChange, working, (StatesInstance smi) => smi.HasPayload());
			working.Enter(delegate(StatesInstance smi)
			{
				smi.master.EmptyPayload();
				smi.GoTo(waiting);
			});
		}
	}

	private static readonly CellOffset[] delivery_offset = new CellOffset[1];

	public static float delivery_time = 10f;

	[SerializeField]
	public ConduitPortInfo liquidPortInfo;

	private int liquidOutputCell = -1;

	private FlowUtilityNetwork.NetworkItem liquidNetworkItem;

	private ConduitDispenser liquidDispenser;

	[SerializeField]
	public ConduitPortInfo gasPortInfo;

	private int gasOutputCell = -1;

	private FlowUtilityNetwork.NetworkItem gasNetworkItem;

	private ConduitDispenser gasDispenser;

	[SerializeField]
	public ConduitPortInfo solidPortInfo;

	private int solidOutputCell = -1;

	private FlowUtilityNetwork.NetworkItem solidNetworkItem;

	private SolidConduitDispenser solidDispenser;

	public Storage payloadStorage;

	public Storage resourceStorage;

	private ManualDeliveryKG[] deliveryComponents;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		gasOutputCell = Grid.OffsetCell(Grid.PosToCell(this), gasPortInfo.offset);
		gasDispenser = CreateConduitDispenser(ConduitType.Gas, gasOutputCell, out gasNetworkItem);
		liquidOutputCell = Grid.OffsetCell(Grid.PosToCell(this), liquidPortInfo.offset);
		liquidDispenser = CreateConduitDispenser(ConduitType.Liquid, liquidOutputCell, out liquidNetworkItem);
		solidOutputCell = Grid.OffsetCell(Grid.PosToCell(this), solidPortInfo.offset);
		solidDispenser = CreateSolidConduitDispenser(solidOutputCell, out solidNetworkItem);
		deliveryComponents = GetComponents<ManualDeliveryKG>();
		payloadStorage.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interact_railgun_emptier_kanim")
		};
		payloadStorage.useGunForDelivery = false;
		payloadStorage.synchronizeAnims = true;
		payloadStorage.workAnims = new HashedString[2]
		{
			"working_pre",
			"working_loop"
		};
		payloadStorage.storageWorkTime = delivery_time;
		payloadStorage.workingPstComplete = new HashedString[1]
		{
			"working_pst"
		};
		payloadStorage.SetOffsets(delivery_offset);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(liquidPortInfo.conduitType);
		networkManager.RemoveFromNetworks(liquidOutputCell, liquidNetworkItem, is_endpoint: true);
		networkManager = Conduit.GetNetworkManager(gasPortInfo.conduitType);
		networkManager.RemoveFromNetworks(gasOutputCell, gasNetworkItem, is_endpoint: true);
		Game.Instance.solidConduitSystem.RemoveFromNetworks(solidOutputCell, solidDispenser, is_endpoint: true);
		base.OnCleanUp();
	}

	private ConduitDispenser CreateConduitDispenser(ConduitType outputType, int outputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		ConduitDispenser conduitDispenser = base.gameObject.AddComponent<ConduitDispenser>();
		conduitDispenser.conduitType = outputType;
		conduitDispenser.useSecondaryOutput = true;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.storage = resourceStorage;
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(outputType);
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(outputType, Endpoint.Source, outputCell, base.gameObject);
		networkManager.AddToNetworks(outputCell, flowNetworkItem, is_endpoint: true);
		return conduitDispenser;
	}

	private SolidConduitDispenser CreateSolidConduitDispenser(int outputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		SolidConduitDispenser solidConduitDispenser = base.gameObject.AddComponent<SolidConduitDispenser>();
		solidConduitDispenser.storage = resourceStorage;
		solidConduitDispenser.alwaysDispense = true;
		solidConduitDispenser.useSecondaryOutput = true;
		solidConduitDispenser.solidOnly = true;
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Source, outputCell, base.gameObject);
		Game.Instance.solidConduitSystem.AddToNetworks(outputCell, flowNetworkItem, is_endpoint: true);
		return solidConduitDispenser;
	}

	public void EmptyPayload()
	{
		Storage component = GetComponent<Storage>();
		if (component != null && component.items.Count > 0)
		{
			GameObject gameObject = payloadStorage.items[0];
			Storage component2 = gameObject.GetComponent<Storage>();
			component2.Transfer(resourceStorage);
			Util.KDestroyGameObject(gameObject);
			component.ConsumeIgnoringDisease(payloadStorage.items[0]);
		}
	}

	public bool HasSecondaryConduitType(ConduitType type)
	{
		return type == gasPortInfo.conduitType || type == liquidPortInfo.conduitType || type == solidPortInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == gasPortInfo.conduitType)
		{
			return gasPortInfo.offset;
		}
		if (type == liquidPortInfo.conduitType)
		{
			return liquidPortInfo.offset;
		}
		if (type == solidPortInfo.conduitType)
		{
			return solidPortInfo.offset;
		}
		return CellOffset.none;
	}
}
