public class ClusterMapFXAnimator : GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer>
{
	public class StatesInstance : GameInstance
	{
		public StatesInstance(ClusterMapVisualizer visualizer, ClusterGridEntity entity)
			: base(visualizer)
		{
			base.sm.entityTarget.Set(entity, this);
			visualizer.GetFirstAnimController().gameObject.Subscribe(-1061186183, OnAnimQueueComplete);
		}

		private void OnAnimQueueComplete(object data)
		{
			base.sm.onAnimComplete.Trigger(this);
		}

		public void DestroyEntity()
		{
			base.sm.entityTarget.Get(this).DeleteObject();
		}
	}

	private KBatchedAnimController animController;

	public TargetParameter entityTarget;

	public State play;

	public State finished;

	public Signal onAnimComplete;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = play;
		play.OnSignal(onAnimComplete, finished);
		finished.Enter(delegate(StatesInstance smi)
		{
			smi.DestroyEntity();
		});
	}
}
