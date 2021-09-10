using UnityEngine;

public class RocketConduitReceiver : StateMachineComponent<RocketConduitReceiver.StatesInstance>, ISecondaryOutput
{
	public struct ConduitPort
	{
		public ConduitPortInfo portInfo;

		public int outputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		public ConduitDispenser conduitDispenser;

		public void SetPortInfo(GameObject parent, ConduitPortInfo info, Storage senderStorage)
		{
			portInfo = info;
			ConduitDispenser conduitDispenser = parent.AddComponent<ConduitDispenser>();
			conduitDispenser.conduitType = portInfo.conduitType;
			conduitDispenser.useSecondaryOutput = true;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.storage = senderStorage;
			this.conduitDispenser = conduitDispenser;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketConduitReceiver, object>.GameInstance
	{
		public StatesInstance(RocketConduitReceiver master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketConduitReceiver>
	{
		public class onStates : State
		{
			public State hasResources;

			public State empty;
		}

		public State off;

		public onStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			base.serializable = SerializeType.Both_DEPRECATED;
			off.EventTransition(GameHashes.OperationalFlagChanged, on, (StatesInstance smi) => smi.GetComponent<Operational>().GetFlag(WarpConduitStatus.warpConnectedFlag));
			on.DefaultState(on.empty);
			on.empty.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).Update(delegate(StatesInstance smi, float dt)
			{
				if (smi.master.CanTransferFromSender())
				{
					smi.GoTo(on.hasResources);
				}
			});
			on.hasResources.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).Update(delegate(StatesInstance smi, float dt)
			{
				if (!smi.master.CanTransferFromSender())
				{
					smi.GoTo(on.empty);
				}
			});
		}
	}

	[SerializeField]
	public ConduitPortInfo conduitPortInfo;

	public ConduitPort conduitPort;

	public Storage senderConduitStorage;

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> TryFindPartner = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.FindPartner();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.AddConduitPortToNetwork();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.RemoveConduitPortFromNetwork();
	});

	public void AddConduitPortToNetwork()
	{
		if (!(conduitPort.conduitDispenser == null))
		{
			int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), conduitPortInfo.offset);
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(conduitPortInfo.conduitType);
			conduitPort.outputCell = num;
			conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(conduitPortInfo.conduitType, Endpoint.Source, num, base.gameObject);
			networkManager.AddToNetworks(num, conduitPort.networkItem, is_endpoint: true);
		}
	}

	public void RemoveConduitPortFromNetwork()
	{
		if (!(conduitPort.conduitDispenser == null))
		{
			Conduit.GetNetworkManager(conduitPortInfo.conduitType).RemoveFromNetworks(conduitPort.outputCell, conduitPort.networkItem, is_endpoint: true);
		}
	}

	private bool CanTransferFromSender()
	{
		bool result = false;
		if ((base.smi.master.senderConduitStorage.MassStored() > 0f || base.smi.master.senderConduitStorage.items.Count > 0) && base.smi.master.conduitPort.conduitDispenser.GetConduitManager().GetPermittedFlow(base.smi.master.conduitPort.outputCell) != 0)
		{
			result = true;
		}
		return result;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		FindPartner();
		Subscribe(-1118736034, TryFindPartner);
		Subscribe(546421097, OnLaunchedDelegate);
		Subscribe(-735346771, OnLandedDelegate);
		base.smi.StartSM();
		Components.RocketConduitReceivers.Add(this);
	}

	protected override void OnCleanUp()
	{
		RemoveConduitPortFromNetwork();
		base.OnCleanUp();
		Components.RocketConduitReceivers.Remove(this);
	}

	private void FindPartner()
	{
		if (senderConduitStorage != null)
		{
			return;
		}
		RocketConduitSender rocketConduitSender = null;
		WorldContainer world = ClusterManager.Instance.GetWorld(base.gameObject.GetMyWorldId());
		if (world != null && world.IsModuleInterior)
		{
			RocketConduitSender[] components = world.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().GetComponents<RocketConduitSender>();
			foreach (RocketConduitSender rocketConduitSender2 in components)
			{
				if (rocketConduitSender2.conduitPortInfo.conduitType == conduitPortInfo.conduitType)
				{
					rocketConduitSender = rocketConduitSender2;
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
				foreach (RocketConduitSender worldItem in Components.RocketConduitSenders.GetWorldItems(targetWorld.id))
				{
					if (worldItem.conduitPortInfo.conduitType == conduitPortInfo.conduitType)
					{
						rocketConduitSender = worldItem;
						break;
					}
				}
			}
		}
		if (rocketConduitSender == null)
		{
			Debug.LogWarning("No warp conduit sender found?");
		}
		else
		{
			SetStorage(rocketConduitSender.conduitStorage);
		}
	}

	public void SetStorage(Storage conduitStorage)
	{
		senderConduitStorage = conduitStorage;
		conduitPort.SetPortInfo(base.gameObject, conduitPortInfo, conduitStorage);
		if (base.gameObject.GetMyWorld() != null)
		{
			AddConduitPortToNetwork();
		}
	}

	bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type)
	{
		return type == conduitPortInfo.conduitType;
	}

	CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == conduitPortInfo.conduitType)
		{
			return conduitPortInfo.offset;
		}
		return CellOffset.none;
	}
}
