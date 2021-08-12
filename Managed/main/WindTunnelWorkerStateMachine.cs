using UnityEngine;

public class WindTunnelWorkerStateMachine : GameStateMachine<WindTunnelWorkerStateMachine, WindTunnelWorkerStateMachine.StatesInstance, Worker>
{
	public class StatesInstance : GameInstance
	{
		private VerticalWindTunnelWorkable workable;

		public HashedString OverrideAnim => workable.overrideAnim;

		public string PreFrontAnim => workable.preAnims[0];

		public string PreBackAnim => workable.preAnims[1];

		public string LoopAnim => workable.loopAnim;

		public string PstBackAnim => workable.pstAnims[0];

		public string PstFrontAnim => workable.pstAnims[1];

		public StatesInstance(Worker master, VerticalWindTunnelWorkable workable)
			: base(master)
		{
			this.workable = workable;
			base.sm.worker.Set(master, base.smi);
		}
	}

	private State pre_front;

	private State pre_back;

	private State loop;

	private State pst_back;

	private State pst_front;

	private State complete;

	public TargetParameter worker;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre_front;
		Target(worker);
		root.ToggleAnims((StatesInstance smi) => smi.OverrideAnim);
		pre_front.PlayAnim((StatesInstance smi) => smi.PreFrontAnim).OnAnimQueueComplete(pre_back);
		pre_back.PlayAnim((StatesInstance smi) => smi.PreBackAnim).Enter(delegate(StatesInstance smi)
		{
			Vector3 position2 = smi.transform.GetPosition();
			position2.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			smi.transform.SetPosition(position2);
		}).OnAnimQueueComplete(loop);
		loop.PlayAnim((StatesInstance smi) => smi.LoopAnim, KAnim.PlayMode.Loop).EventTransition(GameHashes.WorkerPlayPostAnim, pst_back, (StatesInstance smi) => smi.GetComponent<Worker>().state == Worker.State.PendingCompletion);
		pst_back.PlayAnim((StatesInstance smi) => smi.PstBackAnim).OnAnimQueueComplete(pst_front);
		pst_front.PlayAnim((StatesInstance smi) => smi.PstFrontAnim).Enter(delegate(StatesInstance smi)
		{
			Vector3 position = smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			smi.transform.SetPosition(position);
		}).OnAnimQueueComplete(complete);
	}
}
