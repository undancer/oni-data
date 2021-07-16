public class FishOvercrowdingMonitor : GameStateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int cellCount;

		public int fishCount;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void SetOvercrowdingInfo(int cell_count, int fish_count)
		{
			cellCount = cell_count;
			fishCount = fish_count;
		}
	}

	public State satisfied;

	public State overcrowded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Enter(Register).Exit(Unregister);
		satisfied.DoNothing();
		overcrowded.DoNothing();
	}

	private static void Register(Instance smi)
	{
		FishOvercrowingManager.Instance.Add(smi);
	}

	private static void Unregister(Instance smi)
	{
		FishOvercrowingManager instance = FishOvercrowingManager.Instance;
		if (!(instance == null))
		{
			instance.Remove(smi);
		}
	}
}
