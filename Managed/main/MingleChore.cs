using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MingleChore : Chore<MingleChore.StatesInstance>, IWorkerPrioritizable
{
	public class States : GameStateMachine<States, StatesInstance, MingleChore>
	{
		public TargetParameter mingler;

		public State mingle;

		public State move;

		public State walk;

		public State onfloor;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = mingle;
			Target(mingler);
			root.EventTransition(GameHashes.ScheduleBlocksChanged, null, (StatesInstance smi) => !smi.IsRecTime());
			mingle.Transition(walk, (StatesInstance smi) => smi.IsSameRoom()).Transition(move, (StatesInstance smi) => !smi.IsSameRoom());
			move.Transition(null, (StatesInstance smi) => !smi.HasMingleCell()).MoveTo((StatesInstance smi) => smi.GetMingleCell(), onfloor);
			walk.Transition(null, (StatesInstance smi) => !smi.HasMingleCell()).TriggerOnEnter(GameHashes.BeginWalk).TriggerOnExit(GameHashes.EndWalk)
				.ToggleAnims("anim_loco_walk_kanim")
				.MoveTo((StatesInstance smi) => smi.GetMingleCell(), onfloor);
			onfloor.ToggleAnims("anim_generic_convo_kanim").PlayAnim("idle", KAnim.PlayMode.Loop).ScheduleGoTo((StatesInstance smi) => UnityEngine.Random.Range(5, 10), success)
				.ToggleTag(GameTags.AlwaysConverse);
			success.ReturnSuccess();
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, MingleChore, object>.GameInstance
	{
		private MingleCellSensor mingleCellSensor;

		private GameObject mingler;

		public StatesInstance(MingleChore master, GameObject mingler)
			: base(master)
		{
			this.mingler = mingler;
			base.sm.mingler.Set(mingler, base.smi);
			mingleCellSensor = GetComponent<Sensors>().GetSensor<MingleCellSensor>();
		}

		public bool IsRecTime()
		{
			Schedulable component = base.master.GetComponent<Schedulable>();
			return component.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public int GetMingleCell()
		{
			return mingleCellSensor.GetCell();
		}

		public bool HasMingleCell()
		{
			return mingleCellSensor.GetCell() != Grid.InvalidCell;
		}

		public bool IsSameRoom()
		{
			int cell = Grid.PosToCell(mingler);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(GetMingleCell());
			if (cavityForCell != null && cavityForCell2 != null)
			{
				return cavityForCell.handle == cavityForCell2.handle;
			}
			return false;
		}
	}

	private int basePriority = RELAXATION.PRIORITY.TIER1;

	private Precondition HasMingleCell = new Precondition
	{
		id = "HasMingleCell",
		description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_MINGLE_CELL,
		fn = delegate(ref Precondition.Context context, object data)
		{
			MingleChore mingleChore = (MingleChore)data;
			return mingleChore.smi.HasMingleCell();
		}
	};

	public MingleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Relax, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.PersonalTime)
	{
		showAvailabilityInHoverText = false;
		base.smi = new StatesInstance(this, target.gameObject);
		AddPrecondition(HasMingleCell, this);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	protected override StatusItem GetStatusItem()
	{
		return Db.Get().DuplicantStatusItems.Mingling;
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		return true;
	}
}
