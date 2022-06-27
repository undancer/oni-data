using UnityEngine;

public class HappySinger : GameStateMachine<HappySinger, HappySinger.Instance>
{
	public class OverjoyedStates : State
	{
		public State idle;

		public State moving;
	}

	public new class Instance : GameInstance
	{
		private Reactable passerbyReactable;

		public GameObject musicParticleFX;

		public SpeechMonitor.Instance speechMonitor;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void CreatePasserbyReactable()
		{
			if (passerbyReactable == null)
			{
				passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_react_singer_kanim", 5, 5, 0f, 600f).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "react",
					startcb = AddReactionEffect
				}).AddThought(Db.Get().Thoughts.CatchyTune).AddPrecondition(ReactorIsOnFloor);
			}
		}

		public SpeechMonitor.Instance GetSpeechMonitor()
		{
			if (speechMonitor == null)
			{
				speechMonitor = base.master.gameObject.GetSMI<SpeechMonitor.Instance>();
			}
			return speechMonitor;
		}

		private void AddReactionEffect(GameObject reactor)
		{
			reactor.Trigger(-1278274506);
		}

		private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
		{
			return transition.end == NavType.Floor;
		}

		public void ClearPasserbyReactable()
		{
			if (passerbyReactable != null)
			{
				passerbyReactable.Cleanup();
				passerbyReactable = null;
			}
		}
	}

	private Vector3 offset = new Vector3(0f, 0f, 0.1f);

	public State neutral;

	public OverjoyedStates overjoyed;

	public string soundPath = GlobalAssets.GetSound("DupeSinging_NotesFX_LP");

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		root.TagTransition(GameTags.Dead, null);
		neutral.TagTransition(GameTags.Overjoyed, overjoyed);
		overjoyed.DefaultState(overjoyed.idle).TagTransition(GameTags.Overjoyed, neutral, on_remove: true).ToggleEffect("IsJoySinger")
			.ToggleLoopingSound(soundPath)
			.ToggleAnims("anim_loco_singer_kanim")
			.ToggleAnims("anim_idle_singer_kanim")
			.EventHandler(GameHashes.TagsChanged, delegate(Instance smi, object obj)
			{
				smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
			})
			.Enter(delegate(Instance smi)
			{
				smi.musicParticleFX = Util.KInstantiate(EffectPrefabs.Instance.HappySingerFX, smi.master.transform.GetPosition() + offset);
				smi.musicParticleFX.transform.SetParent(smi.master.transform);
				smi.CreatePasserbyReactable();
				smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
			})
			.Update(delegate(Instance smi, float dt)
			{
				if (!smi.GetSpeechMonitor().IsPlayingSpeech() && SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
				{
					smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
				}
			}, UpdateRate.SIM_1000ms)
			.Exit(delegate(Instance smi)
			{
				Util.KDestroyGameObject(smi.musicParticleFX);
				smi.ClearPasserbyReactable();
				smi.musicParticleFX.SetActive(value: false);
			});
	}
}
