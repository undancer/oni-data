public class HiveGrowthMonitor : GameStateMachine<HiveGrowthMonitor, HiveGrowthMonitor.Instance, IStateMachineTarget, HiveGrowthMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.Behaviours.GrowUpBehaviour, IsGrowing);
	}

	public static bool IsGrowing(Instance smi)
	{
		return !smi.GetSMI<BeeHive.StatesInstance>().IsFullyGrown();
	}
}
