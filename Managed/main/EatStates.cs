using STRINGS;
using UnityEngine;

public class EatStates : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Element lastMealElement;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
		}

		public Element GetLatestMealElement()
		{
			return lastMealElement;
		}
	}

	public class EatingState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public ApproachSubState<Pickupable> goingtoeat;

	public EatingState eating;

	public State behaviourcomplete;

	public TargetParameter target;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtoeat;
		root.Enter(SetTarget).Enter(ReserveEdible).Exit(UnreserveEdible);
		goingtoeat.MoveTo(GetEdibleCell, eating).ToggleStatusItem(CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME, CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		eating.DefaultState(eating.pre).ToggleStatusItem(CREATURES.STATUSITEMS.EATING.NAME, CREATURES.STATUSITEMS.EATING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		eating.pre.QueueAnim("eat_pre").OnAnimQueueComplete(eating.loop);
		eating.loop.Enter(EatComplete).QueueAnim("eat_loop", loop: true).ScheduleGoTo(3f, eating.pst);
		eating.pst.QueueAnim("eat_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat);
	}

	private static void SetTarget(Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<SolidConsumerMonitor.Instance>().targetEdible, smi);
	}

	private static void ReserveEdible(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveEdible(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			if (gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
				return;
			}
			Debug.LogWarningFormat(smi.gameObject, "{0} UnreserveEdible but it wasn't reserved: {1}", smi.gameObject, gameObject);
		}
	}

	private static void EatComplete(Instance smi)
	{
		PrimaryElement primaryElement = smi.sm.target.Get<PrimaryElement>(smi);
		if (primaryElement != null)
		{
			smi.lastMealElement = primaryElement.Element;
		}
		smi.Trigger(1386391852, smi.sm.target.Get<KPrefabID>(smi));
	}

	private static int GetEdibleCell(Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}
}
