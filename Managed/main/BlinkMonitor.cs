using UnityEngine;

public class BlinkMonitor : GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public class Tuning : TuningData<Tuning>
	{
		public float randomBlinkIntervalMin;

		public float randomBlinkIntervalMax;
	}

	public new class Instance : GameInstance
	{
		public KBatchedAnimController eyes;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool IsBlinking()
		{
			return IsInsideState(base.sm.blinking);
		}

		public void Blink()
		{
			GoTo(base.sm.blinking);
		}
	}

	public State satisfied;

	public State blinking;

	public TargetParameter eyes;

	private static HashedString HASH_SNAPTO_EYES = "snapto_eyes";

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Enter(CreateEyes).Exit(DestroyEyes);
		satisfied.ScheduleGoTo(GetRandomBlinkTime, blinking);
		blinking.EnterTransition(satisfied, GameStateMachine<BlinkMonitor, Instance, IStateMachineTarget, Def>.Not(CanBlink)).Enter(BeginBlinking).Update(UpdateBlinking, UpdateRate.RENDER_EVERY_TICK)
			.Target(eyes)
			.OnAnimQueueComplete(satisfied)
			.Exit(EndBlinking);
	}

	private static bool CanBlink(Instance smi)
	{
		return SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject) && smi.Get<Navigator>().CurrentNavType != NavType.Ladder;
	}

	private static float GetRandomBlinkTime(Instance smi)
	{
		return Random.Range(TuningData<Tuning>.Get().randomBlinkIntervalMin, TuningData<Tuning>.Get().randomBlinkIntervalMax);
	}

	private static void CreateEyes(Instance smi)
	{
		smi.eyes = Util.KInstantiate(Assets.GetPrefab(EyeAnimation.ID)).GetComponent<KBatchedAnimController>();
		smi.eyes.gameObject.SetActive(value: true);
		smi.sm.eyes.Set(smi.eyes.gameObject, smi);
	}

	private static void DestroyEyes(Instance smi)
	{
		if (smi.eyes != null)
		{
			Util.KDestroyGameObject(smi.eyes);
			smi.eyes = null;
		}
	}

	public static void BeginBlinking(Instance smi)
	{
		string s = "eyes1";
		smi.eyes.Play(s);
		UpdateBlinking(smi, 0f);
	}

	public static void EndBlinking(Instance smi)
	{
		smi.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(HASH_SNAPTO_EYES, 3);
	}

	public static void UpdateBlinking(Instance smi, float dt)
	{
		int currentFrameIndex = smi.eyes.GetCurrentFrameIndex();
		KAnimBatch batch = smi.eyes.GetBatch();
		if (currentFrameIndex == -1 || batch == null)
		{
			return;
		}
		KAnim.Anim.Frame frame = smi.eyes.GetBatch().group.data.GetFrame(currentFrameIndex);
		if (frame == KAnim.Anim.Frame.InvalidFrame)
		{
			return;
		}
		HashedString hash = HashedString.Invalid;
		for (int i = 0; i < frame.numElements; i++)
		{
			int num = frame.firstElementIdx + i;
			if (num < batch.group.data.frameElements.Count)
			{
				KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[num];
				if (!(frameElement.symbol == HashedString.Invalid))
				{
					hash = frameElement.symbol;
					break;
				}
			}
		}
		smi.GetComponent<SymbolOverrideController>().AddSymbolOverride(HASH_SNAPTO_EYES, smi.eyes.AnimFiles[0].GetData().build.GetSymbol(hash), 3);
	}
}
