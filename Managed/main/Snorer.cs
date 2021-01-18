using UnityEngine;

[SkipSaveFileSerialization]
public class Snorer : StateMachineComponent<Snorer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Snorer, object>.GameInstance
	{
		private SchedulerHandle snoreHandle;

		private KBatchedAnimController snoreEffect;

		private KBatchedAnimController snoreBGEffect;

		private const float BGEmissionRadius = 3f;

		public StatesInstance(Snorer master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<StaminaMonitor.Instance>()?.IsSleeping() ?? false;
		}

		public void StartSmallSnore()
		{
			snoreHandle = GameScheduler.Instance.Schedule("snorelines", 2f, StartSmallSnoreInternal);
		}

		private void StartSmallSnoreInternal(object data)
		{
			snoreHandle.ClearScheduler();
			bool symbolVisible;
			Matrix4x4 symbolTransform = base.smi.master.GetComponent<KBatchedAnimController>().GetSymbolTransform(HeadHash, out symbolVisible);
			if (symbolVisible)
			{
				Vector3 position = symbolTransform.GetColumn(3);
				position.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				snoreEffect = FXHelpers.CreateEffect("snore_fx_kanim", position);
				snoreEffect.destroyOnAnimComplete = true;
				snoreEffect.Play("snore", KAnim.PlayMode.Loop);
			}
		}

		public void StopSmallSnore()
		{
			snoreHandle.ClearScheduler();
			if (snoreEffect != null)
			{
				snoreEffect.PlayMode = KAnim.PlayMode.Once;
			}
			snoreEffect = null;
		}

		public void StartSnoreBGEffect()
		{
			AcousticDisturbance.Emit(base.smi.master.gameObject, 3);
		}

		public void StopSnoreBGEffect()
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Snorer>
	{
		public class SleepStates : State
		{
			public State quiet;

			public State snoring;
		}

		public State idle;

		public SleepStates sleeping;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, null);
			idle.Transition(sleeping, (StatesInstance smi) => smi.IsSleeping());
			sleeping.DefaultState(sleeping.quiet).Enter(delegate(StatesInstance smi)
			{
				smi.StartSmallSnore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.StopSmallSnore();
			})
				.Transition(idle, (StatesInstance smi) => !smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping());
			sleeping.quiet.Enter("ScheduleNextSnore", delegate(StatesInstance smi)
			{
				smi.ScheduleGoTo(GetNewInterval(), sleeping.snoring);
			});
			sleeping.snoring.Enter(delegate(StatesInstance smi)
			{
				smi.StartSnoreBGEffect();
			}).ToggleExpression(Db.Get().Expressions.Relief).ScheduleGoTo(3f, sleeping.quiet)
				.Exit(delegate(StatesInstance smi)
				{
					smi.StopSnoreBGEffect();
				});
		}

		private float GetNewInterval()
		{
			return Mathf.Min(Mathf.Max(Util.GaussianRandom(5f), 3f), 10f);
		}
	}

	private static readonly HashedString HeadHash = "snapTo_mouth";

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
