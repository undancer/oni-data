public class HiveEatingMonitor : GameStateMachine<HiveEatingMonitor, HiveEatingMonitor.Instance, IStateMachineTarget, HiveEatingMonitor.Def>
{
	public class Def : BaseDef
	{
		public Tag consumedOre;
	}

	public new class Instance : GameInstance
	{
		[MyCmpReq]
		public Storage storage;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.WantsToEat, ShouldEat);
	}

	public static bool ShouldEat(Instance smi)
	{
		return smi.storage.FindFirst(smi.def.consumedOre) != null;
	}
}
