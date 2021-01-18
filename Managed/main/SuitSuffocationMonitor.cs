using Klei.AI;
using STRINGS;

public class SuitSuffocationMonitor : GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance>
{
	public class NoOxygenState : State
	{
		public State holdingbreath;

		public State suffocating;
	}

	public class SatisfiedState : State
	{
		public State normal;

		public State low;
	}

	public new class Instance : GameInstance
	{
		private AmountInstance breath;

		public AttributeModifier breathing;

		public AttributeModifier holdingbreath;

		private OxygenBreather masterOxygenBreather;

		public SuitTank suitTank
		{
			get;
			private set;
		}

		public Instance(IStateMachineTarget master, SuitTank suit_tank)
			: base(master)
		{
			breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 0.90909094f;
			breathing = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.BREATHING.NAME);
			holdingbreath = new AttributeModifier(deltaAttribute.Id, 0f - num, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME);
			suitTank = suit_tank;
		}

		public bool IsTankEmpty()
		{
			return suitTank.IsEmpty();
		}

		public bool HasSuffocated()
		{
			return breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return breath.value <= 45.454548f;
		}

		public void Kill()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
		}
	}

	public SatisfiedState satisfied;

	public NoOxygenState nooxygen;

	public State death;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.DefaultState(satisfied.normal).ToggleAttributeModifier("Breathing", (Instance smi) => smi.breathing).Transition(nooxygen, (Instance smi) => smi.IsTankEmpty());
		satisfied.normal.Transition(satisfied.low, (Instance smi) => smi.suitTank.NeedsRecharging());
		satisfied.low.DoNothing();
		nooxygen.ToggleExpression(Db.Get().Expressions.Suffocate).ToggleAttributeModifier("Holding Breath", (Instance smi) => smi.holdingbreath).ToggleTag(GameTags.NoOxygen)
			.DefaultState(nooxygen.holdingbreath);
		nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath).Transition(nooxygen.suffocating, (Instance smi) => smi.IsSuffocating());
		nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating).Transition(death, (Instance smi) => smi.HasSuffocated());
		death.Enter("SuffocationDeath", delegate(Instance smi)
		{
			smi.Kill();
		});
	}
}
