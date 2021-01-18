using System;
using STRINGS;
using UnityEngine;

public class RescueSweepBotChore : Chore<RescueSweepBotChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RescueSweepBotChore, object>.GameInstance
	{
		public StatesInstance(RescueSweepBotChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RescueSweepBotChore>
	{
		public class HoldingSweepBot : State
		{
			public State pickup;

			public ApproachSubState<IApproachable> delivering;

			public State deposit;

			public State ditch;
		}

		public ApproachSubState<Storage> approachSweepBot;

		public State failure;

		public HoldingSweepBot holding;

		public TargetParameter rescueTarget;

		public TargetParameter deliverTarget;

		public TargetParameter rescuer;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approachSweepBot;
			approachSweepBot.InitializeStates(rescuer, rescueTarget, holding.pickup, failure, Grid.DefaultOffset);
			holding.Target(rescuer).Enter(delegate(StatesInstance smi)
			{
				KAnimFile anim2 = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim2);
				rescuer.Get(smi).GetComponent<KAnimControllerBase>().AddAnimOverrides(anim2);
			}).Exit(delegate(StatesInstance smi)
			{
				KAnimFile anim = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
			});
			holding.pickup.Target(rescuer).PlayAnim("pickup").Enter(delegate
			{
			})
				.Exit(delegate(StatesInstance smi)
				{
					rescuer.Get(smi).GetComponent<Storage>().Store(rescueTarget.Get(smi));
					rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.symbol = new HashedString("snapTo_pivot");
					component.offset = new Vector3(0f, 0f, 1f);
				})
				.EventTransition(GameHashes.AnimQueueComplete, holding.delivering);
			holding.delivering.InitializeStates(rescuer, deliverTarget, holding.deposit, holding.ditch).Update(delegate(StatesInstance smi, float dt)
			{
				if (deliverTarget.Get(smi) == null)
				{
					smi.GoTo(holding.ditch);
				}
			});
			holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				smi.master.DropSweepBot();
				smi.SetStatus(Status.Success);
				smi.StopSM("complete");
			});
			holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, failure).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropSweepBot();
			});
			failure.Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("failed");
			});
		}
	}

	public Precondition CanReachBaseStation = new Precondition
	{
		id = "CanReachBaseStation",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Precondition.Context context, object data)
		{
			if (context.consumerState.consumer == null)
			{
				return false;
			}
			KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)data;
			return !(kMonoBehaviour == null) && context.consumerState.consumer.navigator.CanReach(Grid.PosToCell(kMonoBehaviour));
		}
	};

	public static Precondition CanReachIncapacitated = new Precondition
	{
		id = "CanReachIncapacitated",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Precondition.Context context, object data)
		{
			KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)data;
			if (kMonoBehaviour == null)
			{
				return false;
			}
			int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(kMonoBehaviour.transform.GetPosition()));
			if (-1 != navigationCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		}
	};

	public RescueSweepBotChore(IStateMachineTarget master, GameObject sweepBot, GameObject baseStation)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		base.runUntilComplete = true;
		AddPrecondition(CanReachIncapacitated, sweepBot.GetComponent<Storage>());
		AddPrecondition(CanReachBaseStation, baseStation.GetComponent<Storage>());
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.rescuer.Set(context.consumerState.gameObject, base.smi);
		base.smi.sm.rescueTarget.Set(gameObject, base.smi);
		base.smi.sm.deliverTarget.Set(gameObject.GetSMI<SweepBotTrappedStates.Instance>().sm.GetSweepLocker(gameObject.GetSMI<SweepBotTrappedStates.Instance>()).gameObject, base.smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		DropSweepBot();
		base.End(reason);
	}

	private void DropSweepBot()
	{
		if (base.smi.sm.rescuer.Get(base.smi) != null && base.smi.sm.rescueTarget.Get(base.smi) != null)
		{
			base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>().Drop(base.smi.sm.rescueTarget.Get(base.smi));
		}
	}
}
