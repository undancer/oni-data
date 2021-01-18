using UnityEngine;

public class SuperProductiveFX : GameStateMachine<SuperProductiveFX, SuperProductiveFX.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("productive_fx_kanim", master.gameObject.transform.GetPosition() + offset, master.gameObject.transform, update_looping_sounds_position: true);
			base.sm.fx.Set(kBatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
			base.smi.StopSM("destroyed");
		}
	}

	public Signal wasProductive;

	public Signal destroyFX;

	public TargetParameter fx;

	public State pre;

	public State idle;

	public State productive;

	public State pst;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre;
		Target(fx);
		root.OnSignal(wasProductive, productive, (Instance smi) => smi.GetCurrentState() != smi.sm.pst).OnSignal(destroyFX, pst);
		pre.PlayAnim("productive_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(idle);
		idle.PlayAnim("productive_loop", KAnim.PlayMode.Loop);
		productive.QueueAnim("productive_achievement").OnAnimQueueComplete(idle);
		pst.PlayAnim("productive_pst").EventHandler(GameHashes.AnimQueueComplete, delegate(Instance smi)
		{
			smi.DestroyFX();
		});
	}
}
