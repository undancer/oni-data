using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BalloonArtistChore : Chore<BalloonArtistChore.StatesInstance>, IWorkerPrioritizable
{
	public class States : GameStateMachine<States, StatesInstance, BalloonArtistChore>
	{
		public class BalloonStandStates : State
		{
			public State idle;

			public State giveBalloon;
		}

		public TargetParameter balloonArtist;

		public IntParameter balloonsGivenOut = new IntParameter(0);

		public Signal giveBalloonOut;

		public State idle;

		public State goToStand;

		public BalloonStandStates balloonStand;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = goToStand;
			Target(balloonArtist);
			root.EventTransition(GameHashes.ScheduleBlocksChanged, idle, (StatesInstance smi) => !smi.IsRecTime());
			idle.DoNothing();
			goToStand.Transition(null, (StatesInstance smi) => !smi.HasBalloonStallCell()).MoveTo((StatesInstance smi) => smi.GetBalloonStallCell(), balloonStand);
			balloonStand.ToggleAnims("anim_interacts_balloon_artist_kanim").Enter(delegate(StatesInstance smi)
			{
				smi.SpawnBalloonStand();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.DestroyBalloonStand();
			})
				.DefaultState(balloonStand.idle);
			balloonStand.idle.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).OnSignal(giveBalloonOut, balloonStand.giveBalloon);
			balloonStand.giveBalloon.PlayAnim("working_pst").OnAnimQueueComplete(balloonStand.idle);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, BalloonArtistChore, object>.GameInstance
	{
		private BalloonStandCellSensor balloonArtistCellSensor;

		private GameObject balloonArtist;

		private GameObject balloonStand;

		public StatesInstance(BalloonArtistChore master, GameObject balloonArtist)
			: base(master)
		{
			this.balloonArtist = balloonArtist;
			base.sm.balloonArtist.Set(balloonArtist, base.smi);
		}

		public bool IsRecTime()
		{
			return base.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public int GetBalloonStallCell()
		{
			return balloonArtistCellSensor.GetCell();
		}

		public int GetBalloonStallTargetCell()
		{
			return balloonArtistCellSensor.GetStandCell();
		}

		public bool HasBalloonStallCell()
		{
			if (balloonArtistCellSensor == null)
			{
				balloonArtistCellSensor = GetComponent<Sensors>().GetSensor<BalloonStandCellSensor>();
			}
			return balloonArtistCellSensor.GetCell() != Grid.InvalidCell;
		}

		public bool IsSameRoom()
		{
			int cell = Grid.PosToCell(balloonArtist);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(GetBalloonStallCell());
			if (cavityForCell != null && cavityForCell2 != null)
			{
				return cavityForCell.handle == cavityForCell2.handle;
			}
			return false;
		}

		public void SpawnBalloonStand()
		{
			Vector3 vector = Grid.CellToPos(GetBalloonStallTargetCell());
			balloonArtist.GetComponent<Facing>().Face(vector);
			balloonStand = Util.KInstantiate(Assets.GetPrefab("BalloonStand"), vector, Quaternion.identity);
			balloonStand.SetActive(value: true);
			balloonStand.GetComponent<GetBalloonWorkable>().SetBalloonArtist(base.smi);
		}

		public void DestroyBalloonStand()
		{
			balloonStand.DeleteObject();
		}

		public void GiveBalloon()
		{
			BalloonArtist.Instance sMI = balloonArtist.GetSMI<BalloonArtist.Instance>();
			sMI.GiveBalloon();
			base.smi.sm.giveBalloonOut.Trigger(base.smi);
		}
	}

	private int basePriority = RELAXATION.PRIORITY.TIER1;

	private Precondition HasBalloonStallCell = new Precondition
	{
		id = "HasBalloonStallCell",
		description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_BALLOON_STALL_CELL,
		fn = delegate(ref Precondition.Context context, object data)
		{
			BalloonArtistChore balloonArtistChore = (BalloonArtistChore)data;
			return balloonArtistChore.smi.HasBalloonStallCell();
		}
	};

	public BalloonArtistChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.JoyReaction, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.PersonalTime)
	{
		showAvailabilityInHoverText = false;
		base.smi = new StatesInstance(this, target.gameObject);
		AddPrecondition(HasBalloonStallCell, this);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		return true;
	}
}
