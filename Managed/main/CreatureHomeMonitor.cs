public class CreatureHomeMonitor : GameStateMachine<CreatureHomeMonitor, CreatureHomeMonitor.Instance, IStateMachineTarget, CreatureHomeMonitor.Def>
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
		root.ToggleBehaviour(GameTags.Creatures.WantsToGoHome, ShouldGoHome).ToggleBehaviour(GameTags.Creatures.WantsToMakeHome, ShouldMakeHome);
	}

	public static bool ShouldGoHome(Instance smi)
	{
		return GameClock.Instance.IsNighttime();
	}

	public static bool ShouldMakeHome(Instance smi)
	{
		return ShouldGoHome(smi) && !CanGoHome(smi);
	}

	public static bool CanGoHome(Instance smi)
	{
		Bee component = smi.gameObject.GetComponent<Bee>();
		KPrefabID x = component.FindHiveInRoom();
		return x != null;
	}
}
