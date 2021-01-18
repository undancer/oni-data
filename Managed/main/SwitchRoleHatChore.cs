using System;
using UnityEngine;

public class SwitchRoleHatChore : Chore<SwitchRoleHatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SwitchRoleHatChore, object>.GameInstance
	{
		public StatesInstance(SwitchRoleHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SwitchRoleHatChore>
	{
		public TargetParameter duplicant;

		public State remove_hat;

		public State start;

		public State delay;

		public State delay_pst;

		public State applyHat_pre;

		public State applyHat;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = start;
			Target(duplicant);
			start.Enter(delegate(StatesInstance smi)
			{
				if (duplicant.Get(smi).GetComponent<MinionResume>().CurrentHat == null)
				{
					smi.GoTo(delay);
				}
				else
				{
					smi.GoTo(remove_hat);
				}
			});
			remove_hat.ToggleAnims("anim_hat_kanim").PlayAnim("hat_off").OnAnimQueueComplete(delay);
			delay.ToggleThought(Db.Get().Thoughts.NewRole).ToggleExpression(Db.Get().Expressions.Happy).ToggleAnims("anim_selfish_kanim")
				.QueueAnim("working_pre")
				.QueueAnim("working_loop")
				.QueueAnim("working_pst")
				.OnAnimQueueComplete(applyHat_pre);
			applyHat_pre.ToggleAnims("anim_hat_kanim").Enter(delegate(StatesInstance smi)
			{
				duplicant.Get(smi).GetComponent<MinionResume>().ApplyTargetHat();
			}).PlayAnim("hat_first")
				.OnAnimQueueComplete(applyHat);
			applyHat.ToggleAnims("anim_hat_kanim").PlayAnim("working_pst").OnAnimQueueComplete(complete);
			complete.ReturnSuccess();
		}
	}

	public SwitchRoleHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
