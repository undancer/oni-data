using STRINGS;

public class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.HasDebugDestination);
		}
	}

	public State moving;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		moving.MoveTo(GetTargetCell, behaviourcomplete, behaviourcomplete, update_cell: true).ToggleStatusItem(CREATURES.STATUSITEMS.DEBUGGOTO.NAME, CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		behaviourcomplete.BehaviourComplete(GameTags.HasDebugDestination);
	}

	private static int GetTargetCell(Instance smi)
	{
		return smi.GetSMI<CreatureDebugGoToMonitor.Instance>().targetCell;
	}
}
