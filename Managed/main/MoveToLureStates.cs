using STRINGS;
using UnityEngine;

public class MoveToLureStates : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.MoveToLure);
		}
	}

	public State move;

	public State arrive_at_lure;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME, CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		move.MoveTo(GetLureCell, GetLureOffsets, arrive_at_lure, behaviourcomplete);
		arrive_at_lure.Enter(delegate(Instance smi)
		{
			Lure.Instance targetLure = GetTargetLure(smi);
			if (targetLure?.HasTag(GameTags.OneTimeUseLure) ?? false)
			{
				targetLure.GetComponent<KPrefabID>().AddTag(GameTags.LureUsed);
			}
		}).GoTo(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.MoveToLure);
	}

	private static Lure.Instance GetTargetLure(Instance smi)
	{
		LureableMonitor.Instance sMI = smi.GetSMI<LureableMonitor.Instance>();
		GameObject targetLure = sMI.GetTargetLure();
		if (targetLure == null)
		{
			return null;
		}
		return targetLure.GetSMI<Lure.Instance>();
	}

	private static int GetLureCell(Instance smi)
	{
		Lure.Instance targetLure = GetTargetLure(smi);
		if (targetLure == null)
		{
			return Grid.InvalidCell;
		}
		return Grid.PosToCell(targetLure);
	}

	private static CellOffset[] GetLureOffsets(Instance smi)
	{
		return GetTargetLure(smi)?.def.lurePoints;
	}
}
