using Klei.AI;
using UnityEngine;

public class SparkleStreaker : GameStateMachine<SparkleStreaker, SparkleStreaker.Instance>
{
	public class OverjoyedStates : State
	{
		public State idle;

		public State moving;
	}

	public new class Instance : GameInstance
	{
		private Reactable passerbyReactable;

		public GameObject sparkleStreakFX;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void CreatePasserbyReactable()
		{
			if (passerbyReactable == null)
			{
				passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim", 5, 5, 0f, 600f).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_pre",
					startcb = AddReactionEffect
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_loop"
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "cheer_pst"
				})
					.AddThought(Db.Get().Thoughts.Happy)
					.AddPrecondition(ReactorIsOnFloor);
			}
		}

		private void AddReactionEffect(GameObject reactor)
		{
			reactor.GetComponent<Effects>().Add("SawSparkleStreaker", should_save: true);
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

		public bool IsMoving()
		{
			return base.smi.master.GetComponent<Navigator>().IsMoving();
		}

		public void SetSparkleSoundParam(float val)
		{
			GetComponent<LoopingSounds>().SetParameter(GlobalAssets.GetSound("SparkleStreaker_lp"), "sparkleStreaker_moving", val);
		}
	}

	private Vector3 offset = new Vector3(0f, 0f, 0.1f);

	public State neutral;

	public OverjoyedStates overjoyed;

	public string soundPath = GlobalAssets.GetSound("SparkleStreaker_lp");

	public HashedString SPARKLE_STREAKER_MOVING_PARAMETER = "sparkleStreaker_moving";

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		root.TagTransition(GameTags.Dead, null);
		neutral.TagTransition(GameTags.Overjoyed, overjoyed);
		overjoyed.DefaultState(overjoyed.idle).TagTransition(GameTags.Overjoyed, neutral, on_remove: true).ToggleEffect("IsSparkleStreaker")
			.ToggleLoopingSound(soundPath)
			.Enter(delegate(Instance smi)
			{
				smi.sparkleStreakFX = Util.KInstantiate(EffectPrefabs.Instance.SparkleStreakFX, smi.master.transform.GetPosition() + offset);
				smi.sparkleStreakFX.transform.SetParent(smi.master.transform);
				smi.sparkleStreakFX.SetActive(value: true);
				smi.CreatePasserbyReactable();
			})
			.Exit(delegate(Instance smi)
			{
				Util.KDestroyGameObject(smi.sparkleStreakFX);
				smi.ClearPasserbyReactable();
			});
		overjoyed.idle.Enter(delegate(Instance smi)
		{
			smi.SetSparkleSoundParam(0f);
		}).EventTransition(GameHashes.ObjectMovementStateChanged, overjoyed.moving, (Instance smi) => smi.IsMoving());
		overjoyed.moving.Enter(delegate(Instance smi)
		{
			smi.SetSparkleSoundParam(1f);
		}).EventTransition(GameHashes.ObjectMovementStateChanged, overjoyed.idle, (Instance smi) => !smi.IsMoving());
	}
}
