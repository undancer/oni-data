using System;
using KSerialization;

public class RoverChoreMonitor : GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public int lastDigCell = -1;

		private Action<object> OnDestinationReachedDelegate;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
		}
	}

	public State loop;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		loop.ToggleBehaviour(GameTags.Creatures.Tunnel, (Instance smi) => true).ToggleBehaviour(GameTags.Creatures.Builder, (Instance smi) => true);
	}
}
