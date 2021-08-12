using System;
using UnityEngine;

public class StressIdleChore : Chore<StressIdleChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, StressIdleChore, object>.GameInstance
	{
		public StatesInstance(StressIdleChore master, GameObject idler)
			: base(master)
		{
			base.sm.idler.Set(idler, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, StressIdleChore>
	{
		public TargetParameter idler;

		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			Target(idler);
			idle.PlayAnim("idle_default", KAnim.PlayMode.Loop);
		}
	}

	public StressIdleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.StressIdle, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
