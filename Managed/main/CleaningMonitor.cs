public class CleaningMonitor : GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>
{
	public class Def : BaseDef
	{
		public Element.State elementState = Element.State.Liquid;

		public CellOffset[] cellOffsets;

		public float coolDown = 30f;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool CanCleanElementState()
		{
			int num = Grid.PosToCell(base.smi.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (!Grid.IsLiquid(num) && base.smi.def.elementState == Element.State.Liquid)
			{
				return false;
			}
			if (Grid.DiseaseCount[num] > 0)
			{
				return true;
			}
			if (base.smi.def.cellOffsets != null)
			{
				CellOffset[] cellOffsets = base.smi.def.cellOffsets;
				foreach (CellOffset offset in cellOffsets)
				{
					int num2 = Grid.OffsetCell(num, offset);
					if (Grid.IsValidCell(num2) && Grid.DiseaseCount[num2] > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public State cooldown;

	public State clean;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = clean;
		clean.ToggleBehaviour(GameTags.Creatures.Cleaning, (Instance smi) => smi.CanCleanElementState(), delegate(Instance smi)
		{
			smi.GoTo(cooldown);
		});
		cooldown.ScheduleGoTo((Instance smi) => smi.def.coolDown, clean);
	}
}
