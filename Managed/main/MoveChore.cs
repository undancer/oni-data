using System;
using UnityEngine;

public class MoveChore : Chore<MoveChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MoveChore, object>.GameInstance
	{
		public Func<StatesInstance, int> getCellCallback;

		public StatesInstance(MoveChore master, GameObject mover, Func<StatesInstance, int> get_cell_callback, bool update_cell = false)
			: base(master)
		{
			getCellCallback = get_cell_callback;
			base.sm.mover.Set(mover, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MoveChore>
	{
		public ApproachSubState<IApproachable> approach;

		public TargetParameter mover;

		public TargetParameter locator;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(mover);
			root.MoveTo((StatesInstance smi) => smi.getCellCallback(smi));
		}
	}

	public MoveChore(IStateMachineTarget target, ChoreType chore_type, Func<StatesInstance, int> get_cell_callback, bool update_cell = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, get_cell_callback, update_cell);
	}
}
