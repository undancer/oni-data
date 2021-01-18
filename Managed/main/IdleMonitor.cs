public class IdleMonitor : GameStateMachine<IdleMonitor, IdleMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public State idle;

	public State stopped;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.TagTransition(GameTags.Dying, stopped).ToggleRecurringChore(CreateIdleChore);
		stopped.DoNothing();
	}

	private Chore CreateIdleChore(Instance smi)
	{
		return new IdleChore(smi.master);
	}
}
