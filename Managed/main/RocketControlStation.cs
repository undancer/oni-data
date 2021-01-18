using KSerialization;
using UnityEngine;

public class RocketControlStation : StateMachineComponent<RocketControlStation.StatesInstance>
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

		private TargetParameter clusterCraft;

		private State unoperational;

		private State operational;

		private State running;

		private ReadyStates ready;

		private LaunchStates launch;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			root.Enter("SetTarget", delegate(StatesInstance smi)
			{
				clusterCraft.Set(GetRocket(smi), smi);
			});
			unoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational);
			operational.PlayAnim("on").TagTransition(GameTags.Operational, unoperational, on_remove: true).Transition(ready, IsInFlight, UpdateRate.SIM_4000ms)
				.Target(clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, launch, (StatesInstance smi) => clusterCraft.Get(smi).GetComponent<Clustercraft>().LaunchRequested)
				.Target(masterTarget);
			launch.ToggleChore(CreateLaunchChore, operational).Transition(launch.fadein, IsInFlight).Target(clusterCraft)
				.EventTransition(GameHashes.RocketRequestLaunch, operational, (StatesInstance smi) => !clusterCraft.Get(smi).GetComponent<Clustercraft>().LaunchRequested)
				.Target(masterTarget);
			launch.fadein.Enter(delegate(StatesInstance smi)
			{
				if (CameraController.Instance.cameraActiveCluster == clusterCraft.Get(smi).GetComponent<WorldContainer>().id)
				{
					CameraController.Instance.FadeIn();
				}
			});
			running.PlayAnim("on").TagTransition(GameTags.Operational, unoperational, on_remove: true).Transition(operational, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(IsInFlight))
				.ScheduleGoTo(120f, ready);
			ready.TagTransition(GameTags.Operational, unoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, running)
				.Transition(operational, GameStateMachine<States, StatesInstance, RocketControlStation, object>.Not(IsInFlight));
			ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working).ScheduleGoTo(30f, ready.warning)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.TimeRemaining = 30f;
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.TimeRemaining -= dt;
				});
			ready.working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).Enter(delegate(StatesInstance smi)
			{
				SetRocketSpeed(smi, 1f);
			})
				.WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.post);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(running);
			ready.warning.PlayAnim("on_alert").WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working).ToggleMainStatusItem(Db.Get().BuildingStatusItems.PilotNeeded)
				.ScheduleGoTo(30f, ready.autopilot)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.TimeRemaining = 30f;
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.TimeRemaining -= dt;
				});
			ready.autopilot.PlayAnim("on_failed").ToggleMainStatusItem(Db.Get().BuildingStatusItems.AutoPilotActive).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<RocketControlStationIdleWorkable>(), ready.working)
				.Enter(delegate(StatesInstance smi)
				{
					SetRocketSpeed(smi, 0.5f);
				});
		}

		private GameObject GetRocket(StatesInstance smi)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(smi.GetMyWorldId());
			return world.gameObject.GetComponent<Clustercraft>().gameObject;
		}

		private void SetRocketSpeed(StatesInstance smi, float speed_multiplier)
		{
			clusterCraft.Get(smi).GetComponent<Clustercraft>().AutoPilotMultiplier = speed_multiplier;
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationIdleWorkable>();
			Chore chore = new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Work, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			return chore;
		}

		private Chore CreateLaunchChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<RocketControlStationLaunchWorkable>();
			Chore chore = new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: true, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.topPriority);
			chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRocketControlStation);
			return chore;
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

	[Serialize]
	public float TimeRemaining;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
