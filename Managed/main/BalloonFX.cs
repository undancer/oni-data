using UnityEngine;

public class BalloonFX : GameStateMachine<BalloonFX, BalloonFX.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("balloon_anim_kanim", master.gameObject.transform.GetPosition() + new Vector3(0f, 0.3f, 1f), master.transform, update_looping_sounds_position: true, Grid.SceneLayer.Creatures);
			base.sm.fx.Set(kBatchedAnimController.gameObject, base.smi);
			kBatchedAnimController.GetComponent<KBatchedAnimController>().defaultAnim = "idle_default";
			master.GetComponent<KBatchedAnimController>().GetSynchronizer().Add(kBatchedAnimController.GetComponent<KBatchedAnimController>());
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}
	}

	public TargetParameter fx;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		Target(fx);
		root.Exit("DestroyFX", delegate(Instance smi)
		{
			smi.DestroyFX();
		});
	}
}
