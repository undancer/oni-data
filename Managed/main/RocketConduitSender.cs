using UnityEngine;

public class RocketConduitSender : StateMachineComponent<RocketConduitSender.StatesInstance>, ISecondaryInput
{
	private class ConduitPort
	{
		public ConduitPortInfo conduitPortInfo;

		public int inputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		private ConduitConsumer conduitConsumer;

		public ConduitPort(GameObject parent, ConduitPortInfo info, Storage targetStorage)
		{
			conduitPortInfo = info;
			ConduitConsumer conduitConsumer = parent.AddComponent<ConduitConsumer>();
			conduitConsumer.conduitType = conduitPortInfo.conduitType;
			conduitConsumer.useSecondaryInput = true;
			conduitConsumer.storage = targetStorage;
			conduitConsumer.capacityKG = targetStorage.capacityKg;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			this.conduitConsumer = conduitConsumer;
			this.conduitConsumer.keepZeroMassObject = false;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketConduitSender, object>.GameInstance
	{
		public StatesInstance(RocketConduitSender smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketConduitSender>
	{
		public class onStates : State
		{
			public workingStates working;

			public State waiting;
		}

		public class workingStates : State
		{
			public State notOnGround;

			public State ground;
		}

		public onStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = on;
			base.serializable = SerializeType.Both_DEPRECATED;
			on.DefaultState(on.waiting);
			on.waiting.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).EventTransition(GameHashes.OnStorageChange, on.working, (StatesInstance smi) => smi.GetComponent<Storage>().MassStored() > 0f);
			on.working.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).DefaultState(on.working.ground);
			on.working.notOnGround.Enter(delegate(StatesInstance smi)
			{
				smi.gameObject.AddOrGetDef<AutoStorageDropper.Def>().invertElementFilter = true;
			}).UpdateTransition(on.working.ground, delegate(StatesInstance smi, float f)
			{
				WorldContainer myWorld2 = smi.master.GetMyWorld();
				return (bool)myWorld2 && myWorld2.IsModuleInterior && !myWorld2.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().HasTag(GameTags.RocketNotOnGround);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.gameObject.AddOrGetDef<AutoStorageDropper.Def>().invertElementFilter = false;
			});
			on.working.ground.Enter(delegate(StatesInstance smi)
			{
				smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = true;
			}).UpdateTransition(on.working.notOnGround, delegate(StatesInstance smi, float f)
			{
				WorldContainer myWorld = smi.master.GetMyWorld();
				return (bool)myWorld && myWorld.IsModuleInterior && myWorld.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().HasTag(GameTags.RocketNotOnGround);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = false;
			});
		}
	}

	public Storage conduitStorage;

	[SerializeField]
	public ConduitPortInfo conduitPortInfo;

	private ConduitPort conduitPort;

	private RocketConduitReceiver partnerReceiver;

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> TryFindPartnerDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.FindPartner();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.AddConduitPortToNetwork();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.RemoveConduitPortFromNetwork();
	});

	public void AddConduitPortToNetwork()
	{
		if (conduitPort != null)
		{
			int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), conduitPortInfo.offset);
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(conduitPortInfo.conduitType);
			conduitPort.inputCell = num;
			conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(conduitPortInfo.conduitType, Endpoint.Sink, num, base.gameObject);
			networkManager.AddToNetworks(num, conduitPort.networkItem, is_endpoint: true);
		}
	}

	public void RemoveConduitPortFromNetwork()
	{
		if (conduitPort != null)
		{
			Conduit.GetNetworkManager(conduitPortInfo.conduitType).RemoveFromNetworks(conduitPort.inputCell, conduitPort.networkItem, is_endpoint: true);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		FindPartner();
		Subscribe(-1118736034, TryFindPartnerDelegate);
		Subscribe(546421097, OnLaunchedDelegate);
		Subscribe(-735346771, OnLandedDelegate);
		base.smi.StartSM();
		Components.RocketConduitSenders.Add(this);
	}

	protected override void OnCleanUp()
	{
		RemoveConduitPortFromNetwork();
		base.OnCleanUp();
		Components.RocketConduitSenders.Remove(this);
	}

	private void FindPartner()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(base.gameObject.GetMyWorldId());
		if (world != null && world.IsModuleInterior)
		{
			RocketConduitReceiver[] components = world.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().GetComponents<RocketConduitReceiver>();
			foreach (RocketConduitReceiver rocketConduitReceiver in components)
			{
				if (rocketConduitReceiver.conduitPortInfo.conduitType == conduitPortInfo.conduitType)
				{
					partnerReceiver = rocketConduitReceiver;
					break;
				}
			}
		}
		else
		{
			ClustercraftExteriorDoor component = base.gameObject.GetComponent<ClustercraftExteriorDoor>();
			if (component.HasTargetWorld())
			{
				WorldContainer targetWorld = component.GetTargetWorld();
				foreach (RocketConduitReceiver worldItem in Components.RocketConduitReceivers.GetWorldItems(targetWorld.id))
				{
					if (worldItem.conduitPortInfo.conduitType == conduitPortInfo.conduitType)
					{
						partnerReceiver = worldItem;
						break;
					}
				}
			}
		}
		if (partnerReceiver == null)
		{
			Debug.LogWarning("No rocket conduit receiver found?");
			return;
		}
		conduitPort = new ConduitPort(base.gameObject, conduitPortInfo, conduitStorage);
		if (world != null)
		{
			AddConduitPortToNetwork();
		}
		partnerReceiver.SetStorage(conduitStorage);
	}

	bool ISecondaryInput.HasSecondaryConduitType(ConduitType type)
	{
		return conduitPortInfo.conduitType == type;
	}

	CellOffset ISecondaryInput.GetSecondaryConduitOffset(ConduitType type)
	{
		if (conduitPortInfo.conduitType == type)
		{
			return conduitPortInfo.offset;
		}
		return CellOffset.none;
	}
}
