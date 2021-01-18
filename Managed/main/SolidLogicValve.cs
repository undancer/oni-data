using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidLogicValve : StateMachineComponent<SolidLogicValve.StatesInstance>
{
	public class States : GameStateMachine<States, StatesInstance, SolidLogicValve>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;
		}

		public State off;

		public ReadyStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			});
			on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true);
			});
			on.idle.PlayAnim("on").Transition(on.working, (StatesInstance smi) => smi.IsDispensing());
			on.working.PlayAnim("on_flow", KAnim.PlayMode.Loop).Transition(on.idle, (StatesInstance smi) => !smi.IsDispensing());
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SolidLogicValve, object>.GameInstance
	{
		public StatesInstance(SolidLogicValve master)
			: base(master)
		{
		}

		public bool IsDispensing()
		{
			return base.master.bridge.IsDispensing;
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitBridge bridge;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}
}
