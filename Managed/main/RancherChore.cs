using System;
using STRINGS;
using UnityEngine;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public class RancherChoreStates : GameStateMachine<RancherChoreStates, RancherChoreStates.Instance>
	{
		private class RanchState : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public new class Instance : GameInstance
		{
			public RanchStation.Instance ranchStation;

			public Instance(KPrefabID rancher_station)
				: base((IStateMachineTarget)rancher_station)
			{
				ranchStation = rancher_station.GetSMI<RanchStation.Instance>();
			}

			public void CheckForMoreRanchables()
			{
				ranchStation.FindRanchable();
				if (ranchStation.IsCreatureAvailableForRanching())
				{
					GoTo(base.sm.movetoranch);
				}
				else
				{
					GoTo((BaseState)null);
				}
			}

			public void TriggerRanchStationNoLongerAvailable()
			{
				ranchStation.TriggerRanchStationNoLongerAvailable();
			}

			public void TellCreatureRancherIsReady()
			{
				if (!ranchStation.targetRanchable.IsNullOrStopped())
				{
					ranchStation.targetRanchable.Trigger(1084749845);
				}
			}
		}

		public TargetParameter rancher;

		private State movetoranch;

		private State waitforcreature_pre;

		private State waitforcreature;

		private RanchState ranchcreature;

		private State wavegoodbye;

		private State checkformoreranchables;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = movetoranch;
			Target(rancher);
			root.Exit("TriggerRanchStationNoLongerAvailable", delegate(Instance smi)
			{
				smi.TriggerRanchStationNoLongerAvailable();
			});
			movetoranch.MoveTo((Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), waitforcreature_pre).Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms);
			waitforcreature_pre.EnterTransition(null, (Instance smi) => smi.ranchStation.IsNullOrStopped()).Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).EnterTransition(waitforcreature, (Instance smi) => true);
			waitforcreature.Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).ToggleAnims("anim_interacts_rancherstation_kanim").PlayAnim("calling_loop", KAnim.PlayMode.Loop)
				.Enter(FaceCreature)
				.Enter("TellCreatureToGoGetRanched", delegate(Instance smi)
				{
					smi.ranchStation.SetRancherIsAvailableForRanching();
				})
				.Exit("ClearRancherIsAvailableForRanching", delegate(Instance smi)
				{
					smi.ranchStation.ClearRancherIsAvailableForRanching();
				})
				.Target(masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtRanchStation, ranchcreature);
			ranchcreature.Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).ToggleAnims(GetRancherInteractAnim).DefaultState(ranchcreature.pre)
				.EventTransition(GameHashes.CreatureAbandonedRanchStation, checkformoreranchables)
				.Enter(SetCreatureLayer)
				.Exit(ClearCreatureLayer);
			ranchcreature.pre.Enter(FaceCreature).Enter(PlayBuildingWorkingPre).QueueAnim("working_pre")
				.OnAnimQueueComplete(ranchcreature.loop);
			ranchcreature.loop.Enter("TellCreatureRancherIsReady", delegate(Instance smi)
			{
				smi.TellCreatureRancherIsReady();
			}).Enter(PlayBuildingWorkingLoop).Enter(PlayRancherWorkingLoops)
				.Target(rancher)
				.OnAnimQueueComplete(ranchcreature.pst);
			ranchcreature.pst.Enter(RanchCreature).Enter(PlayBuildingWorkingPst).QueueAnim("working_pst")
				.QueueAnim("wipe_brow")
				.OnAnimQueueComplete(checkformoreranchables);
			checkformoreranchables.Enter("FindRanchable", delegate(Instance smi)
			{
				smi.CheckForMoreRanchables();
			}).Update("FindRanchable", delegate(Instance smi, float dt)
			{
				smi.CheckForMoreRanchables();
			});
		}

		private static bool HasCreatureLeft(Instance smi)
		{
			if (!smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				return !smi.ranchStation.targetRanchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>();
			}
			return true;
		}

		private static void SetCreatureLayer(Instance smi)
		{
			if (!smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
			}
		}

		private static void ClearCreatureLayer(Instance smi)
		{
			if (!smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			}
		}

		private static HashedString GetRancherInteractAnim(Instance smi)
		{
			return smi.ranchStation.def.rancherInteractAnim;
		}

		private static void FaceCreature(Instance smi)
		{
			Facing facing = smi.sm.rancher.Get<Facing>(smi);
			Vector3 position = smi.ranchStation.targetRanchable.transform.GetPosition();
			facing.Face(position);
		}

		private static void RanchCreature(Instance smi)
		{
			Debug.Assert(smi.ranchStation != null, "smi.ranchStation was null");
			RanchableMonitor.Instance targetRanchable = smi.ranchStation.targetRanchable;
			if (!targetRanchable.IsNullOrStopped())
			{
				KPrefabID component = targetRanchable.GetComponent<KPrefabID>();
				smi.sm.rancher.Get(smi).Trigger(937885943, component.PrefabTag.Name);
				smi.ranchStation.RanchCreature();
			}
		}

		private static bool ShouldSynchronizeBuilding(Instance smi)
		{
			return smi.ranchStation.def.synchronizeBuilding;
		}

		private static void PlayBuildingWorkingPre(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_pre");
			}
		}

		private static void PlayRancherWorkingLoops(Instance smi)
		{
			KBatchedAnimController kBatchedAnimController = smi.sm.rancher.Get<KBatchedAnimController>(smi);
			for (int i = 0; i < smi.ranchStation.def.interactLoopCount; i++)
			{
				kBatchedAnimController.Queue("working_loop");
			}
		}

		private static void PlayBuildingWorkingLoop(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_loop", KAnim.PlayMode.Loop);
			}
		}

		private static void PlayBuildingWorkingPst(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_pst");
			}
		}
	}

	public Precondition IsCreatureAvailableForRanching = new Precondition
	{
		id = "IsCreatureAvailableForRanching",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING,
		fn = delegate(ref Precondition.Context context, object data)
		{
			return (data as RanchStation.Instance).IsCreatureAvailableForRanching();
		}
	};

	public RancherChore(KPrefabID rancher_station)
		: base(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget)rancher_station, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		AddPrecondition(IsCreatureAvailableForRanching, rancher_station.GetSMI<RanchStation.Instance>());
		AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRanchStation.Id);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, rancher_station.GetComponent<Building>());
		Operational component = rancher_station.GetComponent<Operational>();
		AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		Deconstructable component2 = rancher_station.GetComponent<Deconstructable>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
		BuildingEnabledButton component3 = rancher_station.GetComponent<BuildingEnabledButton>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
		base.smi = new RancherChoreStates.Instance(rancher_station);
		SetPrioritizable(rancher_station.GetComponent<Prioritizable>());
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.rancher.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
