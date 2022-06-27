using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public class RancherChoreStates : GameStateMachine<RancherChoreStates, RancherChoreStates.Instance>
	{
		private class RanchState : State
		{
			public State working;

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
			ranchcreature.Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).DefaultState(ranchcreature.working).EventTransition(GameHashes.CreatureAbandonedRanchStation, checkformoreranchables)
				.Enter(SetCreatureLayer)
				.Exit(ClearCreatureLayer);
			ranchcreature.working.Enter("TellCreatureRancherIsReady", delegate(Instance smi)
			{
				smi.TellCreatureRancherIsReady();
			}).ToggleWork<RancherWorkable>(masterTarget, ranchcreature.pst, checkformoreranchables, null);
			ranchcreature.pst.ToggleAnims(GetRancherInteractAnim).QueueAnim("wipe_brow").OnAnimQueueComplete(checkformoreranchables);
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

		public static void RanchCreature(Instance smi)
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
	}

	public class RancherWorkable : Workable
	{
		protected override void OnPrefabInit()
		{
			RanchStation.Instance sMI = base.gameObject.GetSMI<RanchStation.Instance>();
			overrideAnims = new KAnimFile[1] { Assets.GetAnim(sMI.def.rancherInteractAnim) };
			SetWorkTime(sMI.def.worktime);
			SetWorkerStatusItem(sMI.def.ranchingStatusItem);
			attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
			skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			lightEfficiencyBonus = false;
		}

		public override Klei.AI.Attribute GetWorkAttribute()
		{
			return Db.Get().Attributes.Ranching;
		}

		protected override void OnStartWork(Worker worker)
		{
			RanchStation.Instance sMI = base.gameObject.GetSMI<RanchStation.Instance>();
			if (sMI != null)
			{
				sMI.targetRanchable.Get<KBatchedAnimController>().Play(sMI.def.ranchedPreAnim);
				sMI.targetRanchable.Get<KBatchedAnimController>().Queue(sMI.def.ranchedLoopAnim, KAnim.PlayMode.Loop);
			}
		}

		public override void OnPendingCompleteWork(Worker work)
		{
			RanchStation.Instance sMI = base.gameObject.GetSMI<RanchStation.Instance>();
			if (sMI != null)
			{
				sMI.targetRanchable.Get<KBatchedAnimController>().Play(sMI.def.ranchedPstAnim);
				RancherChoreStates.Instance sMI2 = base.gameObject.GetSMI<RancherChoreStates.Instance>();
				if (sMI2 != null)
				{
					RancherChoreStates.RanchCreature(sMI2);
				}
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
