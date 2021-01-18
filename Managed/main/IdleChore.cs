using System;
using UnityEngine;

public class IdleChore : Chore<IdleChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IdleChore, object>.GameInstance
	{
		private IdleCellSensor idleCellSensor;

		public StatesInstance(IdleChore master, GameObject idler)
			: base(master)
		{
			base.sm.idler.Set(idler, base.smi);
			idleCellSensor = GetComponent<Sensors>().GetSensor<IdleCellSensor>();
		}

		public void UpdateNavType()
		{
			NavType currentNavType = GetComponent<Navigator>().CurrentNavType;
			base.sm.isOnLadder.Set(currentNavType == NavType.Ladder || currentNavType == NavType.Pole, this);
			base.sm.isOnTube.Set(currentNavType == NavType.Tube, this);
		}

		public int GetIdleCell()
		{
			return idleCellSensor.GetCell();
		}

		public bool HasIdleCell()
		{
			return idleCellSensor.GetCell() != Grid.InvalidCell;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, IdleChore>
	{
		public class IdleState : State
		{
			public State onfloor;

			public State onladder;

			public State ontube;

			public State move;
		}

		public BoolParameter isOnLadder;

		public BoolParameter isOnTube;

		public TargetParameter idler;

		public IdleState idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			Target(idler);
			idle.DefaultState(idle.onfloor).Enter("UpdateNavType", delegate(StatesInstance smi)
			{
				smi.UpdateNavType();
			}).Update("UpdateNavType", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateNavType();
			})
				.ToggleStateMachine((StatesInstance smi) => new TaskAvailabilityMonitor.Instance(smi.master))
				.ToggleTag(GameTags.Idle);
			idle.onfloor.PlayAnim("idle_default", KAnim.PlayMode.Loop).ParamTransition(isOnLadder, idle.onladder, GameStateMachine<States, StatesInstance, IdleChore, object>.IsTrue).ParamTransition(isOnTube, idle.ontube, GameStateMachine<States, StatesInstance, IdleChore, object>.IsTrue)
				.ToggleScheduleCallback("IdleMove", (StatesInstance smi) => UnityEngine.Random.Range(5, 15), delegate(StatesInstance smi)
				{
					smi.GoTo(idle.move);
				});
			idle.onladder.PlayAnim("ladder_idle", KAnim.PlayMode.Loop).ToggleScheduleCallback("IdleMove", (StatesInstance smi) => UnityEngine.Random.Range(5, 15), delegate(StatesInstance smi)
			{
				smi.GoTo(idle.move);
			});
			idle.ontube.PlayAnim("tube_idle_loop", KAnim.PlayMode.Loop).Update("IdleMove", delegate(StatesInstance smi, float dt)
			{
				if (smi.HasIdleCell())
				{
					smi.GoTo(idle.move);
				}
			}, UpdateRate.SIM_1000ms);
			idle.move.Transition(idle, (StatesInstance smi) => !smi.HasIdleCell()).TriggerOnEnter(GameHashes.BeginWalk).TriggerOnExit(GameHashes.EndWalk)
				.ToggleAnims("anim_loco_walk_kanim")
				.MoveTo((StatesInstance smi) => smi.GetIdleCell(), idle, idle)
				.Exit("UpdateNavType", delegate(StatesInstance smi)
				{
					smi.UpdateNavType();
				})
				.Exit("ClearWalk", delegate(StatesInstance smi)
				{
					smi.GetComponent<KBatchedAnimController>().Play("idle_default");
				});
		}
	}

	public IdleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Idle, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.idle, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.IdleTime)
	{
		showAvailabilityInHoverText = false;
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
