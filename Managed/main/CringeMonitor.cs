public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private StatusItem statusItem;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetCringeSourceData(object data)
		{
			string name = (string)data;
			statusItem = new StatusItem("CringeSource", name, null, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "Cringe", Db.Get().ChoreTypes.EmoteHighPriority, "anim_cringe_kanim", 0f, 0f).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "cringe_pre"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "cringe_loop"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "cringe_pst"
			});
			emoteReactable.preventChoreInterruption = true;
			return emoteReactable;
		}

		public StatusItem GetStatusItem()
		{
			return statusItem;
		}
	}

	private static readonly HashedString[] CringeAnims = new HashedString[3] { "cringe_pre", "cringe_loop", "cringe_pst" };

	public State idle;

	public State cringe;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventHandler(GameHashes.Cringe, TriggerCringe);
		cringe.ToggleReactable((Instance smi) => smi.GetReactable()).ToggleStatusItem((Instance smi) => smi.GetStatusItem()).ScheduleGoTo(3f, idle);
	}

	private void TriggerCringe(Instance smi, object data)
	{
		if (!smi.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
		{
			smi.SetCringeSourceData(data);
			smi.GoTo(cringe);
		}
	}
}
