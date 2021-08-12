using UnityEngine;

public class HotTubWorkerStateMachine : GameStateMachine<HotTubWorkerStateMachine, HotTubWorkerStateMachine.StatesInstance, Worker>
{
	public class StatesInstance : GameInstance
	{
		public StatesInstance(Worker master)
			: base(master)
		{
			base.sm.worker.Set(master, base.smi);
		}
	}

	private State pre_front;

	private State pre_back;

	private State loop;

	private State loop_reenter;

	private State pst_back;

	private State pst_front;

	private State complete;

	public TargetParameter worker;

	public static string[] workAnimLoopVariants = new string[3] { "working_loop1", "working_loop2", "working_loop3" };

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre_front;
		Target(worker);
		root.ToggleAnims("anim_interacts_hottub_kanim");
		pre_front.PlayAnim("working_pre_front").OnAnimQueueComplete(pre_back);
		pre_back.PlayAnim("working_pre_back").Enter(delegate(StatesInstance smi)
		{
			Vector3 position2 = smi.transform.GetPosition();
			position2.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			smi.transform.SetPosition(position2);
		}).OnAnimQueueComplete(loop);
		loop.PlayAnim((StatesInstance smi) => workAnimLoopVariants[Random.Range(0, workAnimLoopVariants.Length)]).OnAnimQueueComplete(loop_reenter).EventTransition(GameHashes.WorkerPlayPostAnim, pst_back, (StatesInstance smi) => smi.GetComponent<Worker>().state == Worker.State.PendingCompletion);
		loop_reenter.GoTo(loop).EventTransition(GameHashes.WorkerPlayPostAnim, pst_back, (StatesInstance smi) => smi.GetComponent<Worker>().state == Worker.State.PendingCompletion);
		pst_back.PlayAnim("working_pst_back").OnAnimQueueComplete(pst_front);
		pst_front.PlayAnim("working_pst_front").Enter(delegate(StatesInstance smi)
		{
			Vector3 position = smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			smi.transform.SetPosition(position);
		}).OnAnimQueueComplete(complete);
	}
}
