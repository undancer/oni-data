using UnityEngine;

public class WarpConduitSender : StateMachineComponent<WarpConduitSender.StatesInstance>, ISecondaryInput
{
	private class ConduitPort
	{
		public ConduitPortInfo portInfo;

		public int inputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		private ConduitConsumer conduitConsumer;

		public SolidConduitConsumer solidConsumer;

		public MeterController airlock;

		private bool open;

		private string pre;

		private string loop;

		private string pst;

		public ConduitPort(GameObject parent, ConduitPortInfo info, int number, Storage targetStorage)
		{
			portInfo = info;
			inputCell = Grid.OffsetCell(Grid.PosToCell(parent), portInfo.offset);
			if (portInfo.conduitType != ConduitType.Solid)
			{
				ConduitConsumer conduitConsumer = parent.AddComponent<ConduitConsumer>();
				conduitConsumer.conduitType = portInfo.conduitType;
				conduitConsumer.useSecondaryInput = true;
				conduitConsumer.storage = targetStorage;
				conduitConsumer.capacityKG = targetStorage.capacityKg;
				conduitConsumer.alwaysConsume = false;
				this.conduitConsumer = conduitConsumer;
				this.conduitConsumer.keepZeroMassObject = false;
				IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(portInfo.conduitType);
				networkItem = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Sink, inputCell, parent);
				networkManager.AddToNetworks(inputCell, networkItem, is_endpoint: true);
			}
			else
			{
				solidConsumer = parent.AddComponent<SolidConduitConsumer>();
				solidConsumer.useSecondaryInput = true;
				solidConsumer.storage = targetStorage;
				networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, inputCell, parent);
				Game.Instance.solidConduitSystem.AddToNetworks(inputCell, networkItem, is_endpoint: true);
			}
			string meter_animation = "airlock_" + number;
			string text = "airlock_target_" + number;
			pre = "airlock_" + number + "_pre";
			loop = "airlock_" + number + "_loop";
			pst = "airlock_" + number + "_pst";
			airlock = new MeterController(parent.GetComponent<KBatchedAnimController>(), text, meter_animation, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, text);
		}

		public void Update()
		{
			bool flag = false;
			if (conduitConsumer != null)
			{
				flag = conduitConsumer.IsConnected && conduitConsumer.IsSatisfied && conduitConsumer.consumedLastTick;
			}
			else if (solidConsumer != null)
			{
				flag = solidConsumer.IsConnected && solidConsumer.IsConsuming;
			}
			if (flag != open)
			{
				open = flag;
				if (open)
				{
					airlock.meterController.Play(pre);
					airlock.meterController.Queue(loop, KAnim.PlayMode.Loop);
				}
				else
				{
					airlock.meterController.Play(pst);
				}
			}
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, WarpConduitSender, object>.GameInstance
	{
		public StatesInstance(WarpConduitSender smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, WarpConduitSender>
	{
		public class onStates : State
		{
			public State working;

			public State waiting;
		}

		public State off;

		public onStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.EventHandler(GameHashes.BuildingActivated, delegate(StatesInstance smi, object data)
			{
				smi.master.OnActivatedChanged(data);
			});
			off.PlayAnim("off").Enter(delegate(StatesInstance smi)
			{
				smi.master.gasPort.Update();
				smi.master.liquidPort.Update();
				smi.master.solidPort.Update();
			}).EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			on.DefaultState(on.waiting).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.gasPort.Update();
				smi.master.liquidPort.Update();
				smi.master.solidPort.Update();
			});
			on.working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working)
				.Exit(delegate(StatesInstance smi)
				{
					smi.Play("working_pst");
				});
			on.waiting.QueueAnim("idle").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).EventTransition(GameHashes.OnStorageChange, on.working, (StatesInstance smi) => smi.GetComponent<Storage>().MassStored() > 0f);
		}
	}

	[MyCmpReq]
	private Operational operational;

	public Storage gasStorage;

	public Storage liquidStorage;

	public Storage solidStorage;

	public WarpConduitReceiver receiver;

	[SerializeField]
	public ConduitPortInfo liquidPortInfo;

	private ConduitPort liquidPort;

	[SerializeField]
	public ConduitPortInfo gasPortInfo;

	private ConduitPort gasPort;

	[SerializeField]
	public ConduitPortInfo solidPortInfo;

	private ConduitPort solidPort;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage[] components = GetComponents<Storage>();
		gasStorage = components[0];
		liquidStorage = components[1];
		solidStorage = components[2];
		gasPort = new ConduitPort(base.gameObject, gasPortInfo, 1, gasStorage);
		liquidPort = new ConduitPort(base.gameObject, liquidPortInfo, 2, liquidStorage);
		solidPort = new ConduitPort(base.gameObject, solidPortInfo, 3, solidStorage);
		Vector3 position = liquidPort.airlock.gameObject.transform.position;
		liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().transform.position = position + new Vector3(0f, 0f, -0.1f);
		liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = false;
		liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = true;
		FindPartner();
		WarpConduitStatus.UpdateWarpConduitsOperational(base.gameObject, (receiver != null) ? receiver.gameObject : null);
		base.smi.StartSM();
	}

	public void OnActivatedChanged(object data)
	{
		WarpConduitStatus.UpdateWarpConduitsOperational(base.gameObject, (receiver != null) ? receiver.gameObject : null);
	}

	private void FindPartner()
	{
		SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag("WarpConduitReceiver");
		WarpConduitReceiver[] array = Object.FindObjectsOfType<WarpConduitReceiver>();
		WarpConduitReceiver[] array2 = array;
		foreach (WarpConduitReceiver component in array2)
		{
			if (component.GetMyWorldId() != this.GetMyWorldId())
			{
				receiver = component;
				break;
			}
		}
		if (receiver == null)
		{
			Debug.LogWarning("No warp conduit receiver found - maybe POI stomping or failure to spawn?");
		}
		else
		{
			receiver.SetStorage(gasStorage, liquidStorage, solidStorage);
		}
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(liquidPortInfo.conduitType);
		networkManager.RemoveFromNetworks(liquidPort.inputCell, liquidPort.networkItem, is_endpoint: true);
		networkManager = Conduit.GetNetworkManager(gasPortInfo.conduitType);
		networkManager.RemoveFromNetworks(gasPort.inputCell, gasPort.networkItem, is_endpoint: true);
		Game.Instance.solidConduitSystem.RemoveFromNetworks(solidPort.inputCell, solidPort.solidConsumer, is_endpoint: true);
		base.OnCleanUp();
	}

	bool ISecondaryInput.HasSecondaryConduitType(ConduitType type)
	{
		return liquidPortInfo.conduitType == type || gasPortInfo.conduitType == type || solidPortInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (liquidPortInfo.conduitType == type)
		{
			return liquidPortInfo.offset;
		}
		if (gasPortInfo.conduitType == type)
		{
			return gasPortInfo.offset;
		}
		if (solidPortInfo.conduitType == type)
		{
			return solidPortInfo.offset;
		}
		return CellOffset.none;
	}
}
