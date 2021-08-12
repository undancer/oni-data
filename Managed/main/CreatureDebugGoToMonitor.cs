public class CreatureDebugGoToMonitor : GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetCell = Grid.InvalidCell;

		public Instance(IStateMachineTarget target, Def def)
			: base(target, def)
		{
		}

		public void GoToCursor()
		{
			targetCell = DebugHandler.GetMouseCell();
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.HasDebugDestination, HasTargetCell, ClearTargetCell);
	}

	private static bool HasTargetCell(Instance smi)
	{
		return smi.targetCell != Grid.InvalidCell;
	}

	private static void ClearTargetCell(Instance smi)
	{
		smi.targetCell = Grid.InvalidCell;
	}
}
