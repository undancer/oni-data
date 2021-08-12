public class RobotIdleMonitor : GameStateMachine<RobotIdleMonitor, RobotIdleMonitor.Instance>
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

	public State working;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Transition(working, (Instance smi) => !CheckShouldIdle(smi));
		working.Transition(idle, (Instance smi) => CheckShouldIdle(smi));
	}

	private static bool CheckShouldIdle(Instance smi)
	{
		FallMonitor.Instance sMI = smi.master.gameObject.GetSMI<FallMonitor.Instance>();
		if (sMI == null)
		{
			return true;
		}
		if (!smi.master.gameObject.GetComponent<ChoreConsumer>().choreDriver.HasChore())
		{
			return sMI.GetCurrentState() == sMI.sm.standing;
		}
		return false;
	}
}
