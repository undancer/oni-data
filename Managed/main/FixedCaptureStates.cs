using STRINGS;

public class FixedCaptureStates : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public float originalSpeed;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			originalSpeed = GetComponent<Navigator>().defaultSpeed;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetCaptured);
		}

		public FixedCapturePoint.Instance GetCapturePoint()
		{
			return this.GetSMI<FixedCapturableMonitor.Instance>()?.targetCapturePoint;
		}

		public void AbandonedCapturePoint()
		{
			if (GetCapturePoint() != null)
			{
				GetCapturePoint().Trigger(-1000356449);
			}
		}
	}

	public class CaptureStates : State
	{
		public class CheerStates : State
		{
			public State pre;

			public State cheer;

			public State pst;
		}

		public class MoveStates : State
		{
			public State movetoranch;

			public State waitforranchertobeready;
		}

		public CheerStates cheer;

		public MoveStates move;

		public State ranching;
	}

	private CaptureStates capture;

	private State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = capture;
		root.Exit("AbandonedCapturePoint", delegate(Instance smi)
		{
			smi.AbandonedCapturePoint();
		});
		capture.EventTransition(GameHashes.CapturePointNoLongerAvailable, null).DefaultState(capture.cheer);
		capture.cheer.DefaultState(capture.cheer.pre).ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		capture.cheer.pre.ScheduleGoTo(0.9f, capture.cheer.cheer);
		capture.cheer.cheer.Enter("FaceRancher", delegate(Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetCapturePoint().transform.GetPosition());
		}).PlayAnim("excited_loop").OnAnimQueueComplete(capture.cheer.pst);
		capture.cheer.pst.ScheduleGoTo(0.2f, capture.move);
		capture.move.DefaultState(capture.move.movetoranch).ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		capture.move.movetoranch.Enter("Speedup", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f;
		}).MoveTo(GetTargetCaptureCell, capture.move.waitforranchertobeready).Exit("RestoreSpeed", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed;
		});
		capture.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", delegate(Instance smi)
		{
			smi.GetCapturePoint().Trigger(-1992722293);
		}).EventTransition(GameHashes.RancherReadyAtCapturePoint, capture.ranching);
		capture.ranching.ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetCaptured);
	}

	private static FixedCapturePoint.Instance GetCapturePoint(Instance smi)
	{
		return smi.GetSMI<FixedCapturableMonitor.Instance>().targetCapturePoint;
	}

	private static int GetTargetCaptureCell(Instance smi)
	{
		FixedCapturePoint.Instance capturePoint = GetCapturePoint(smi);
		return capturePoint.def.getTargetCapturePoint(capturePoint);
	}
}
