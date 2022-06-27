public class AnimInterruptMonitor : GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public HashedString[] anims;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void PlayAnim(HashedString anim)
		{
			PlayAnimSequence(new HashedString[1] { anim });
		}

		public void PlayAnimSequence(HashedString[] anims)
		{
			this.anims = anims;
			GetComponent<CreatureBrain>().UpdateBrain();
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.Behaviours.PlayInterruptAnim, ShoulPlayAnim, ClearAnim);
	}

	private static bool ShoulPlayAnim(Instance smi)
	{
		return smi.anims != null;
	}

	private static void ClearAnim(Instance smi)
	{
		smi.anims = null;
	}
}
