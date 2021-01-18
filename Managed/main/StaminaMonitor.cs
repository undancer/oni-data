using Klei.AI;

public class StaminaMonitor : GameStateMachine<StaminaMonitor, StaminaMonitor.Instance>
{
	public class SleepyState : State
	{
		public State needssleep;

		public State sleeping;
	}

	public new class Instance : GameInstance
	{
		private ChoreDriver choreDriver;

		private Schedulable schedulable;

		public AmountInstance stamina;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			stamina = Db.Get().Amounts.Stamina.Lookup(base.gameObject);
			choreDriver = GetComponent<ChoreDriver>();
			schedulable = GetComponent<Schedulable>();
		}

		public bool NeedsToSleep()
		{
			return stamina.value <= 0f;
		}

		public bool WantsToSleep()
		{
			return choreDriver.HasChore() && choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Sleep);
		}

		public void TryExitSleepState()
		{
			if (!NeedsToSleep() && !WantsToSleep())
			{
				base.smi.GoTo(base.smi.sm.satisfied);
			}
		}

		public bool IsSleeping()
		{
			bool result = false;
			if (WantsToSleep())
			{
				Worker component = choreDriver.GetComponent<Worker>();
				Workable workable = component.workable;
				if (workable != null)
				{
					result = true;
				}
			}
			return result;
		}

		public void CheckDebugFastWorkMode()
		{
			if (Game.Instance.FastWorkersModeActive)
			{
				stamina.value = stamina.GetMax();
			}
		}

		public bool ShouldExitSleep()
		{
			if (schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
			{
				return false;
			}
			Narcolepsy component = GetComponent<Narcolepsy>();
			if (component != null && component.IsNarcolepsing())
			{
				return false;
			}
			if (stamina.value < stamina.GetMax())
			{
				return false;
			}
			return true;
		}
	}

	public State satisfied;

	public SleepyState sleepy;

	private const float OUTSIDE_SCHEDULE_STAMINA_THRESHOLD = 0f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = SerializeType.Both_DEPRECATED;
		root.ToggleStateMachine((Instance smi) => new UrgeMonitor.Instance(smi.master, Db.Get().Urges.Sleep, Db.Get().Amounts.Stamina, Db.Get().ScheduleBlockTypes.Sleep, 100f, 0f, is_threshold_minimum: false)).ToggleStateMachine((Instance smi) => new SleepChoreMonitor.Instance(smi.master));
		satisfied.Transition(sleepy, (Instance smi) => smi.NeedsToSleep() || smi.WantsToSleep());
		sleepy.Update("Check Sleep State", delegate(Instance smi, float dt)
		{
			smi.TryExitSleepState();
		}, UpdateRate.SIM_1000ms).DefaultState(sleepy.needssleep);
		sleepy.needssleep.Transition(sleepy.sleeping, (Instance smi) => smi.IsSleeping()).ToggleExpression(Db.Get().Expressions.Tired).ToggleStatusItem(Db.Get().DuplicantStatusItems.Tired)
			.ToggleThought(Db.Get().Thoughts.Sleepy);
		sleepy.sleeping.Enter(delegate(Instance smi)
		{
			smi.CheckDebugFastWorkMode();
		}).Transition(satisfied, (Instance smi) => !smi.IsSleeping());
	}
}
