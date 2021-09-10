public class RocketSelfDestructMonitor : GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public KBatchedAnimController eyes;

		public Instance(IStateMachineTarget master, Def def)
			: base(master)
		{
		}
	}

	public State idle;

	public State exploding;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventTransition(GameHashes.RocketSelfDestructRequested, exploding);
		exploding.Update(delegate(Instance smi, float dt)
		{
			if (smi.timeinstate >= 3f)
			{
				smi.master.Trigger(-1311384361);
				smi.GoTo(idle);
			}
		});
	}
}
