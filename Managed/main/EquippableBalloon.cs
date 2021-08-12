using KSerialization;
using TUNING;

public class EquippableBalloon : StateMachineComponent<EquippableBalloon.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EquippableBalloon, object>.GameInstance
	{
		[Serialize]
		public float transitionTime;

		public StatesInstance(EquippableBalloon master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EquippableBalloon>
	{
		public State destroy;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.Transition(destroy, (StatesInstance smi) => GameClock.Instance.GetTime() >= smi.transitionTime);
			destroy.Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<Equippable>().Unassign();
			});
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.transitionTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.JOY_REACTION_DURATION;
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}
}
