public class AnimInterruptStates : GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.PlayInterruptAnim);
		}
	}

	public State play_anim;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = play_anim;
		play_anim.Enter(PlayAnim).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.PlayInterruptAnim);
	}

	private void PlayAnim(Instance smi)
	{
		KBatchedAnimController kBatchedAnimController = smi.Get<KBatchedAnimController>();
		HashedString[] anims = smi.GetSMI<AnimInterruptMonitor.Instance>().anims;
		kBatchedAnimController.Play(anims[0]);
		for (int i = 1; i < anims.Length; i++)
		{
			kBatchedAnimController.Queue(anims[i]);
		}
	}
}
