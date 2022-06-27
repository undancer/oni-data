using UnityEngine;

public class InspirationEffectMonitor : GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public SpeechMonitor.Instance speechMonitor;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public SpeechMonitor.Instance GetSpeechMonitor()
		{
			if (speechMonitor == null)
			{
				speechMonitor = base.master.gameObject.GetSMI<SpeechMonitor.Instance>();
			}
			return speechMonitor;
		}
	}

	public BoolParameter shouldCatchyTune;

	public FloatParameter inspirationTimeRemaining;

	public State idle;

	public State catchyTune;

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		idle.EventHandler(GameHashes.CatchyTune, OnCatchyTune).ParamTransition(shouldCatchyTune, catchyTune, (Instance smi, bool shouldCatchyTune) => shouldCatchyTune);
		catchyTune.Exit(delegate(Instance smi)
		{
			shouldCatchyTune.Set(value: false, smi);
		}).ToggleEffect("HeardJoySinger").ToggleThought(Db.Get().Thoughts.CatchyTune)
			.EventHandler(GameHashes.StartWork, TryThinkCatchyTune)
			.Enter(delegate(Instance smi)
			{
				SingCatchyTune(smi);
			})
			.Update(delegate(Instance smi, float dt)
			{
				TryThinkCatchyTune(smi, null);
				inspirationTimeRemaining.Delta(0f - dt, smi);
			}, UpdateRate.SIM_4000ms)
			.ParamTransition(inspirationTimeRemaining, idle, (Instance smi, float p) => p <= 0f);
	}

	private void OnCatchyTune(Instance smi, object data)
	{
		inspirationTimeRemaining.Set(600f, smi);
		shouldCatchyTune.Set(value: true, smi);
	}

	private void TryThinkCatchyTune(Instance smi, object data)
	{
		if (Random.Range(1, 101) > 66)
		{
			SingCatchyTune(smi);
		}
	}

	private void SingCatchyTune(Instance smi)
	{
		smi.master.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.CatchyTune);
		if (!smi.GetSpeechMonitor().IsPlayingSpeech() && SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
		{
			smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
		}
	}
}
