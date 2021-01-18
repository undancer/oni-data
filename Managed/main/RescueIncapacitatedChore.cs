using System;
using STRINGS;
using UnityEngine;

public class RescueIncapacitatedChore : Chore<RescueIncapacitatedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RescueIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(RescueIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RescueIncapacitatedChore>
	{
		public class HoldingIncapacitated : State
		{
			public State pickup;

			public ApproachSubState<IApproachable> delivering;

			public State deposit;

			public State ditch;
		}

		public ApproachSubState<Chattable> approachIncapacitated;

		public State failure;

		public HoldingIncapacitated holding;

		public TargetParameter rescueTarget;

		public TargetParameter deliverTarget;

		public TargetParameter rescuer;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approachIncapacitated;
			approachIncapacitated.InitializeStates(rescuer, rescueTarget, holding.pickup, failure, Grid.DefaultOffset).Enter(delegate(StatesInstance smi)
			{
				if (rescueTarget.GetSMI<DeathMonitor.Instance>(smi)?.IsDead() ?? true)
				{
					smi.StopSM("target died");
				}
			});
			holding.Target(rescuer).Enter(delegate(StatesInstance smi)
			{
				smi.sm.rescueTarget.Get(smi).Subscribe(1623392196, delegate
				{
					smi.GoTo(holding.ditch);
				});
				KAnimFile anim2 = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim2);
				smi.master.GetComponent<KAnimControllerBase>().AddAnimOverrides(anim2);
			}).Exit(delegate(StatesInstance smi)
			{
				KAnimFile anim = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
			});
			holding.pickup.Target(rescuer).PlayAnim("pickup").Enter(delegate(StatesInstance smi)
			{
				rescueTarget.Get(smi).gameObject.GetComponent<KBatchedAnimController>().Play("pickup");
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
			holding.delivering.InitializeStates(rescuer, deliverTarget, holding.deposit, holding.ditch).Enter(delegate(StatesInstance smi)
			{
				if (rescueTarget.GetSMI<DeathMonitor.Instance>(smi)?.IsDead() ?? true)
				{
					smi.StopSM("target died");
				}
			}).Update(delegate(StatesInstance smi, float dt)
			{
				if (deliverTarget.Get(smi) == null)
				{
					smi.GoTo(holding.ditch);
				}
			});
			holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
				smi.SetStatus(Status.Success);
				smi.StopSM("complete");
			});
			holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, failure).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
			});
			failure.Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("failed");
			});
		}
	}

	public static Precondition CanReachIncapacitated = new Precondition
	{
		id = "CanReachIncapacitated",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = delegate(ref Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject == null)
			{
				return false;
			}
			int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(gameObject.transform.GetPosition()));
			if (-1 != navigationCost)
			{
				context.cost += navigationCost;
				return true;
			}
			return false;
		}
	};

	public RescueIncapacitatedChore(IStateMachineTarget master, GameObject incapacitatedDuplicant)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		base.runUntilComplete = true;
		AddPrecondition(ChorePreconditions.instance.NotChoreCreator, incapacitatedDuplicant.gameObject);
		AddPrecondition(CanReachIncapacitated, incapacitatedDuplicant);
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.rescuer.Set(context.consumerState.gameObject, base.smi);
		base.smi.sm.rescueTarget.Set(gameObject, base.smi);
		base.smi.sm.deliverTarget.Set(gameObject.GetSMI<BeIncapacitatedChore.StatesInstance>().master.GetChosenClinic(), base.smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		DropIncapacitatedDuplicant();
		base.End(reason);
	}

	private void DropIncapacitatedDuplicant()
	{
		if (base.smi.sm.rescuer.Get(base.smi) != null && base.smi.sm.rescueTarget.Get(base.smi) != null)
		{
			base.smi.sm.rescuer.Get(base.smi).GetComponent<Storage>().Drop(base.smi.sm.rescueTarget.Get(base.smi));
		}
	}
}
