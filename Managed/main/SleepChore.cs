using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SleepChore, object>.GameInstance
	{
		public bool hadPeacefulSleep;

		public bool hadNormalSleep;

		public bool hadBadSleep;

		public bool hadTerribleSleep;

		public int lastEvaluatedDay = -1;

		public float wakeUpBuffer = 2f;

		public string stateChangeNoiseSource;

		private GameObject locator;

		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed, bool bedIsLocator, bool isInterruptable)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.isInterruptable.Set(isInterruptable, base.smi);
			if (bedIsLocator)
			{
				AddLocator(bed);
			}
			else
			{
				base.sm.bed.Set(bed, base.smi);
			}
		}

		public void CheckLightLevel()
		{
			GameObject go = base.sm.sleeper.Get(base.smi);
			int cell = Grid.PosToCell(go);
			if (Grid.IsValidCell(cell) && !IsLightLevelOk(cell) && !IsLoudSleeper())
			{
				go.Trigger(-1063113160);
			}
		}

		public bool IsLoudSleeper()
		{
			if (base.sm.sleeper.Get(base.smi).GetComponent<Snorer>() != null)
			{
				return true;
			}
			return false;
		}

		public void EvaluateSleepQuality()
		{
		}

		public void AddLocator(GameObject sleepable)
		{
			locator = sleepable;
			int i = Grid.PosToCell(locator);
			Grid.Reserved[i] = true;
			base.sm.bed.Set(locator, this);
		}

		public void DestroyLocator()
		{
			if (locator != null)
			{
				Grid.Reserved[Grid.PosToCell(locator)] = false;
				ChoreHelpers.DestroyLocator(locator);
				base.sm.bed.Set(null, this);
				locator = null;
			}
		}

		public void SetAnim()
		{
			Sleepable sleepable = base.sm.bed.Get<Sleepable>(base.smi);
			if (sleepable.GetComponent<Building>() == null)
			{
				string s = base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType switch
				{
					NavType.Ladder => "anim_sleep_ladder_kanim", 
					NavType.Pole => "anim_sleep_pole_kanim", 
					_ => "anim_sleep_floor_kanim", 
				};
				sleepable.overrideAnims = new KAnimFile[1]
				{
					Assets.GetAnim(s)
				};
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SleepChore>
	{
		public class SleepStates : State
		{
			public State condition_transition;

			public State condition_transition_pre;

			public State uninterruptable;

			public State normal;

			public State interrupt_noise;

			public State interrupt_noise_transition;

			public State interrupt_light;

			public State interrupt_light_transition;
		}

		public TargetParameter sleeper;

		public TargetParameter bed;

		public BoolParameter isInterruptable;

		public BoolParameter isDisturbedByNoise;

		public BoolParameter isDisturbedByLight;

		public ApproachSubState<IApproachable> approach;

		public SleepStates sleep;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(sleeper);
			root.Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			approach.InitializeStates(sleeper, bed, sleep);
			sleep.Enter("SetAnims", delegate(StatesInstance smi)
			{
				smi.SetAnim();
			}).DefaultState(sleep.normal).ToggleTag(GameTags.Asleep)
				.DoSleep(sleeper, bed, success, null)
				.TriggerOnExit(GameHashes.SleepFinished)
				.EventHandler(GameHashes.SleepDisturbedByLight, delegate(StatesInstance smi)
				{
					isDisturbedByLight.Set(value: true, smi);
				})
				.EventHandler(GameHashes.SleepDisturbedByNoise, delegate(StatesInstance smi)
				{
					isDisturbedByNoise.Set(value: true, smi);
				});
			sleep.uninterruptable.DoNothing();
			sleep.normal.ParamTransition(isInterruptable, sleep.uninterruptable, GameStateMachine<States, StatesInstance, SleepChore, object>.IsFalse).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Sleeping).QueueAnim("working_loop", loop: true)
				.ParamTransition(isDisturbedByNoise, sleep.interrupt_noise, GameStateMachine<States, StatesInstance, SleepChore, object>.IsTrue)
				.ParamTransition(isDisturbedByLight, sleep.interrupt_light, GameStateMachine<States, StatesInstance, SleepChore, object>.IsTrue)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.CheckLightLevel();
				});
			sleep.interrupt_noise.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByNoise).QueueAnim("interrupt_light").OnAnimQueueComplete(sleep.interrupt_noise_transition);
			sleep.interrupt_noise_transition.Enter(delegate(StatesInstance smi)
			{
				Effects component = smi.master.GetComponent<Effects>();
				component.Add(Db.Get().effects.Get("TerribleSleep"), should_save: true);
				if (component.HasEffect(Db.Get().effects.Get("BadSleep")))
				{
					component.Remove(Db.Get().effects.Get("BadSleep"));
				}
				isDisturbedByNoise.Set(value: false, smi);
				State state2 = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? sleep.normal : success);
				smi.GoTo(state2);
			});
			sleep.interrupt_light.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByLight).QueueAnim("interrupt").OnAnimQueueComplete(sleep.interrupt_light_transition);
			sleep.interrupt_light_transition.Enter(delegate(StatesInstance smi)
			{
				if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
				{
					smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleep"), should_save: true);
				}
				State state = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? sleep.normal : success);
				isDisturbedByLight.Set(value: false, smi);
				smi.GoTo(state);
			});
			success.Enter(delegate(StatesInstance smi)
			{
				smi.EvaluateSleepQuality();
			}).ReturnSuccess();
		}
	}

	public static readonly Precondition IsOkayTimeToSleep = new Precondition
	{
		id = "IsOkayTimeToSleep",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OKAY_TIME_TO_SLEEP,
		fn = delegate(ref Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumerState.consumer.GetComponent<Narcolepsy>();
			bool num = component != null && component.IsNarcolepsing();
			bool flag = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>()?.NeedsToSleep() ?? false;
			bool flag2 = ChorePreconditions.instance.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return num || flag2 || flag;
		}
	};

	public SleepChore(ChoreType choreType, IStateMachineTarget target, GameObject bed, bool bedIsLocator, bool isInterruptable)
		: base(choreType, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, bed, bedIsLocator, isInterruptable);
		if (isInterruptable)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		}
		AddPrecondition(IsOkayTimeToSleep);
		Operational component = bed.GetComponent<Operational>();
		if (component != null)
		{
			AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
	}

	public static Sleepable GetSafeFloorLocator(GameObject sleeper)
	{
		int num = sleeper.GetComponent<Sensors>().GetSensor<SafeCellSensor>().GetSleepCellQuery();
		if (num == Grid.InvalidCell)
		{
			num = Grid.PosToCell(sleeper.transform.GetPosition());
		}
		return ChoreHelpers.CreateSleepLocator(Grid.CellToPosCBC(num, Grid.SceneLayer.Move)).GetComponent<Sleepable>();
	}

	public static bool IsLightLevelOk(int cell)
	{
		return Grid.LightIntensity[cell] <= 0;
	}
}
