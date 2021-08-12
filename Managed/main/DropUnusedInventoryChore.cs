using System;

public class DropUnusedInventoryChore : Chore<DropUnusedInventoryChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, DropUnusedInventoryChore, object>.GameInstance
	{
		public StatesInstance(DropUnusedInventoryChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, DropUnusedInventoryChore>
	{
		public State dropping;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = dropping;
			dropping.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Storage>().DropAll();
			}).GoTo(success);
			success.ReturnSuccess();
		}
	}

	public DropUnusedInventoryChore(ChoreType chore_type, IStateMachineTarget target)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: true, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
	}
}
