using Klei.AI;
using STRINGS;

public class BeeHappinessMonitor : GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>
{
	public class Def : BaseDef
	{
		public float threshold;
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

	private State happy;

	private State unhappy;

	private Effect happyEffect;

	private Effect unhappyEffect;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(happy, IsHappy, UpdateRate.SIM_1000ms).Transition(unhappy, GameStateMachine<BeeHappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		happy.ToggleEffect((Instance smi) => happyEffect).TriggerOnEnter(GameHashes.Happy).Transition(satisfied, GameStateMachine<BeeHappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		unhappy.TriggerOnEnter(GameHashes.Unhappy).Transition(satisfied, IsHappy, UpdateRate.SIM_1000ms).ToggleEffect((Instance smi) => unhappyEffect);
		happyEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		unhappyEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
	}

	private static bool IsHappy(Instance smi)
	{
		return smi.happiness.GetTotalValue() >= smi.def.threshold;
	}
}
