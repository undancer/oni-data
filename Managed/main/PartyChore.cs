using System;
using Klei.AI;
using TUNING;

public class PartyChore : Chore<PartyChore.StatesInstance>, IWorkerPrioritizable
{
	public class States : GameStateMachine<States, StatesInstance, PartyChore>
	{
		public TargetParameter partyer;

		public TargetParameter chitchatlocator;

		public ApproachSubState<IApproachable> stand;

		public ApproachSubState<IApproachable> chat_move;

		public State chat;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = stand;
			Target(partyer);
			stand.InitializeStates(partyer, masterTarget, chat);
			chat_move.InitializeStates(partyer, chitchatlocator, chat);
			chat.ToggleWork<Workable>(chitchatlocator, success, null, null);
			success.Enter(delegate(StatesInstance smi)
			{
				partyer.Get(smi).gameObject.GetComponent<Effects>().Add("RecentlyPartied", should_save: true);
			}).ReturnSuccess();
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, PartyChore, object>.GameInstance
	{
		public StatesInstance(PartyChore master)
			: base(master)
		{
		}
	}

	public int basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;

	public const string specificEffect = "Socialized";

	public const string trackingEffect = "RecentlySocialized";

	public PartyChore(IStateMachineTarget master, Workable chat_workable, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null)
		: base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), run_until_complete: true, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.high, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new StatesInstance(this);
		base.smi.sm.chitchatlocator.Set(chat_workable, base.smi);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, chat_workable);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.partyer.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
		base.smi.sm.partyer.Get(base.smi).gameObject.AddTag(GameTags.Partying);
	}

	protected override void End(string reason)
	{
		if (base.smi.sm.partyer.Get(base.smi) != null)
		{
			base.smi.sm.partyer.Get(base.smi).gameObject.RemoveTag(GameTags.Partying);
		}
		base.End(reason);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		return true;
	}
}
