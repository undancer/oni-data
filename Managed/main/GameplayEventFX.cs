using UnityEngine;

public class GameplayEventFX : GameStateMachine<GameplayEventFX, GameplayEventFX.Instance>
{
	public new class Instance : GameInstance
	{
		public int previousCount;

		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("event_alert_fx_kanim", base.smi.master.transform.GetPosition() + offset, base.smi.master.transform);
			base.sm.fx.Set(kBatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}
	}

	public TargetParameter fx;

	public IntParameter notificationCount;

	public State single;

	public State multiple;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		Target(fx);
		root.PlayAnim("event_pre").OnAnimQueueComplete(single).Exit("DestroyFX", delegate(Instance smi)
		{
			smi.DestroyFX();
		});
		single.PlayAnim("event_loop", KAnim.PlayMode.Loop).ParamTransition(notificationCount, multiple, (Instance smi, int p) => p > 1);
		multiple.PlayAnim("event_loop_multiple", KAnim.PlayMode.Loop).ParamTransition(notificationCount, single, (Instance smi, int p) => p == 1);
	}
}
