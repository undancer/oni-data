using System.Collections.Generic;
using System.Linq;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RocketControlStation : StateMachineComponent<RocketControlStation.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, RocketControlStation>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;

			public State post;

			public State warning;

			public State autopilot;
		}

		public class LaunchStates : State
		{
			public State launch;

			public State fadein;
		}

		public TargetParameter clusterCraft;

		private State unoperational;

		private State operational;

		private State running;

		private ReadyStates ready;

		private LaunchStates launch;

		public Signal pilotSuccessful;

		public FloatParameter timeRemaining;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.ParamsOnly;
			default_state = unoperational;
			root.Enter("SetTarget", delegate(StatesInstance smi)
			{
				clusterCraft.Set(GetRocket(smi), smi);
				clusterCraft.Get(smi).Subscribe(-1582839653, smi.master.OnTagsChanged);
			}).Target(masterTarget).Exit(delegate(StatesInstance smi)
			{
				SetRocketSpeed(smi, 0.5f);
			});
			unoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational);
			operational.Enter(delegate(StatesInstance smi)
			{
				SetRocketSpeed(smi, 1f);
			}).PlayAnim("on").TagTransition(GameTags.Operational, unoperational, on_remove: true)
				.Transition(ready, IsInFlight, UpdateRate.SIM_4000ms)
				.Target(clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, launch, RocketReadyForLaunch)
				.EventTransition(GameHashes.LaunchConditionChanged, launch, RocketReadyForLaunch)
				.Target(masterTarget)
				.Exit(delegate(StatesInstance smi)
				{
					timeRemaining.Set(120f, smi);
				});
			launch.Enter(delegate(StatesInstance smi)
			{
				SetRocketSpeed(smi, 1f);
			}).ToggleChore(CreateLaunchChore, operational).Transition(launch.fadein, IsInFlight)
				.Target(clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, operational, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(RocketReadyForLaunch))
				.EventTransition(GameHashes.LaunchConditionChanged, launch, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(RocketReadyForLaunch))
				.Target(masterTarget);
			launch.fadein.Enter(delegate(StatesInstance smi)
			{
				if (CameraController.Instance.cameraActiveCluster == clusterCraft.Get(smi).GetComponent<WorldContainer>().id)
				{
					CameraController.Instance.FadeIn();
				}
			});
			running.PlayAnim("on").TagTransition(GameTags.Operational, unoperational, on_remove: true).Transition(operational, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(IsInFlight))
				.ParamTransition(timeRemaining, ready, (StatesInstance smi, float p) => p <= 0f)
				.Enter(delegate(StatesInstance smi)
				{
					SetRocketSpeed(smi, 1f);
				})
				.Update("Decrement time", DecrementTime)
				.Exit(delegate(StatesInstance smi)
				{
					timeRemaining.Set(30f, smi);
				});
			ready.TagTransition(GameTags.Operational, unoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, ready.post, ready)
				.Transition(operational, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(IsInFlight))
				.OnSignal(pilotSuccessful, ready.post)
				.Update("Decrement time", DecrementTime);
			ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working).ParamTransition(timeRemaining, ready.warning, (StatesInstance smi, float p) => p <= 15f);
			ready.warning.PlayAnim("on_alert", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working).ToggleMainStatusItem(Db.Get().BuildingStatusItems.PilotNeeded)
				.ParamTransition(timeRemaining, ready.autopilot, (StatesInstance smi, float p) => p <= 0f);
			ready.autopilot.PlayAnim("on_failed", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().BuildingStatusItems.AutoPilotActive).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working)
				.Enter(delegate(StatesInstance smi)
				{
					SetRocketSpeed(smi, 0.5f);
				});
			ready.working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).Enter(delegate(StatesInstance smi)
			{
				SetRocketSpeed(smi, 1f);
			})
				.WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.idle);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(running).Exit(delegate(StatesInstance smi)
			{
				timeRemaining.Set(120f, smi);
			});
		}

		private void DecrementTime(StatesInstance smi, float dt)
		{
			timeRemaining.Delta(0f - dt, smi);
		}

		private bool RocketReadyForLaunch(StatesInstance smi)
		{
			Clustercraft component = clusterCraft.Get(smi).GetComponent<Clustercraft>();
			if (component.LaunchRequested)
			{
				return component.CheckReadyToLaunch();
			}
			return false;
		}

		private GameObject GetRocket(StatesInstance smi)
		{
			return ClusterManager.Instance.GetWorld(smi.GetMyWorldId()).gameObject.GetComponent<Clustercraft>().gameObject;
		}

		private void SetRocketSpeed(StatesInstance smi, float speed_multiplier)
		{
			clusterCraft.Get(smi).GetComponent<Clustercraft>().AutoPilotMultiplier = speed_multiplier;
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationIdleWorkable>();
			WorkChore<RocketControlStationIdleWorkable> workChore = new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Work, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			return workChore;
		}

		private Chore CreateLaunchChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationLaunchWorkable>();
			WorkChore<RocketControlStationLaunchWorkable> workChore = new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: true, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.topPriority);
			workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			return workChore;
		}

		public void LaunchRocket(StatesInstance smi)
		{
			clusterCraft.Get(smi).GetComponent<Clustercraft>().Launch();
		}

		public bool IsInFlight(StatesInstance smi)
		{
			return clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.InFlight;
		}

		public bool IsLaunching(StatesInstance smi)
		{
			return clusterCraft.Get(smi).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Launching;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketControlStation, object>.GameInstance
	{
		public StatesInstance(RocketControlStation smi)
			: base(smi)
		{
		}

		public void LaunchRocket()
		{
			base.sm.LaunchRocket(this);
		}
	}

	public static List<Tag> CONTROLLED_BUILDINGS = new List<Tag>();

	private const int UNNETWORKED_VALUE = 1;

	[Serialize]
	public float TimeRemaining;

	private int m_logicUsageRestrictionState;

	[Serialize]
	private bool m_restrictWhenGrounded;

	public static readonly HashedString PORT_ID = "LogicUsageRestriction";

	private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RocketControlStation>(delegate(RocketControlStation component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketControlStation> OnRocketRestrictionChanged = new EventSystem.IntraObjectHandler<RocketControlStation>(delegate(RocketControlStation component, object data)
	{
		component.UpdateRestrictionAnimSymbol(data);
	});

	public bool RestrictWhenGrounded
	{
		get
		{
			return m_restrictWhenGrounded;
		}
		set
		{
			m_restrictWhenGrounded = value;
			Trigger(1861523068);
		}
	}

	public bool BuildingRestrictionsActive
	{
		get
		{
			if (IsLogicInputConnected())
			{
				return m_logicUsageRestrictionState == 1;
			}
			GameObject gameObject = base.smi.sm.clusterCraft.Get(base.smi);
			if (RestrictWhenGrounded && gameObject != null)
			{
				return gameObject.gameObject.HasTag(GameTags.RocketOnGround);
			}
			return false;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.RocketControlStations.Add(this);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(1861523068, OnRocketRestrictionChanged);
		CheckWireState();
		UpdateRestrictionAnimSymbol();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RocketControlStations.Remove(this);
	}

	public bool IsLogicInputConnected()
	{
		return GetNetwork() != null;
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			m_logicUsageRestrictionState = logicValueChanged.newValue;
			Trigger(1861523068);
		}
	}

	public void OnTagsChanged(object obj)
	{
		if (((TagChangedEventData)obj).tag == GameTags.RocketOnGround)
		{
			Trigger(1861523068);
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = GetComponent<LogicPorts>().GetPortCell(PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private LogicCircuitNetwork CheckWireState()
	{
		LogicCircuitNetwork network = GetNetwork();
		int num = network?.OutputValue ?? 1;
		if (num != m_logicUsageRestrictionState)
		{
			m_logicUsageRestrictionState = num;
			Trigger(1861523068);
		}
		return network;
	}

	private void UpdateRestrictionAnimSymbol(object o = null)
	{
		GetComponent<KAnimControllerBase>().SetSymbolVisiblity("restriction_sign", BuildingRestrictionsActive);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> obj = new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.ROCKETRESTRICTION_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_HEADER)
		};
		string newValue = string.Join(", ", CONTROLLED_BUILDINGS.Select((Tag t) => Strings.Get("STRINGS.BUILDINGS.PREFABS." + t.Name.ToUpper() + ".NAME").String).ToArray());
		obj.Add(new Descriptor(UI.BUILDINGEFFECTS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", newValue), UI.BUILDINGEFFECTS.TOOLTIPS.ROCKETRESTRICTION_BUILDINGS.text.Replace("{buildinglist}", newValue)));
		return obj;
	}
}
