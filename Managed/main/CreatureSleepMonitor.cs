public class CreatureSleepMonitor : GameStateMachine<CreatureSleepMonitor, CreatureSleepMonitor.Instance, IStateMachineTarget, CreatureSleepMonitor.Def>
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
		root.ToggleBehaviour(GameTags.Creatures.Behaviours.SleepBehaviour, ShouldSleep);
	}

	public static bool ShouldSleep(Instance smi)
	{
		return GameClock.Instance.IsNighttime();
	}
}
