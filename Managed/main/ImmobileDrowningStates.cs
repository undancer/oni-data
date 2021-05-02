using STRINGS;

public class ImmobileDrowningStates : GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Drowning);
		}
	}

	public State drown;

	public State drown_pst;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = drown;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.DROWNING.NAME, CREATURES.STATUSITEMS.DROWNING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Drowning, drown_pst, on_remove: true);
		drown.PlayAnim("drown_pre").QueueAnim("drown_loop", loop: true);
		drown_pst.PlayAnim("drown_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Drowning);
	}
}
