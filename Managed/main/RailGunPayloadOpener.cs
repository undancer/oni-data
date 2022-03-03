using UnityEngine;

public class RailGunPayloadOpener : StateMachineComponent<RailGunPayloadOpener.StatesInstance>, ISecondaryOutput
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RailGunPayloadOpener, object>.GameInstance
	{
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
		public class OperationalStates : State
		{
			public State idle;

			public State pre;

			public State loop;

			public State pst;
		}

		public State unoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			base.serializable = SerializeType.Both_DEPRECATED;
			unoperational.PlayAnim("off").EventTransition(GameHashes.OperationalFlagChanged, operational, (StatesInstance smi) => smi.master.PowerOperationalChanged()).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false, force_ignore: true);
				smi.GetComponent<ManualDeliveryKG>().Pause(pause: true, "no_power");
			});
			operational.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<ManualDeliveryKG>().Pause(pause: false, "power");
			}).EventTransition(GameHashes.OperationalFlagChanged, unoperational, (StatesInstance smi) => !smi.master.PowerOperationalChanged()).DefaultState(operational.idle)
				.EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
				{
					smi.master.payloadMeter.SetPositionPercent(Mathf.Clamp01((float)smi.master.payloadStorage.items.Count / smi.master.payloadStorage.capacityKg));
				});
			operational.idle.PlayAnim("on").EventTransition(GameHashes.OnStorageChange, operational.pre, (StatesInstance smi) => smi.HasPayload());
			operational.pre.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true, force_ignore: true);
			}).PlayAnim("working_pre").OnAnimQueueComplete(operational.loop);
			operational.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ScheduleGoTo(10f, operational.pst);
			operational.pst.PlayAnim("working_pst").Exit(delegate(StatesInstance smi)
			{
				smi.master.EmptyPayload();
				smi.GetComponent<Operational>().SetActive(value: false, force_ignore: true);
			}).OnAnimQueueComplete(operational.idle);
		}
	}

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

	private MeterController payloadMeter;

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
		payloadStorage.gunTargetOffset = new Vector2(-1f, 1.5f);
		payloadMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_storage_target", "meter_storage", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(liquidPortInfo.conduitType).RemoveFromNetworks(liquidOutputCell, liquidNetworkItem, is_endpoint: true);
		Conduit.GetNetworkManager(gasPortInfo.conduitType).RemoveFromNetworks(gasOutputCell, gasNetworkItem, is_endpoint: true);
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
			GameObject obj = payloadStorage.items[0];
			obj.GetComponent<Storage>().Transfer(resourceStorage);
			Util.KDestroyGameObject(obj);
			component.ConsumeIgnoringDisease(payloadStorage.items[0]);
		}
	}

	public bool PowerOperationalChanged()
	{
		EnergyConsumer component = GetComponent<EnergyConsumer>();
		if (component != null)
		{
			return component.IsPowered;
		}
		return false;
	}

	bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type)
	{
		if (type != gasPortInfo.conduitType && type != liquidPortInfo.conduitType)
		{
			return type == solidPortInfo.conduitType;
		}
		return true;
	}

	CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == gasPortInfo.conduitType)
		{
			return gasPortInfo.offset;
		}
		if (type == liquidPortInfo.conduitType)
		{
			return liquidPortInfo.offset;
		}
		if (type != solidPortInfo.conduitType)
		{
			return CellOffset.none;
		}
		return solidPortInfo.offset;
	}
}
