using STRINGS;

public class GrowUpStates : GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.GrowUpBehaviour);
		}
	}

	public State grow_up_pre;

	public State spawn_adult;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grow_up_pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.GROWINGUP.NAME, CREATURES.STATUSITEMS.GROWINGUP.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		grow_up_pre.QueueAnim("growup_pre").OnAnimQueueComplete(spawn_adult);
		spawn_adult.Enter(SpawnAdult);
	}

	private static void SpawnAdult(Instance smi)
	{
		smi.GetSMI<BabyMonitor.Instance>().SpawnAdult();
	}
}
