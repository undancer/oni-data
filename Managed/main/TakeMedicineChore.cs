using System;
using STRINGS;

public class TakeMedicineChore : Chore<TakeMedicineChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TakeMedicineChore, object>.GameInstance
	{
		public StatesInstance(TakeMedicineChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TakeMedicineChore>
	{
		public TargetParameter eater;

		public TargetParameter source;

		public TargetParameter chunk;

		public FloatParameter requestedpillcount;

		public FloatParameter actualpillcount;

		public FetchSubState fetch;

		public State takemedicine;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(eater);
			fetch.InitializeStates(eater, source, chunk, requestedpillcount, actualpillcount, takemedicine);
			takemedicine.ToggleAnims("anim_eat_floor_kanim").ToggleTag(GameTags.TakingMedicine).ToggleWork("TakeMedicine", delegate(StatesInstance smi)
			{
				MedicinalPill workable = chunk.Get<MedicinalPill>(smi);
				eater.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo(workable));
			}, (StatesInstance smi) => chunk.Get<MedicinalPill>(smi) != null, null, null);
		}
	}

	private Pickupable pickupable;

	private MedicinalPill medicine;

	public static readonly Precondition CanCure;

	public static readonly Precondition IsConsumptionPermitted;

	public TakeMedicineChore(MedicinalPill master)
		: base(Db.Get().ChoreTypes.TakeMedicine, (IStateMachineTarget)master, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		medicine = master;
		pickupable = medicine.GetComponent<Pickupable>();
		base.smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.CanPickup, pickupable);
		AddPrecondition(CanCure, this);
		AddPrecondition(IsConsumptionPermitted, this);
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.source.Set(pickupable.gameObject, base.smi);
		base.smi.sm.requestedpillcount.Set(1f, base.smi);
		base.smi.sm.eater.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
		new TakeMedicineChore(medicine);
	}

	static TakeMedicineChore()
	{
		Precondition precondition = new Precondition
		{
			id = "CanCure",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_CURE,
			fn = delegate(ref Precondition.Context context, object data)
			{
				return ((TakeMedicineChore)data).medicine.CanBeTakenBy(context.consumerState.gameObject);
			}
		};
		CanCure = precondition;
		precondition = new Precondition
		{
			id = "IsConsumptionPermitted",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CONSUMPTION_PERMITTED,
			fn = delegate(ref Precondition.Context context, object data)
			{
				TakeMedicineChore takeMedicineChore = (TakeMedicineChore)data;
				ConsumableConsumer consumableConsumer = context.consumerState.consumableConsumer;
				return consumableConsumer == null || consumableConsumer.IsPermitted(takeMedicineChore.medicine.PrefabID().Name);
			}
		};
		IsConsumptionPermitted = precondition;
	}
}
