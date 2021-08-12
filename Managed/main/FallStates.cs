using System;
using STRINGS;

public class FallStates : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
{
	public class Def : BaseDef
	{
		public Func<Instance, string> getLandAnim = (Instance smi) => "idle_loop";
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Falling);
		}
	}

	private State loop;

	private State snaptoground;

	private State pst;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.FALLING.NAME, CREATURES.STATUSITEMS.FALLING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		loop.PlayAnim((Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().anim, KAnim.PlayMode.Loop).ToggleGravity().EventTransition(GameHashes.Landed, snaptoground)
			.Transition(pst, (Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation(check_head: true), UpdateRate.SIM_33ms);
		snaptoground.Enter(delegate(Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().SnapToGround();
		}).GoTo(pst);
		pst.Enter(PlayLandAnim).BehaviourComplete(GameTags.Creatures.Falling);
	}

	private static void PlayLandAnim(Instance smi)
	{
		smi.GetComponent<KBatchedAnimController>().Queue(smi.def.getLandAnim(smi), KAnim.PlayMode.Loop);
	}
}
