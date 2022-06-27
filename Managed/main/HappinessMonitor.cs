using Klei.AI;
using STRINGS;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public class Def : BaseDef
	{
		public float threshold;
	}

	public class UnhappyState : State
	{
		public State wild;

		public State tame;
	}

	public class HappyState : State
	{
		public State wild;

		public State tame;
	}

	public new class Instance : GameInstance
	{
		public AttributeInstance happiness;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			happiness = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
		}
	}

	private State satisfied;

	private HappyState happy;

	private UnhappyState unhappy;

	private Effect happyWildEffect;

	private Effect happyTameEffect;

	private Effect unhappyWildEffect;

	private Effect unhappyTameEffect;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(happy, IsHappy, UpdateRate.SIM_1000ms).Transition(unhappy, GameStateMachine<HappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		happy.DefaultState(happy.wild).Transition(satisfied, GameStateMachine<HappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		happy.wild.ToggleEffect((Instance smi) => happyWildEffect).TagTransition(GameTags.Creatures.Wild, happy.tame, on_remove: true);
		happy.tame.ToggleEffect((Instance smi) => happyTameEffect).TagTransition(GameTags.Creatures.Wild, happy.wild);
		unhappy.DefaultState(unhappy.wild).Transition(satisfied, IsHappy, UpdateRate.SIM_1000ms).ToggleTag(GameTags.Creatures.Unhappy);
		unhappy.wild.ToggleEffect((Instance smi) => unhappyWildEffect).TagTransition(GameTags.Creatures.Wild, unhappy.tame, on_remove: true);
		unhappy.tame.ToggleEffect((Instance smi) => unhappyTameEffect).TagTransition(GameTags.Creatures.Wild, unhappy.wild);
		happyWildEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		happyTameEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		happyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 9f, CREATURES.MODIFIERS.HAPPY.NAME, is_multiplier: true));
		unhappyWildEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		unhappyWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.UNHAPPY.NAME));
		unhappyTameEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
		unhappyTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.UNHAPPY.NAME));
	}

	private static bool IsHappy(Instance smi)
	{
		return smi.happiness.GetTotalValue() >= smi.def.threshold;
	}
}
