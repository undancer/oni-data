using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitDropper : StateMachineComponent<SolidConduitDropper.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, SolidConduitDropper, object>.GameInstance
	{
		public SMInstance(SolidConduitDropper master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, SolidConduitDropper>
	{
		public BoolParameter consuming;

		public BoolParameter isclosed;

		public State idle;

		public State working;

		public State post;

		public State closed;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.Update("Update", delegate(SMInstance smi, float dt)
			{
				smi.master.Update();
			}, UpdateRate.SIM_1000ms);
			idle.PlayAnim("on").ParamTransition(consuming, working, GameStateMachine<States, SMInstance, SolidConduitDropper, object>.IsTrue).ParamTransition(isclosed, closed, GameStateMachine<States, SMInstance, SolidConduitDropper, object>.IsTrue);
			working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).ParamTransition(consuming, post, GameStateMachine<States, SMInstance, SolidConduitDropper, object>.IsFalse);
			post.PlayAnim("working_pst").OnAnimQueueComplete(idle);
			closed.PlayAnim("closed").ParamTransition(consuming, working, GameStateMachine<States, SMInstance, SolidConduitDropper, object>.IsTrue).ParamTransition(isclosed, idle, GameStateMachine<States, SMInstance, SolidConduitDropper, object>.IsFalse);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitConsumer consumer;

	[MyCmpAdd]
	private Storage storage;

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

	private void Update()
	{
		base.smi.sm.consuming.Set(consumer.IsConsuming, base.smi);
		base.smi.sm.isclosed.Set(!operational.IsOperational, base.smi);
		storage.DropAll();
	}
}
