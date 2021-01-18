using KSerialization;
using UnityEngine;

public class RailGun : StateMachineComponent<RailGun.StatesInstance>, ISim200ms, IUserControlledCapacity, ISecondaryInput
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RailGun, object>.GameInstance
	{
		public StatesInstance(RailGun smi)
			: base(smi)
		{
		}

		public bool HasResources()
		{
			return base.smi.master.resourceStorage.MassStored() >= base.smi.master.launchMass;
		}

		public bool HasEnergy()
		{
			return base.smi.master.particleStorage.Particles > 200f;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RailGun>
	{
		public class PowerStates : State
		{
			public State on;

			public State off;

			public State power_on;

			public State power_off;
		}

		public class WorkingStates : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public PowerStates power;

		public WorkingStates working;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = power.off;
			base.serializable = SerializeType.Both_DEPRECATED;
			power.off.PlayAnim("off").EventTransition(GameHashes.OnParticleStorageChanged, power.power_on, (StatesInstance smi) => smi.HasEnergy());
			power.power_on.PlayAnim("power_on").OnAnimQueueComplete(power.on);
			power.on.PlayAnim("on").EventTransition(GameHashes.OnParticleStorageEmpty, power.power_off).EventTransition(GameHashes.OnStorageChange, working.pre, (StatesInstance smi) => smi.HasResources() && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, working.pre, (StatesInstance smi) => smi.HasResources() && smi.master.operational.IsOperational);
			power.power_off.PlayAnim("power_off").OnAnimQueueComplete(power.off);
			working.pre.PlayAnim("working_pre").OnAnimQueueComplete(working.loop);
			working.loop.PlayAnim("working_loop").OnAnimQueueComplete(working.pst).Exit(delegate(StatesInstance smi)
			{
				smi.master.LaunchProjectile();
			});
			working.pst.PlayAnim("working_pst").Enter(delegate(StatesInstance smi)
			{
				if (!smi.HasEnergy())
				{
					smi.GoTo(power.power_off);
				}
				else if (!smi.HasResources())
				{
					smi.GoTo(power.on);
				}
				else
				{
					smi.GoTo(working.pre);
				}
			});
		}
	}

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	public float launchMass = 200f;

	public float MinLaunchMass = 2f;

	[MyCmpGet]
	private Operational operational;

	public Storage resourceStorage;

	private MeterController resourceMeter;

	private HighEnergyParticleStorage particleStorage;

	private MeterController particleMeter;

	private ClusterDestinationSelector destinationSelector;

	public static readonly Operational.Flag noSurfaceSight = new Operational.Flag("noSurfaceSight", Operational.Flag.Type.Requirement);

	private static StatusItem noSurfaceSightStatusItem;

	public static readonly Operational.Flag noDestination = new Operational.Flag("noDestination", Operational.Flag.Type.Requirement);

	private static StatusItem noDestinationStatusItem;

	[SerializeField]
	public ConduitPortInfo liquidPortInfo;

	private int liquidInputCell = -1;

	private FlowUtilityNetwork.NetworkItem liquidNetworkItem;

	private ConduitConsumer liquidConsumer;

	[SerializeField]
	public ConduitPortInfo gasPortInfo;

	private int gasInputCell = -1;

	private FlowUtilityNetwork.NetworkItem gasNetworkItem;

	private ConduitConsumer gasConsumer;

	[SerializeField]
	public ConduitPortInfo solidPortInfo;

	private int solidInputCell = -1;

	private FlowUtilityNetwork.NetworkItem solidNetworkItem;

	private SolidConduitConsumer solidConsumer;

	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, GetComponent<Storage>().capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			resourceStorage.capacityKg = value;
		}
	}

	public float AmountStored => GetComponent<Storage>().MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => GetComponent<Storage>().capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	public float MaxLaunchMass => 200f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		destinationSelector = GetComponent<ClusterDestinationSelector>();
		resourceStorage = GetComponent<Storage>();
		particleStorage = GetComponent<HighEnergyParticleStorage>();
		if (noSurfaceSightStatusItem == null)
		{
			noSurfaceSightStatusItem = new StatusItem("RAILGUN_PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		}
		if (noDestinationStatusItem == null)
		{
			noDestinationStatusItem = new StatusItem("RAILGUN_NO_DESTINATION", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		}
		gasInputCell = Grid.OffsetCell(Grid.PosToCell(this), gasPortInfo.offset);
		gasConsumer = CreateConduitConsumer(ConduitType.Gas, gasInputCell, out gasNetworkItem);
		liquidInputCell = Grid.OffsetCell(Grid.PosToCell(this), liquidPortInfo.offset);
		liquidConsumer = CreateConduitConsumer(ConduitType.Liquid, liquidInputCell, out liquidNetworkItem);
		solidInputCell = Grid.OffsetCell(Grid.PosToCell(this), solidPortInfo.offset);
		solidConsumer = CreateSolidConduitConsumer(solidInputCell, out solidNetworkItem);
		CreateMeters();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(liquidPortInfo.conduitType);
		networkManager.RemoveFromNetworks(liquidInputCell, liquidNetworkItem, is_endpoint: true);
		networkManager = Conduit.GetNetworkManager(gasPortInfo.conduitType);
		networkManager.RemoveFromNetworks(gasInputCell, gasNetworkItem, is_endpoint: true);
		Game.Instance.solidConduitSystem.RemoveFromNetworks(solidInputCell, solidConsumer, is_endpoint: true);
		base.OnCleanUp();
	}

	private void CreateMeters()
	{
		resourceMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_storage_target", "meter_storage", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
		particleMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_orb_target", "meter_orb", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
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

	public void Sim200ms(float dt)
	{
		WorldContainer myWorld = this.GetMyWorld();
		Building component = GetComponent<Building>();
		Extents extents = component.GetExtents();
		int x = extents.x;
		int x2 = extents.x + extents.width - 2;
		int y = extents.y + extents.height;
		int num = Grid.XYToCell(x, y);
		int num2 = Grid.XYToCell(x2, y);
		bool flag = true;
		for (int i = num; i <= num2; i++)
		{
			int num3 = i;
			while (Grid.CellRow(num3) < myWorld.Height)
			{
				if (!Grid.IsValidCell(num3) || Grid.Solid[num3])
				{
					flag = false;
				}
				num3 = Grid.CellAbove(num3);
			}
		}
		operational.SetFlag(noSurfaceSight, flag);
		operational.SetFlag(noDestination, destinationSelector.GetDestinationWorld() >= 0);
		KSelectable component2 = GetComponent<KSelectable>();
		component2.ToggleStatusItem(noSurfaceSightStatusItem, !flag);
		component2.ToggleStatusItem(noDestinationStatusItem, destinationSelector.GetDestinationWorld() < 0);
		UpdateMeters();
	}

	private void UpdateMeters()
	{
		resourceMeter.SetPositionPercent(Mathf.Clamp01(resourceStorage.MassStored() / resourceStorage.capacityKg));
		particleMeter.SetPositionPercent(Mathf.Clamp01(particleStorage.Particles / particleStorage.capacity));
	}

	private void LaunchProjectile()
	{
		Extents extents = GetComponent<Building>().GetExtents();
		Vector2I vector2I = Grid.PosToXY(base.transform.position);
		vector2I.y += extents.height + 1;
		int cell = Grid.XYToCell(vector2I.x, vector2I.y);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), Grid.CellToPos(cell));
		for (float num = 0f; num < launchMass; num += resourceStorage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, launchMass - num, block_events: false, hide_popups: true))
		{
			if (!(resourceStorage.MassStored() > 0f))
			{
				break;
			}
		}
		particleStorage.ConsumeAndGet(200f);
		gameObject.SetActive(value: true);
		if (destinationSelector.GetDestinationWorld() >= 0)
		{
			gameObject.GetSMI<RailGunPayload.StatesInstance>().Launch(destinationSelector.GetDestinationWorld());
		}
	}

	private ConduitConsumer CreateConduitConsumer(ConduitType inputType, int inputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		ConduitConsumer conduitConsumer = base.gameObject.AddComponent<ConduitConsumer>();
		conduitConsumer.conduitType = inputType;
		conduitConsumer.useSecondaryInput = true;
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(inputType);
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(inputType, Endpoint.Sink, inputCell, base.gameObject);
		networkManager.AddToNetworks(inputCell, flowNetworkItem, is_endpoint: true);
		return conduitConsumer;
	}

	private SolidConduitConsumer CreateSolidConduitConsumer(int inputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		SolidConduitConsumer solidConduitConsumer = base.gameObject.AddComponent<SolidConduitConsumer>();
		solidConduitConsumer.useSecondaryInput = true;
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, inputCell, base.gameObject);
		Game.Instance.solidConduitSystem.AddToNetworks(inputCell, flowNetworkItem, is_endpoint: true);
		return solidConduitConsumer;
	}
}
