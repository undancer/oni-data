using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RailGun : StateMachineComponent<RailGun.StatesInstance>, ISim200ms, ISecondaryInput
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RailGun, object>.GameInstance
	{
		public const int INVALID_PATH_LENGTH = -1;

		private List<AxialI> m_cachedPath;

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
			return base.smi.master.particleStorage.Particles > EnergyCost();
		}

		public bool HasDestination()
		{
			return base.smi.master.destinationSelector.GetDestinationWorld() != base.smi.master.GetMyWorldId();
		}

		public bool IsDestinationReachable(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				UpdatePath();
			}
			if (base.smi.master.destinationSelector.GetDestinationWorld() != base.smi.master.GetMyWorldId())
			{
				return PathLength() != -1;
			}
			return false;
		}

		public int PathLength()
		{
			if (base.smi.m_cachedPath == null)
			{
				UpdatePath();
			}
			if (base.smi.m_cachedPath == null)
			{
				return -1;
			}
			int num = base.smi.m_cachedPath.Count;
			if (base.master.FreeStartHex)
			{
				num--;
			}
			if (base.master.FreeDestinationHex)
			{
				num--;
			}
			return num;
		}

		public void UpdatePath()
		{
			m_cachedPath = ClusterGrid.Instance.GetPath(base.gameObject.GetMyWorldLocation(), base.smi.master.destinationSelector.GetDestination(), base.smi.master.destinationSelector);
		}

		public float EnergyCost()
		{
			return Mathf.Max(0f, 10f + (float)PathLength() * 10f);
		}

		public bool MayTurnOn()
		{
			if (HasEnergy() && IsDestinationReachable() && base.master.operational.IsOperational)
			{
				return base.sm.allowedFromLogic.Get(this);
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RailGun>
	{
		public class WorkingStates : State
		{
			public State pre;

			public State loop;

			public State fire;

			public State bounce;

			public State pst;
		}

		public class CooldownStates : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public class OnStates : State
		{
			public State power_on;

			public State wait_for_storage;

			public State power_off;

			public WorkingStates working;

			public CooldownStates cooldown;
		}

		public State off;

		public OnStates on;

		public FloatParameter cooldownTimer;

		public IntParameter payloadsFiredSinceCooldown;

		public BoolParameter allowedFromLogic;

		public BoolParameter updatePath;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			base.serializable = SerializeType.ParamsOnly;
			root.EventHandler(GameHashes.ClusterDestinationChanged, delegate(StatesInstance smi)
			{
				smi.UpdatePath();
			});
			off.PlayAnim("off").EventTransition(GameHashes.OnParticleStorageChanged, on, (StatesInstance smi) => smi.MayTurnOn()).EventTransition(GameHashes.ClusterDestinationChanged, on, (StatesInstance smi) => smi.MayTurnOn())
				.EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.MayTurnOn())
				.ParamTransition(allowedFromLogic, on, (StatesInstance smi, bool p) => smi.MayTurnOn());
			on.DefaultState(on.power_on).EventTransition(GameHashes.OperationalChanged, on.power_off, (StatesInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.ClusterDestinationChanged, on.power_off, (StatesInstance smi) => !smi.IsDestinationReachable())
				.EventTransition(GameHashes.ClusterFogOfWarRevealed, (StatesInstance smi) => Game.Instance, on.power_off, (StatesInstance smi) => !smi.IsDestinationReachable(forceRefresh: true))
				.ParamTransition(allowedFromLogic, on.power_off, (StatesInstance smi, bool p) => !p)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
			on.power_on.PlayAnim("power_on").OnAnimQueueComplete(on.wait_for_storage);
			on.power_off.PlayAnim("power_off").OnAnimQueueComplete(off);
			on.wait_for_storage.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.ClusterDestinationChanged, on.power_off, (StatesInstance smi) => !smi.HasEnergy()).EventTransition(GameHashes.OnStorageChange, on.working, (StatesInstance smi) => smi.HasResources() && smi.sm.cooldownTimer.Get(smi) <= 0f)
				.EventTransition(GameHashes.OperationalChanged, on.working, (StatesInstance smi) => smi.HasResources() && smi.sm.cooldownTimer.Get(smi) <= 0f)
				.ParamTransition(cooldownTimer, on.cooldown, (StatesInstance smi, float p) => p > 0f);
			on.working.DefaultState(on.working.pre).Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			});
			on.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(on.working.loop);
			on.working.loop.PlayAnim("working_loop").OnAnimQueueComplete(on.working.fire);
			on.working.fire.Enter(delegate(StatesInstance smi)
			{
				if (smi.IsDestinationReachable())
				{
					smi.master.LaunchProjectile();
					smi.sm.payloadsFiredSinceCooldown.Delta(1, smi);
					if (smi.sm.payloadsFiredSinceCooldown.Get(smi) >= 6)
					{
						smi.sm.cooldownTimer.Set(30f, smi);
					}
				}
			}).GoTo(on.working.bounce);
			on.working.bounce.ParamTransition(cooldownTimer, on.working.pst, (StatesInstance smi, float p) => p > 0f || !smi.HasResources()).ParamTransition(payloadsFiredSinceCooldown, on.working.loop, (StatesInstance smi, int p) => p < 6 && smi.HasResources());
			on.working.pst.PlayAnim("working_pst").OnAnimQueueComplete(on.wait_for_storage);
			on.cooldown.DefaultState(on.cooldown.pre).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailGunCooldown);
			on.cooldown.pre.PlayAnim("cooldown_pre").OnAnimQueueComplete(on.cooldown.loop);
			on.cooldown.loop.PlayAnim("cooldown_loop", KAnim.PlayMode.Loop).ParamTransition(cooldownTimer, on.cooldown.pst, (StatesInstance smi, float p) => p <= 0f).Update(delegate(StatesInstance smi, float dt)
			{
				cooldownTimer.Delta(0f - dt, smi);
			}, UpdateRate.SIM_1000ms);
			on.cooldown.pst.PlayAnim("cooldown_pst").OnAnimQueueComplete(on.wait_for_storage).Exit(delegate(StatesInstance smi)
			{
				smi.sm.payloadsFiredSinceCooldown.Set(0, smi);
			});
		}
	}

	[Serialize]
	public float launchMass = 200f;

	public float MinLaunchMass = 2f;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KAnimControllerBase kac;

	[MyCmpGet]
	public HighEnergyParticleStorage hepStorage;

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

	public static readonly HashedString PORT_ID = "LogicLaunching";

	private bool hasLogicWire;

	private bool isLogicActive;

	private static StatusItem infoStatusItemLogic;

	public bool FreeStartHex;

	public bool FreeDestinationHex;

	private static readonly EventSystem.IntraObjectHandler<RailGun> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RailGun>(delegate(RailGun component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public float MaxLaunchMass => 200f;

	public float EnergyCost => base.smi.EnergyCost();

	public float CurrentEnergy => hepStorage.Particles;

	public bool AllowLaunchingFromLogic
	{
		get
		{
			if (hasLogicWire)
			{
				if (hasLogicWire)
				{
					return isLogicActive;
				}
				return false;
			}
			return true;
		}
	}

	public bool HasLogicWire => hasLogicWire;

	public bool IsLogicActive => isLogicActive;

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
		if (infoStatusItemLogic == null)
		{
			infoStatusItemLogic = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItemLogic.resolveStringCallback = ResolveInfoStatusItemString;
		}
		CheckLogicWireState();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(liquidPortInfo.conduitType).RemoveFromNetworks(liquidInputCell, liquidNetworkItem, is_endpoint: true);
		Conduit.GetNetworkManager(gasPortInfo.conduitType).RemoveFromNetworks(gasInputCell, gasNetworkItem, is_endpoint: true);
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
		if (liquidPortInfo.conduitType != type && gasPortInfo.conduitType != type)
		{
			return solidPortInfo.conduitType == type;
		}
		return true;
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

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = GetComponent<LogicPorts>().GetPortCell(PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void CheckLogicWireState()
	{
		LogicCircuitNetwork network = GetNetwork();
		hasLogicWire = network != null;
		int value = network?.OutputValue ?? 1;
		bool flag = (isLogicActive = LogicCircuitNetwork.IsBitActive(0, value));
		base.smi.sm.allowedFromLogic.Set(AllowLaunchingFromLogic, base.smi);
		GetComponent<KSelectable>().ToggleStatusItem(infoStatusItemLogic, network != null, this);
	}

	private void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == PORT_ID)
		{
			CheckLogicWireState();
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		RailGun obj = (RailGun)data;
		_ = obj.operational;
		return obj.AllowLaunchingFromLogic ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED;
	}

	public void Sim200ms(float dt)
	{
		WorldContainer myWorld = this.GetMyWorld();
		Extents extents = GetComponent<Building>().GetExtents();
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
		KSelectable component = GetComponent<KSelectable>();
		component.ToggleStatusItem(noSurfaceSightStatusItem, !flag);
		component.ToggleStatusItem(noDestinationStatusItem, destinationSelector.GetDestinationWorld() < 0);
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
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), Grid.CellToPosCBC(cell, Grid.SceneLayer.Front));
		for (float num = 0f; num < launchMass; num += resourceStorage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, launchMass - num, block_events: false, hide_popups: true))
		{
			if (!(resourceStorage.MassStored() > 0f))
			{
				break;
			}
		}
		particleStorage.ConsumeAndGet(base.smi.EnergyCost());
		gameObject.SetActive(value: true);
		if (destinationSelector.GetDestinationWorld() >= 0)
		{
			RailGunPayload.StatesInstance sMI = gameObject.GetSMI<RailGunPayload.StatesInstance>();
			sMI.takeoffVelocity = 35f;
			sMI.StartSM();
			sMI.Launch(base.gameObject.GetMyWorldLocation(), destinationSelector.GetDestination());
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
