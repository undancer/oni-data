public class DiggerStates : GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Tunnel);
		}

		public int GetTunnelCell()
		{
			return base.smi.GetSMI<DiggerMonitor.Instance>()?.lastDigCell ?? (-1);
		}
	}

	public State move;

	public State hide;

	public State behaviourcomplete;

	private static bool ShouldStopHiding(Instance smi)
	{
		return !GameplayEventManager.Instance.IsGameplayEventRunningWithTag(GameTags.SpaceDanger);
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		move.MoveTo((Instance smi) => smi.GetTunnelCell(), hide, behaviourcomplete);
		hide.Transition(behaviourcomplete, ShouldStopHiding, UpdateRate.SIM_4000ms);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Tunnel);
	}
}
