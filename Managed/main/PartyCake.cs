public class PartyCake : GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>
{
	public class Def : BaseDef
	{
	}

	public class CreatingStates : State
	{
		public class Tier : State
		{
			public State fetch;

			public State work;

			public State InitializeStates(TargetParameter targ, string animPrefix, State success)
			{
				base.root.Target(targ).DefaultState(fetch);
				fetch.PlayAnim(animPrefix + "_ready").ToggleChore(CreateFetchChore, work);
				work.Enter(delegate(StatesInstance smi)
				{
					KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
					component.Play(animPrefix + "_working");
					component.SetPositionPercent(0f);
				}).ToggleChore(CreateWorkChore, success, work);
				return this;
			}
		}

		public State ready;

		public Tier tier1;

		public Tier tier2;

		public Tier tier3;

		public State finish;
	}

	public class StatesInstance : GameInstance
	{
		public StatesInstance(IStateMachineTarget smi, Def def)
			: base(smi, def)
		{
		}
	}

	public CreatingStates creating;

	public State ready_to_party;

	public State party;

	public State complete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = creating.ready;
		creating.ready.PlayAnim("base").GoTo(creating.tier1);
		creating.tier1.InitializeStates(masterTarget, "tier_1", creating.tier2);
		creating.tier2.InitializeStates(masterTarget, "tier_2", creating.tier3);
		creating.tier3.InitializeStates(masterTarget, "tier_3", ready_to_party);
		ready_to_party.PlayAnim("unlit");
		party.PlayAnim("lit");
		complete.PlayAnim("finished");
	}

	private static Chore CreateFetchChore(StatesInstance smi)
	{
		return new FetchChore(Db.Get().ChoreTypes.FarmFetch, smi.GetComponent<Storage>(), 10f, new Tag[1]
		{
			"MushBar".ToTag()
		}, null, null, null, run_until_complete: true, null, null, null, FetchOrder2.OperationalRequirement.Functional);
	}

	private static Chore CreateWorkChore(StatesInstance smi)
	{
		Workable component = smi.master.GetComponent<PartyCakeWorkable>();
		return new WorkChore<PartyCakeWorkable>(Db.Get().ChoreTypes.Cook, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Work, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
	}
}
