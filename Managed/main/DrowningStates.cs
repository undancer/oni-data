using STRINGS;

public class DrowningStates : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int safeCell = Grid.InvalidCell;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Drowning);
		}
	}

	public class EscapeCellQuery : PathFinderQuery
	{
		private DrowningMonitor monitor;

		public EscapeCellQuery(DrowningMonitor monitor)
		{
			this.monitor = monitor;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			return monitor.IsCellSafe(cell);
		}
	}

	public State drown;

	public State drown_pst;

	public State move_to_safe;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = drown;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.DROWNING.NAME, CREATURES.STATUSITEMS.DROWNING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Drowning, null, on_remove: true);
		drown.PlayAnim("drown_pre").QueueAnim("drown_loop", loop: true).Transition(drown_pst, UpdateSafeCell, UpdateRate.SIM_1000ms);
		drown_pst.PlayAnim("drown_pst").OnAnimQueueComplete(move_to_safe);
		move_to_safe.MoveTo((Instance smi) => smi.safeCell);
	}

	public bool UpdateSafeCell(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		DrowningMonitor component2 = smi.GetComponent<DrowningMonitor>();
		EscapeCellQuery escapeCellQuery = new EscapeCellQuery(component2);
		component.RunQuery(escapeCellQuery);
		smi.safeCell = escapeCellQuery.GetResultCell();
		return smi.safeCell != Grid.InvalidCell;
	}
}
