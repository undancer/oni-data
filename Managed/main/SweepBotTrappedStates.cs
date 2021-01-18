public class SweepBotTrappedStates : GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Robots.Behaviours.TrappedBehaviour);
		}
	}

	public class BlockedStates : State
	{
		public State evaluating;

		public State blocked;

		public State noHome;
	}

	public BlockedStates blockedStates;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = blockedStates.evaluating;
		blockedStates.ToggleStatusItem(Db.Get().RobotStatusItems.CantReachStation, null, Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Robots.Behaviours.TrappedBehaviour, behaviourcomplete, on_remove: true);
		blockedStates.evaluating.Enter(delegate(Instance smi)
		{
			if (smi.sm.GetSweepLocker(smi) == null)
			{
				smi.GoTo(blockedStates.noHome);
			}
			else
			{
				smi.GoTo(blockedStates.blocked);
			}
		});
		blockedStates.blocked.ToggleChore((Instance smi) => new RescueSweepBotChore(smi.master, smi.master.gameObject, smi.sm.GetSweepLocker(smi).gameObject), behaviourcomplete, blockedStates.evaluating).PlayAnim("react_stuck", KAnim.PlayMode.Loop);
		blockedStates.noHome.PlayAnim("react_stuck", KAnim.PlayMode.Once).OnAnimQueueComplete(blockedStates.evaluating);
		behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.TrappedBehaviour);
	}

	public Storage GetSweepLocker(Instance smi)
	{
		StorageUnloadMonitor.Instance sMI = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
		return sMI?.sm.sweepLocker.Get(sMI);
	}
}
