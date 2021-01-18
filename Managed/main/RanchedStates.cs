using STRINGS;

public class RanchedStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
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
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetRanched);
		}

		public RanchStation.Instance GetRanchStation()
		{
			return this.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
		}

		public void AbandonedRanchStation()
		{
			if (GetRanchStation() != null)
			{
				GetRanchStation().Trigger(-364750427);
			}
		}
	}

	public class RanchStates : State
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

			public State getontable;

			public State waitforranchertobeready;
		}

		public CheerStates cheer;

		public MoveStates move;

		public State ranching;
	}

	private RanchStates ranch;

	private State wavegoodbye;

	private State runaway;

	private State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = ranch;
		root.Exit("AbandonedRanchStation", delegate(Instance smi)
		{
			smi.AbandonedRanchStation();
		});
		ranch.EventTransition(GameHashes.RanchStationNoLongerAvailable, null).DefaultState(ranch.cheer).Exit(ClearLayerOverride);
		ranch.cheer.DefaultState(ranch.cheer.pre).ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		ranch.cheer.pre.ScheduleGoTo(0.9f, ranch.cheer.cheer);
		ranch.cheer.cheer.Enter("FaceRancher", delegate(Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetRanchStation().transform.GetPosition());
		}).PlayAnim("excited_loop").OnAnimQueueComplete(ranch.cheer.pst);
		ranch.cheer.pst.ScheduleGoTo(0.2f, ranch.move);
		ranch.move.DefaultState(ranch.move.movetoranch).ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_RANCHED.NAME, CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		ranch.move.movetoranch.Enter("Speedup", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f;
		}).MoveTo(GetTargetRanchCell, ranch.move.getontable).Exit("RestoreSpeed", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed;
		});
		ranch.move.getontable.Enter(GetOnTable).OnAnimQueueComplete(ranch.move.waitforranchertobeready);
		ranch.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", delegate(Instance smi)
		{
			smi.GetRanchStation().Trigger(-1357116271);
		}).EventTransition(GameHashes.RancherReadyAtRanchStation, ranch.ranching);
		ranch.ranching.Enter(PlayGroomingLoopAnim).EventTransition(GameHashes.RanchingComplete, wavegoodbye).ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_RANCHED.NAME, CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		wavegoodbye.Enter(PlayGroomingPstAnim).OnAnimQueueComplete(runaway).ToggleStatusItem(CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		runaway.MoveTo(GetRunawayCell, behaviourcomplete, behaviourcomplete).ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetRanched);
	}

	private static RanchStation.Instance GetRanchStation(Instance smi)
	{
		return smi.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
	}

	private static void ClearLayerOverride(Instance smi)
	{
		smi.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
	}

	private static void GetOnTable(Instance smi)
	{
		Navigator navigator = smi.Get<Navigator>();
		if (navigator.IsValidNavType(NavType.Floor))
		{
			navigator.SetCurrentNavType(NavType.Floor);
		}
		smi.Get<Facing>().SetFacing(mirror_x: false);
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedPreAnim);
	}

	private static void PlayGroomingLoopAnim(Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedLoopAnim, KAnim.PlayMode.Loop);
	}

	private static void PlayGroomingPstAnim(Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(GetRanchStation(smi).def.ranchedPstAnim);
	}

	private static int GetTargetRanchCell(Instance smi)
	{
		return GetRanchStation(smi).GetTargetRanchCell();
	}

	private static int GetRunawayCell(Instance smi)
	{
		int cell = Grid.PosToCell(smi.transform.GetPosition());
		int num = Grid.OffsetCell(cell, 2, 0);
		if (Grid.Solid[num])
		{
			num = Grid.OffsetCell(cell, -2, 0);
		}
		return num;
	}
}
