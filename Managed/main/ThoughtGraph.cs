using System.Collections.Generic;

public class ThoughtGraph : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance>
{
	public class Tuning : TuningData<Tuning>
	{
		public float preLengthInSeconds;
	}

	public class DisplayingThoughtState : State
	{
		public State pre;

		public State talking;
	}

	public new class Instance : GameInstance
	{
		private List<Thought> thoughts = new List<Thought>();

		public Thought currentThought;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
		}

		public bool HasThoughts()
		{
			return thoughts.Count > 0;
		}

		public bool HasImmediateThought()
		{
			bool result = false;
			for (int i = 0; i < thoughts.Count; i++)
			{
				if (thoughts[i].showImmediately)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public void AddThought(Thought thought)
		{
			if (!thoughts.Contains(thought))
			{
				thoughts.Add(thought);
				if (thought.showImmediately)
				{
					base.sm.thoughtsChangedImmediate.Trigger(base.smi);
				}
				else
				{
					base.sm.thoughtsChanged.Trigger(base.smi);
				}
			}
		}

		public void RemoveThought(Thought thought)
		{
			if (thoughts.Contains(thought))
			{
				thoughts.Remove(thought);
				base.sm.thoughtsChanged.Trigger(base.smi);
			}
		}

		private int SortThoughts(Thought a, Thought b)
		{
			if (a.showImmediately != b.showImmediately)
			{
				if (!a.showImmediately)
				{
					return 1;
				}
				return -1;
			}
			return b.priority.CompareTo(a.priority);
		}

		public void CreateBubble()
		{
			if (thoughts.Count != 0)
			{
				thoughts.Sort(SortThoughts);
				Thought thought = thoughts[0];
				if (thought.modeSprite != null)
				{
					NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(base.gameObject, bVisible: true, thought.hoverText, thought.bubbleSprite, thought.sprite, thought.modeSprite);
				}
				else
				{
					NameDisplayScreen.Instance.SetThoughtBubbleDisplay(base.gameObject, bVisible: true, thought.hoverText, thought.bubbleSprite, thought.sprite);
				}
				base.sm.thoughtDisplayTime.Set(thought.showTime, this);
				currentThought = thought;
				if (thought.showImmediately)
				{
					thoughts.RemoveAt(0);
				}
			}
		}

		public void DestroyBubble()
		{
			NameDisplayScreen.Instance.SetThoughtBubbleDisplay(base.gameObject, bVisible: false, null, null, null);
			NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(base.gameObject, bVisible: false, null, null, null, null);
		}
	}

	public Signal thoughtsChanged;

	public Signal thoughtsChangedImmediate;

	public FloatParameter thoughtDisplayTime;

	public State initialdelay;

	public State nothoughts;

	public DisplayingThoughtState displayingthought;

	public State cooldown;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = initialdelay;
		initialdelay.ScheduleGoTo(1f, nothoughts);
		nothoughts.OnSignal(thoughtsChanged, displayingthought, (Instance smi) => smi.HasThoughts()).OnSignal(thoughtsChangedImmediate, displayingthought, (Instance smi) => smi.HasThoughts());
		displayingthought.DefaultState(displayingthought.pre).Enter("CreateBubble", delegate(Instance smi)
		{
			smi.CreateBubble();
		}).Exit("DestroyBubble", delegate(Instance smi)
		{
			smi.DestroyBubble();
		})
			.ScheduleGoTo((Instance smi) => thoughtDisplayTime.Get(smi), cooldown);
		displayingthought.pre.ScheduleGoTo((Instance smi) => TuningData<Tuning>.Get().preLengthInSeconds, displayingthought.talking);
		displayingthought.talking.Enter(BeginTalking);
		cooldown.OnSignal(thoughtsChangedImmediate, displayingthought, (Instance smi) => smi.HasImmediateThought()).ScheduleGoTo(20f, nothoughts);
	}

	private static void BeginTalking(Instance smi)
	{
		if (smi.currentThought != null && SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
		{
			smi.GetSMI<SpeechMonitor.Instance>().PlaySpeech(smi.currentThought.speechPrefix, smi.currentThought.sound);
		}
	}
}
