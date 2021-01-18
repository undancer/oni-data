using Klei.AI;
using STRINGS;
using TUNING;

[SkipSaveFileSerialization]
public class EarlyBird : StateMachineComponent<EarlyBird.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EarlyBird, object>.GameInstance
	{
		public StatesInstance(EarlyBird master)
			: base(master)
		{
		}

		public bool IsMorning()
		{
			if (ScheduleManager.Instance == null || base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview)
			{
				return false;
			}
			int blockIdx = global::Schedule.GetBlockIdx();
			return blockIdx < TRAITS.EARLYBIRD_SCHEDULEBLOCK;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EarlyBird>
	{
		public State idle;

		public State early;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, null);
			idle.Transition(early, (StatesInstance smi) => smi.IsMorning());
			early.Enter("Morning", delegate(StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotMorning", delegate(StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.EarlyMorning)
				.ToggleExpression(Db.Get().Expressions.Happy)
				.Transition(idle, (StatesInstance smi) => !smi.IsMorning());
		}
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	protected override void OnSpawn()
	{
		attributeModifiers = new AttributeModifier[11]
		{
			new AttributeModifier("Construction", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Digging", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Machinery", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Athletics", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Learning", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Cooking", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Art", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Strength", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Caring", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Botanist", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME),
			new AttributeModifier("Ranching", TRAITS.EARLYBIRD_MODIFIER, DUPLICANTS.TRAITS.EARLYBIRD.NAME)
		};
		base.smi.StartSM();
	}

	public void ApplyModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < attributeModifiers.Length; i++)
		{
			AttributeModifier modifier = attributeModifiers[i];
			attributes.Add(modifier);
		}
	}

	public void RemoveModifiers()
	{
		Attributes attributes = base.gameObject.GetAttributes();
		for (int i = 0; i < attributeModifiers.Length; i++)
		{
			AttributeModifier modifier = attributeModifiers[i];
			attributes.Remove(modifier);
		}
	}
}
