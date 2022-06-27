using System;
using Klei.AI;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private AttributeInstance sneezyness;

		private StatusItem statusItem;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			sneezyness = Db.Get().Attributes.Sneezyness.Lookup(master.gameObject);
			OnSneezyChange();
			AttributeInstance attributeInstance = sneezyness;
			attributeInstance.OnDirty = (System.Action)Delegate.Combine(attributeInstance.OnDirty, new System.Action(OnSneezyChange));
		}

		public override void StopSM(string reason)
		{
			AttributeInstance attributeInstance = sneezyness;
			attributeInstance.OnDirty = (System.Action)Delegate.Remove(attributeInstance.OnDirty, new System.Action(OnSneezyChange));
			base.StopSM(reason);
		}

		public float NextSneezeInterval()
		{
			if (sneezyness.GetTotalValue() <= 0f)
			{
				return 70f;
			}
			float num = (IsMinorSneeze() ? 140f : 70f) / sneezyness.GetTotalValue();
			return UnityEngine.Random.Range(num * 0.7f, num * 1.3f);
		}

		public bool IsMinorSneeze()
		{
			return sneezyness.GetTotalValue() <= 5f;
		}

		private void OnSneezyChange()
		{
			base.smi.sm.isSneezy.Set(sneezyness.GetTotalValue() > 0f, base.smi);
		}

		public Reactable GetReactable()
		{
			if (IsMinorSneeze())
			{
				float min_reactor_time = NextSneezeInterval();
				return new SelfEmoteReactable(base.master.gameObject, "Sneeze", Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim", 0f, min_reactor_time).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "sneeze_short",
					startcb = TriggerDisurbance
				}).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "sneeze_short_pst",
					finishcb = ResetSneeze
				});
			}
			float min_reactor_time2 = NextSneezeInterval();
			return new SelfEmoteReactable(base.master.gameObject, "Sneeze", Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim", 0f, min_reactor_time2).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "sneeze",
				startcb = TriggerDisurbance
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "sneeze_pst",
				finishcb = ResetSneeze
			});
		}

		private void TriggerDisurbance(GameObject go)
		{
			if (IsMinorSneeze())
			{
				AcousticDisturbance.Emit(go, 2);
			}
			else
			{
				AcousticDisturbance.Emit(go, 3);
			}
		}

		private void ResetSneeze(GameObject go)
		{
			base.smi.GoTo(base.sm.idle);
		}
	}

	public BoolParameter isSneezy = new BoolParameter(default_value: false);

	public State idle;

	public State taking_medicine;

	public State sneezy;

	public const float SINGLE_SNEEZE_TIME_MINOR = 140f;

	public const float SINGLE_SNEEZE_TIME_MAJOR = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public const float SHORT_SNEEZE_THRESHOLD = 5f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.ParamTransition(isSneezy, sneezy, (Instance smi, bool p) => p);
		sneezy.ParamTransition(isSneezy, idle, (Instance smi, bool p) => !p).ToggleReactable((Instance smi) => smi.GetReactable());
	}
}
