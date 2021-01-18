using STRINGS;

public class StunnedStates : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public static readonly Chore.Precondition IsStunned = new Chore.Precondition
		{
			id = "IsStunned",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Stunned);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsStunned);
		}
	}

	public State stunned;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = stunned;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		stunned.PlayAnim("idle_loop", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.Stunned, null, on_remove: true);
	}
}
