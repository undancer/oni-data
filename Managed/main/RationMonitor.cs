public class RationMonitor : GameStateMachine<RationMonitor, RationMonitor.Instance>
{
	public class EdibleAvailablestate : State
	{
		public State readytoeat;

		public State eating;
	}

	public class RationsAvailableState : State
	{
		public HungrySubState noediblesavailable;

		public HungrySubState ediblereachablebutnotpermitted;

		public HungrySubState ediblesunreachable;

		public EdibleAvailablestate edibleavailable;
	}

	public new class Instance : GameInstance
	{
		private ChoreDriver choreDriver;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			choreDriver = master.GetComponent<ChoreDriver>();
		}

		public Edible GetEdible()
		{
			return GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible();
		}

		public bool HasRationsAvailable()
		{
			return true;
		}

		public float GetRationsAteToday()
		{
			return base.sm.rationsAteToday.Get(base.smi);
		}

		public float GetRationsRemaining()
		{
			return 1f;
		}

		public bool IsEating()
		{
			if (choreDriver.HasChore())
			{
				return choreDriver.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
			}
			return false;
		}

		public void OnNewDay()
		{
			base.smi.sm.rationsAteToday.Set(0f, base.smi);
		}

		public void OnEatComplete(object data)
		{
			Edible edible = (Edible)data;
			base.sm.rationsAteToday.Delta(edible.caloriesConsumed, base.smi);
			RationTracker.Get().RegisterRationsConsumed(edible);
		}
	}

	public FloatParameter rationsAteToday;

	public RationsAvailableState rationsavailable;

	public HungrySubState outofrations;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = rationsavailable;
		base.serializable = true;
		root.EventHandler(GameHashes.EatCompleteEater, delegate(Instance smi, object d)
		{
			smi.OnEatComplete(d);
		}).EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.OnNewDay();
		}).ParamTransition(rationsAteToday, rationsavailable, (Instance smi, float p) => smi.HasRationsAvailable())
			.ParamTransition(rationsAteToday, outofrations, (Instance smi, float p) => !smi.HasRationsAvailable());
		rationsavailable.DefaultState(rationsavailable.noediblesavailable);
		rationsavailable.noediblesavailable.InitializeStates(masterTarget, Db.Get().DuplicantStatusItems.NoRationsAvailable).EventTransition(GameHashes.ColonyHasRationsChanged, GetSaveGame, rationsavailable.ediblesunreachable, AreThereAnyEdibles);
		rationsavailable.ediblereachablebutnotpermitted.InitializeStates(masterTarget, Db.Get().DuplicantStatusItems.RationsNotPermitted).EventTransition(GameHashes.ColonyHasRationsChanged, GetSaveGame, rationsavailable.noediblesavailable, AreThereNoEdibles).EventTransition(GameHashes.ClosestEdibleChanged, rationsavailable.ediblesunreachable, NotIsEdibleInReachButNotPermitted);
		rationsavailable.ediblesunreachable.InitializeStates(masterTarget, Db.Get().DuplicantStatusItems.RationsUnreachable).EventTransition(GameHashes.ColonyHasRationsChanged, GetSaveGame, rationsavailable.noediblesavailable, AreThereNoEdibles).EventTransition(GameHashes.ClosestEdibleChanged, rationsavailable.edibleavailable, IsEdibleAvailable)
			.EventTransition(GameHashes.ClosestEdibleChanged, rationsavailable.ediblereachablebutnotpermitted, IsEdibleInReachButNotPermitted);
		rationsavailable.edibleavailable.ToggleChore((Instance smi) => new EatChore(smi.master), rationsavailable.noediblesavailable).DefaultState(rationsavailable.edibleavailable.readytoeat);
		rationsavailable.edibleavailable.readytoeat.EventTransition(GameHashes.ClosestEdibleChanged, rationsavailable.noediblesavailable).EventTransition(GameHashes.BeginChore, rationsavailable.edibleavailable.eating, (Instance smi) => smi.IsEating());
		rationsavailable.edibleavailable.eating.DoNothing();
		outofrations.InitializeStates(masterTarget, Db.Get().DuplicantStatusItems.DailyRationLimitReached);
	}

	private static bool AreThereNoEdibles(Instance smi)
	{
		return !AreThereAnyEdibles(smi);
	}

	private static bool AreThereAnyEdibles(Instance smi)
	{
		if (SaveGame.Instance != null)
		{
			ColonyRationMonitor.Instance sMI = SaveGame.Instance.GetSMI<ColonyRationMonitor.Instance>();
			if (sMI != null)
			{
				return !sMI.IsOutOfRations();
			}
		}
		return false;
	}

	private static KMonoBehaviour GetSaveGame(Instance smi)
	{
		return SaveGame.Instance;
	}

	private static bool IsEdibleAvailable(Instance smi)
	{
		return smi.GetEdible() != null;
	}

	private static bool NotIsEdibleInReachButNotPermitted(Instance smi)
	{
		return !IsEdibleInReachButNotPermitted(smi);
	}

	private static bool IsEdibleInReachButNotPermitted(Instance smi)
	{
		return smi.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().edibleInReachButNotPermitted;
	}
}
