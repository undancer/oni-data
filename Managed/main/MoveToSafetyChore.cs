using System;
using UnityEngine;

public class MoveToSafetyChore : Chore<MoveToSafetyChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MoveToSafetyChore, object>.GameInstance
	{
		private SafeCellSensor sensor;

		public int targetCell;

		public StatesInstance(MoveToSafetyChore master, GameObject mover)
			: base(master)
		{
			base.sm.mover.Set(mover, base.smi);
			sensor = base.sm.mover.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>();
			targetCell = sensor.GetSensorCell();
		}

		public void UpdateTargetCell()
		{
			targetCell = sensor.GetSensorCell();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MoveToSafetyChore>
	{
		public TargetParameter mover;

		public State move;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = move;
			Target(mover);
			root.ToggleTag(GameTags.Idle);
			move.Enter("UpdateLocatorPosition", delegate(StatesInstance smi)
			{
				smi.UpdateTargetCell();
			}).Update("UpdateLocatorPosition", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateTargetCell();
			}).MoveTo((StatesInstance smi) => smi.targetCell, null, null, update_cell: true);
		}
	}

	public MoveToSafetyChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.MoveToSafety, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.idle, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
