using STRINGS;

public class CreatureHomeStates : GameStateMachine<CreatureHomeStates, CreatureHomeStates.Instance, IStateMachineTarget, CreatureHomeStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Bee bee;

		private KPrefabID hive;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			bee = base.smi.master.GetComponent<Bee>();
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGoHome);
		}

		public int GetHomeCell()
		{
			int result = 0;
			hive = base.smi.gameObject.GetComponent<Bee>().FindHiveInRoom();
			if (hive != null)
			{
				result = Grid.PosToCell(hive.transform.GetPosition());
			}
			return result;
		}

		public void GoHomeComplete()
		{
			if (hive != null)
			{
				BeeHive.StatesInstance sMI = hive.GetSMI<BeeHive.StatesInstance>();
				sMI.master.EnterHive(base.smi.master.gameObject);
			}
			hive = null;
		}
	}

	public State moveToHome;

	public State arriveHome;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moveToHome;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.TENDINGANEGG.NAME, CREATURES.STATUSITEMS.TENDINGANEGG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		moveToHome.MoveTo((Instance smi) => smi.GetHomeCell(), arriveHome, behaviourcomplete);
		arriveHome.PlayAnim("tending_egg_pre", KAnim.PlayMode.Once).QueueAnim("tending_egg_loop").QueueAnim("tending_egg_pst")
			.Exit(delegate(Instance smi)
			{
				smi.GoHomeComplete();
			})
			.OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGoHome);
	}
}
