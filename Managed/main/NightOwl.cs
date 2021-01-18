using Klei.AI;
using STRINGS;
using TUNING;

[SkipSaveFileSerialization]
public class NightOwl : StateMachineComponent<NightOwl.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, NightOwl, object>.GameInstance
	{
		public StatesInstance(NightOwl master)
			: base(master)
		{
		}

		public bool IsNight()
		{
			if (GameClock.Instance == null || base.master.kPrefabID.PrefabTag == GameTags.MinionSelectPreview)
			{
				return false;
			}
			return GameClock.Instance.IsNighttime();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, NightOwl>
	{
		public State idle;

		public State early;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, null);
			idle.Transition(early, (StatesInstance smi) => smi.IsNight());
			early.Enter("Night", delegate(StatesInstance smi)
			{
				smi.master.ApplyModifiers();
			}).Exit("NotNight", delegate(StatesInstance smi)
			{
				smi.master.RemoveModifiers();
			}).ToggleStatusItem(Db.Get().DuplicantStatusItems.NightTime)
				.ToggleExpression(Db.Get().Expressions.Happy)
				.Transition(idle, (StatesInstance smi) => !smi.IsNight());
		}
	}

	[MyCmpReq]
	private KPrefabID kPrefabID;

	private AttributeModifier[] attributeModifiers;

	protected override void OnSpawn()
	{
		attributeModifiers = new AttributeModifier[11]
		{
			new AttributeModifier("Construction", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Digging", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Machinery", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Athletics", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Learning", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Cooking", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Art", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Strength", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Caring", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Botanist", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME),
			new AttributeModifier("Ranching", TRAITS.NIGHTOWL_MODIFIER, DUPLICANTS.TRAITS.NIGHTOWL.NAME)
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
