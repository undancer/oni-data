public class MingleMonitor : GameStateMachine<MingleMonitor, MingleMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public State mingle;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = mingle;
		base.serializable = SerializeType.Never;
		mingle.ToggleRecurringChore(CreateMingleChore);
	}

	private Chore CreateMingleChore(Instance smi)
	{
		return new MingleChore(smi.master);
	}
}
