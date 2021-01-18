using System;
using UnityEngine;

public class EntombedChore : Chore<EntombedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EntombedChore, object>.GameInstance
	{
		public StatesInstance(EntombedChore master, GameObject entombable)
			: base(master)
		{
			base.sm.entombable.Set(entombable, base.smi);
		}

		public void UpdateFaceEntombed()
		{
			int num = Grid.CellAbove(Grid.PosToCell(base.transform.GetPosition()));
			base.sm.isFaceEntombed.Set(Grid.IsValidCell(num) && Grid.Solid[num], base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EntombedChore>
	{
		public BoolParameter isFaceEntombed;

		public TargetParameter entombable;

		public State entombedface;

		public State entombedbody;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = entombedbody;
			Target(entombable);
			root.ToggleAnims("anim_emotes_default_kanim").Update("IsFaceEntombed", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateFaceEntombed();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.EntombedChore);
			entombedface.PlayAnim("entombed_ceiling", KAnim.PlayMode.Loop).ParamTransition(isFaceEntombed, entombedbody, GameStateMachine<States, StatesInstance, EntombedChore, object>.IsFalse);
			entombedbody.PlayAnim("entombed_floor", KAnim.PlayMode.Loop).StopMoving().ParamTransition(isFaceEntombed, entombedface, GameStateMachine<States, StatesInstance, EntombedChore, object>.IsTrue);
		}
	}

	public EntombedChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Entombed, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
