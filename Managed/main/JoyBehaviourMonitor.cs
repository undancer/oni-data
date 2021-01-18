using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

public class JoyBehaviourMonitor : GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public string happyLocoAnim = "";

		public string happyLocoWalkAnim = "";

		public Expression happyExpression;

		[Serialize]
		public float transitionTime;

		private AttributeInstance expectationAttribute;

		private AttributeInstance qolAttribute;

		public Instance(IStateMachineTarget master, string happy_loco_anim, string happy_loco_walk_anim, Expression happy_expression)
			: base(master)
		{
			happyLocoAnim = happy_loco_anim;
			happyLocoWalkAnim = happy_loco_walk_anim;
			happyExpression = happy_expression;
			Attributes attributes = base.gameObject.GetAttributes();
			expectationAttribute = attributes.Add(Db.Get().Attributes.QualityOfLifeExpectation);
			qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(base.gameObject);
		}

		public bool ShouldBeOverjoyed()
		{
			float totalValue = qolAttribute.GetTotalValue();
			float totalValue2 = expectationAttribute.GetTotalValue();
			float num = totalValue - totalValue2;
			if (num >= TRAITS.JOY_REACTIONS.MIN_MORALE_EXCESS)
			{
				float num2 = MathUtil.ReRange(num, TRAITS.JOY_REACTIONS.MIN_MORALE_EXCESS, TRAITS.JOY_REACTIONS.MAX_MORALE_EXCESS, TRAITS.JOY_REACTIONS.MIN_REACTION_CHANCE, TRAITS.JOY_REACTIONS.MAX_REACTION_CHANCE);
				return Random.Range(0f, 100f) <= num2;
			}
			return false;
		}

		public void GoToOverjoyed()
		{
			base.smi.transitionTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.JOY_REACTION_DURATION;
			base.smi.GoTo(base.smi.sm.overjoyed);
		}
	}

	public Signal exitEarly;

	public State neutral;

	public State overjoyed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		base.serializable = true;
		root.TagTransition(GameTags.Dead, null);
		neutral.EventHandler(GameHashes.SleepFinished, delegate(Instance smi)
		{
			if (smi.ShouldBeOverjoyed())
			{
				smi.GoToOverjoyed();
			}
		});
		overjoyed.Transition(neutral, (Instance smi) => GameClock.Instance.GetTime() >= smi.transitionTime).ToggleExpression((Instance smi) => smi.happyExpression).ToggleAnims((Instance smi) => smi.happyLocoAnim)
			.ToggleAnims((Instance smi) => smi.happyLocoWalkAnim)
			.ToggleTag(GameTags.Overjoyed)
			.OnSignal(exitEarly, neutral);
	}
}
