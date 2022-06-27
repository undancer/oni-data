using STRINGS;

public class CreatureDiseaseCleaner : GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>
{
	public class Def : BaseDef
	{
		public float cleanDuration;

		public Def(float duration)
		{
			cleanDuration = duration;
		}
	}

	public class CleaningStates : State
	{
		public State clean_pre;

		public State clean;

		public State clean_pst;
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Cleaning);
		}

		public void EnableDiseaseEmitter(bool enable = true)
		{
			DiseaseEmitter component = GetComponent<DiseaseEmitter>();
			if (component != null)
			{
				component.SetEnable(enable);
			}
		}
	}

	public State behaviourcomplete;

	public CleaningStates cleaning;

	public Signal cellChangedSignal;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = cleaning;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.CLEANING.NAME, CREATURES.STATUSITEMS.CLEANING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		cleaning.DefaultState(cleaning.clean_pre).ScheduleGoTo((Instance smi) => smi.def.cleanDuration, cleaning.clean_pst);
		cleaning.clean_pre.PlayAnim("clean_water_pre").OnAnimQueueComplete(cleaning.clean);
		cleaning.clean.Enter(delegate(Instance smi)
		{
			smi.EnableDiseaseEmitter();
		}).QueueAnim("clean_water_loop", loop: true).Transition(cleaning.clean_pst, (Instance smi) => !smi.GetSMI<CleaningMonitor.Instance>().CanCleanElementState(), UpdateRate.SIM_1000ms)
			.Exit(delegate(Instance smi)
			{
				smi.EnableDiseaseEmitter(enable: false);
			});
		cleaning.clean_pst.PlayAnim("clean_water_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Cleaning);
	}
}
