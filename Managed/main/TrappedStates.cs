using STRINGS;

public class TrappedStates : GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public static readonly Chore.Precondition IsTrapped = new Chore.Precondition
		{
			id = "IsTrapped",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Trapped);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsTrapped);
		}
	}

	private State trapped;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = trapped;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.TRAPPED.NAME, CREATURES.STATUSITEMS.TRAPPED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		trapped.Enter(delegate(Instance smi)
		{
			Navigator component = smi.GetComponent<Navigator>();
			if (component.IsValidNavType(NavType.Floor))
			{
				component.SetCurrentNavType(NavType.Floor);
			}
		}).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trapped", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Trapped, null, on_remove: true);
	}
}
