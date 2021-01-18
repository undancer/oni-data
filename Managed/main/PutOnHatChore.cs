using System;
using UnityEngine;

public class PutOnHatChore : Chore<PutOnHatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PutOnHatChore, object>.GameInstance
	{
		public StatesInstance(PutOnHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PutOnHatChore>
	{
		public TargetParameter duplicant;

		public State applyHat_pre;

		public State applyHat;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = applyHat_pre;
			Target(duplicant);
			applyHat_pre.ToggleAnims("anim_hat_kanim").Enter(delegate(StatesInstance smi)
			{
				duplicant.Get(smi).GetComponent<MinionResume>().ApplyTargetHat();
			}).PlayAnim("hat_first")
				.OnAnimQueueComplete(applyHat);
			applyHat.ToggleAnims("anim_hat_kanim").PlayAnim("working_pst").OnAnimQueueComplete(complete);
			complete.ReturnSuccess();
		}
	}

	public PutOnHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
