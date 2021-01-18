using System;

public class DeliverFoodChore : Chore<DeliverFoodChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, DeliverFoodChore, object>.GameInstance
	{
		public StatesInstance(DeliverFoodChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, DeliverFoodChore>
	{
		public TargetParameter deliverer;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public TargetParameter deliverypoint;

		public FloatParameter requestedrationcount;

		public FloatParameter actualrationcount;

		public FetchSubState fetch;

		public ApproachSubState<Chattable> movetodeliverypoint;

		public DropSubState drop;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			fetch.InitializeStates(deliverer, ediblesource, ediblechunk, requestedrationcount, actualrationcount, movetodeliverypoint);
			movetodeliverypoint.InitializeStates(deliverer, deliverypoint, drop);
			drop.InitializeStates(deliverer, ediblechunk, deliverypoint, success);
			success.ReturnSuccess();
		}
	}

	public DeliverFoodChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.DeliverFood, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.IsChattable, target);
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.requestedrationcount.Set(base.smi.GetComponent<StateMachineController>().GetSMI<RationMonitor.Instance>().GetRationsRemaining(), base.smi);
		base.smi.sm.ediblesource.Set(context.consumerState.gameObject.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible(), base.smi);
		base.smi.sm.deliverypoint.Set(gameObject, base.smi);
		base.smi.sm.deliverer.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
