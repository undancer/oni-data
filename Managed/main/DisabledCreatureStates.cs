using STRINGS;

public class DisabledCreatureStates : GameStateMachine<DisabledCreatureStates, DisabledCreatureStates.Instance, IStateMachineTarget, DisabledCreatureStates.Def>
{
	public class Def : BaseDef
	{
		public string disabledAnim = "off";

		public Def(string anim)
		{
			disabledAnim = anim;
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Behaviours.DisableCreature);
		}
	}

	public State disableCreature;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = disableCreature;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.DISABLED.NAME, CREATURES.STATUSITEMS.DISABLED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Behaviours.DisableCreature, behaviourcomplete, on_remove: true);
		disableCreature.PlayAnim((Instance smi) => smi.def.disabledAnim);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.DisableCreature);
	}
}
