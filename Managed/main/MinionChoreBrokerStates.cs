public class MinionChoreBrokerStates : GameStateMachine<MinionChoreBrokerStates, MinionChoreBrokerStates.Instance, IStateMachineTarget, MinionChoreBrokerStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
		}
	}

	private State hasChore;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = hasChore;
		root.DoNothing();
		hasChore.Enter(delegate
		{
		});
	}
}
