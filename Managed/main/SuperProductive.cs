using STRINGS;
using TUNING;
using UnityEngine;

public class SuperProductive : GameStateMachine<SuperProductive, SuperProductive.Instance>
{
	public class OverjoyedStates : State
	{
		public State idle;

		public State working;

		public State superProductive;
	}

	public new class Instance : GameInstance
	{
		public SuperProductiveFX.Instance fx;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool ShouldSkipWork()
		{
			return Random.Range(0f, 100f) <= TRAITS.JOY_REACTIONS.SUPER_PRODUCTIVE.INSTANT_SUCCESS_CHANCE;
		}

		public void ReactSuperProductive()
		{
			ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (sMI != null)
			{
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, "SuperProductive", Db.Get().ChoreTypes.EmoteHighPriority, "anim_productive_kanim", 0f, 1f, 1f);
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = "productive"
				});
				sMI.AddOneshotReactable(selfEmoteReactable);
			}
		}
	}

	public State neutral;

	public OverjoyedStates overjoyed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		root.TagTransition(GameTags.Dead, null);
		neutral.TagTransition(GameTags.Overjoyed, overjoyed);
		overjoyed.TagTransition(GameTags.Overjoyed, neutral, on_remove: true).ToggleStatusItem(Db.Get().DuplicantStatusItems.BeingProductive).Enter(delegate(Instance smi)
		{
			if (PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, smi.master.transform, new Vector3(0f, 0.5f, 0f));
			}
			smi.fx = new SuperProductiveFX.Instance(smi.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
			smi.fx.StartSM();
		})
			.Exit(delegate(Instance smi)
			{
				smi.fx.sm.destroyFX.Trigger(smi.fx);
			})
			.DefaultState(overjoyed.idle);
		overjoyed.idle.EventTransition(GameHashes.StartWork, overjoyed.working);
		overjoyed.working.ScheduleGoTo(0.33f, overjoyed.superProductive);
		overjoyed.superProductive.Enter(delegate(Instance smi)
		{
			Worker component = smi.GetComponent<Worker>();
			if (component != null && component.state == Worker.State.Working)
			{
				Workable workable = component.workable;
				if (workable != null)
				{
					float num = workable.WorkTimeRemaining;
					if (workable.GetComponent<Diggable>() != null)
					{
						num = Diggable.GetApproximateDigTime(Grid.PosToCell(workable));
					}
					if (num > 1f && smi.ShouldSkipWork() && component.InstantlyFinish())
					{
						smi.ReactSuperProductive();
						smi.fx.sm.wasProductive.Trigger(smi.fx);
					}
				}
			}
			smi.GoTo(overjoyed.idle);
		});
	}
}
