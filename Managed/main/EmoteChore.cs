using System;
using UnityEngine;

public class EmoteChore : Chore<EmoteChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EmoteChore, object>.GameInstance
	{
		public HashedString[] emoteAnims;

		public HashedString emoteKAnim;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public StatesInstance(EmoteChore master, GameObject emoter, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode mode, bool flip_x)
			: base(master)
		{
			emoteKAnim = emote_kanim;
			emoteAnims = emote_anims;
			this.mode = mode;
			base.sm.emoter.Set(emoter, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EmoteChore>
	{
		public TargetParameter emoter;

		public State finish;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			Target(emoter);
			root.ToggleAnims((StatesInstance smi) => smi.emoteKAnim).PlayAnims((StatesInstance smi) => smi.emoteAnims, (StatesInstance smi) => smi.mode).ScheduleGoTo(10f, finish)
				.OnAnimQueueComplete(finish);
			finish.ReturnSuccess();
		}
	}

	private Func<StatusItem> getStatusItem;

	private SelfEmoteReactable reactable;

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString[] emote_anims, Func<StatusItem> get_status_item = null)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, null, emote_anims, KAnim.PlayMode.Once, flip_x: false);
		getStatusItem = get_status_item;
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, KAnim.PlayMode play_mode, bool flip_x = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, emote_kanim, emote_anims, play_mode, flip_x);
	}

	public EmoteChore(IStateMachineTarget target, ChoreType chore_type, HashedString emote_kanim, HashedString[] emote_anims, Func<StatusItem> get_status_item)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, emote_kanim, emote_anims, KAnim.PlayMode.Once, flip_x: false);
		getStatusItem = get_status_item;
	}

	protected override StatusItem GetStatusItem()
	{
		return (getStatusItem != null) ? getStatusItem() : base.GetStatusItem();
	}

	public override string ToString()
	{
		HashedString emoteKAnim;
		if (base.smi.emoteKAnim.IsValid)
		{
			emoteKAnim = base.smi.emoteKAnim;
			return "EmoteChore<" + emoteKAnim.ToString() + ">";
		}
		emoteKAnim = base.smi.emoteAnims[0];
		return "EmoteChore<" + emoteKAnim.ToString() + ">";
	}

	public void PairReactable(SelfEmoteReactable reactable)
	{
		this.reactable = reactable;
	}

	protected new virtual void End(string reason)
	{
		if (reactable != null)
		{
			reactable.PairEmote(null);
			reactable.Cleanup();
			reactable = null;
		}
		base.End(reason);
	}
}
