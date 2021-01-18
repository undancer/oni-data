using Klei.AI;

public class CalorieMonitor : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance>
{
	public class HungryState : State
	{
		public State working;

		public State normal;

		public State starving;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance calories;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
		}

		private float GetCalories0to1()
		{
			return calories.value / calories.GetMax();
		}

		public bool IsEatTime()
		{
			return base.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Eat);
		}

		public bool IsHungry()
		{
			return GetCalories0to1() < 0.825f;
		}

		public bool IsStarving()
		{
			return GetCalories0to1() < 0.25f;
		}

		public bool IsSatisfied()
		{
			return GetCalories0to1() > 0.95f;
		}

		public bool IsEating()
		{
			ChoreDriver component = base.master.GetComponent<ChoreDriver>();
			if (component.HasChore())
			{
				return component.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
			}
			return false;
		}

		public bool IsDepleted()
		{
			return calories.value <= 0f;
		}

		public bool ShouldExitInfirmary()
		{
			return !IsStarving();
		}

		public void Kill()
		{
			if (base.gameObject.GetSMI<DeathMonitor.Instance>() != null)
			{
				base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
			}
		}
	}

	public State satisfied;

	public HungryState hungry;

	public State eating;

	public State incapacitated;

	public State depleted;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		satisfied.Transition(hungry, (Instance smi) => smi.IsHungry());
		hungry.DefaultState(hungry.normal).Transition(satisfied, (Instance smi) => smi.IsSatisfied()).EventTransition(GameHashes.BeginChore, eating, (Instance smi) => smi.IsEating());
		hungry.working.EventTransition(GameHashes.ScheduleBlocksChanged, hungry.normal, (Instance smi) => smi.IsEatTime()).Transition(hungry.starving, (Instance smi) => smi.IsStarving()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry);
		hungry.normal.EventTransition(GameHashes.ScheduleBlocksChanged, hungry.working, (Instance smi) => !smi.IsEatTime()).Transition(hungry.starving, (Instance smi) => smi.IsStarving()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry)
			.ToggleUrge(Db.Get().Urges.Eat)
			.ToggleExpression(Db.Get().Expressions.Hungry)
			.ToggleThought(Db.Get().Thoughts.Starving);
		hungry.starving.Transition(hungry.normal, (Instance smi) => !smi.IsStarving()).Transition(depleted, (Instance smi) => smi.IsDepleted()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Starving)
			.ToggleUrge(Db.Get().Urges.Eat)
			.ToggleExpression(Db.Get().Expressions.Hungry)
			.ToggleThought(Db.Get().Thoughts.Starving);
		eating.EventTransition(GameHashes.EndChore, satisfied, (Instance smi) => !smi.IsEating());
		depleted.ToggleTag(GameTags.CaloriesDepleted).Enter(delegate(Instance smi)
		{
			smi.Kill();
		});
	}
}
