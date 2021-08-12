using STRINGS;
using UnityEngine;

public class IncubatingStates : GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int variant_time = 3;

		public static readonly Chore.Precondition IsInIncubator = new Chore.Precondition
		{
			id = "IsInIncubator",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.InIncubator);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsInIncubator);
		}
	}

	public class IncubatorStates : State
	{
		public State idle;

		public State choose;

		public State variant;
	}

	public IncubatorStates incubator;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = incubator;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.IN_INCUBATOR.NAME, CREATURES.STATUSITEMS.IN_INCUBATOR.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		incubator.DefaultState(incubator.idle).ToggleTag(GameTags.Creatures.Deliverable).TagTransition(GameTags.Creatures.InIncubator, null, on_remove: true);
		incubator.idle.Enter("VariantUpdate", VariantUpdate).PlayAnim("incubator_idle_loop").OnAnimQueueComplete(incubator.choose);
		incubator.choose.Transition(incubator.variant, DoVariant).Transition(incubator.idle, GameStateMachine<IncubatingStates, Instance, IStateMachineTarget, Def>.Not(DoVariant));
		incubator.variant.PlayAnim("incubator_variant").OnAnimQueueComplete(incubator.idle);
	}

	public static bool DoVariant(Instance smi)
	{
		return smi.variant_time == 0;
	}

	public static void VariantUpdate(Instance smi)
	{
		if (smi.variant_time <= 0)
		{
			smi.variant_time = Random.Range(3, 7);
		}
		else
		{
			smi.variant_time--;
		}
	}
}
