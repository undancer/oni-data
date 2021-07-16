using System;
using STRINGS;

public class FixedCaptureChore : Chore<FixedCaptureChore.FixedCaptureChoreStates.Instance>
{
	public class FixedCaptureChoreStates : GameStateMachine<FixedCaptureChoreStates, FixedCaptureChoreStates.Instance>
	{
		public new class Instance : GameInstance
		{
			public FixedCapturePoint.Instance fixedCapturePoint;

			public Instance(KPrefabID capture_point)
				: base((IStateMachineTarget)capture_point)
			{
				fixedCapturePoint = capture_point.GetSMI<FixedCapturePoint.Instance>();
			}
		}

		public TargetParameter rancher;

		public TargetParameter creature;

		private State movetopoint;

		private State waitforcreature_pre;

		private State waitforcreature;

		private State capturecreature;

		private State failed;

		private State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = movetopoint;
			Target(rancher);
			root.Exit("ResetCapturePoint", delegate(Instance smi)
			{
				smi.fixedCapturePoint.ResetCapturePoint();
			});
			movetopoint.MoveTo((Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), waitforcreature_pre).Target(masterTarget).EventTransition(GameHashes.CreatureAbandonedCapturePoint, failed);
			waitforcreature_pre.EnterTransition(null, (Instance smi) => smi.fixedCapturePoint.IsNullOrStopped()).EnterTransition(failed, HasCreatureLeft).EnterTransition(waitforcreature, (Instance smi) => true);
			waitforcreature.ToggleAnims("anim_interacts_rancherstation_kanim").PlayAnim("calling_loop", KAnim.PlayMode.Loop).Transition(failed, HasCreatureLeft)
				.Face(creature)
				.Enter("SetRancherIsAvailableForCapturing", delegate(Instance smi)
				{
					smi.fixedCapturePoint.SetRancherIsAvailableForCapturing();
				})
				.Exit("ClearRancherIsAvailableForCapturing", delegate(Instance smi)
				{
					smi.fixedCapturePoint.ClearRancherIsAvailableForCapturing();
				})
				.Target(masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtCapturePoint, capturecreature);
			capturecreature.EventTransition(GameHashes.CreatureAbandonedCapturePoint, failed).EnterTransition(failed, (Instance smi) => smi.fixedCapturePoint.targetCapturable.IsNullOrStopped()).ToggleWork<Capturable>(creature, success, failed, null);
			failed.GoTo(null);
			success.ReturnSuccess();
		}

		private static bool HasCreatureLeft(Instance smi)
		{
			if (!smi.fixedCapturePoint.targetCapturable.IsNullOrStopped())
			{
				return !smi.fixedCapturePoint.targetCapturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>();
			}
			return true;
		}
	}

	public Precondition IsCreatureAvailableForFixedCapture = new Precondition
	{
		id = "IsCreatureAvailableForFixedCapture",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_FIXED_CAPTURE,
		fn = delegate(ref Precondition.Context context, object data)
		{
			return (data as FixedCapturePoint.Instance).IsCreatureAvailableForFixedCapture();
		}
	};

	public FixedCaptureChore(KPrefabID capture_point)
		: base(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget)capture_point, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		AddPrecondition(IsCreatureAvailableForFixedCapture, capture_point.GetSMI<FixedCapturePoint.Instance>());
		AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanWrangleCreatures.Id);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, capture_point.GetComponent<Building>());
		Operational component = capture_point.GetComponent<Operational>();
		AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		Deconstructable component2 = capture_point.GetComponent<Deconstructable>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
		BuildingEnabledButton component3 = capture_point.GetComponent<BuildingEnabledButton>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
		base.smi = new FixedCaptureChoreStates.Instance(capture_point);
		SetPrioritizable(capture_point.GetComponent<Prioritizable>());
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.rancher.Set(context.consumerState.gameObject, base.smi);
		base.smi.sm.creature.Set(base.smi.fixedCapturePoint.targetCapturable.gameObject, base.smi);
		base.Begin(context);
	}
}
