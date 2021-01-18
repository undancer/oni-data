using STRINGS;

public class InhaleStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>
{
	public class Def : BaseDef
	{
		public string inhaleSound;

		public float inhaleTime = 3f;
	}

	public new class Instance : GameInstance
	{
		public string inhaleSound;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
			inhaleSound = GlobalAssets.GetSound(def.inhaleSound);
		}

		public void StartInhaleSound()
		{
			LoopingSounds component = GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StartSound(base.smi.inhaleSound);
			}
		}

		public void StopInhaleSound()
		{
			LoopingSounds component = GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(base.smi.inhaleSound);
			}
		}
	}

	public class InhalingStates : State
	{
		public State pre;

		public State pst;

		public State full;
	}

	public State goingtoeat;

	public InhalingStates inhaling;

	public State behaviourcomplete;

	public IntParameter targetCell;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtoeat;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi);
		});
		goingtoeat.MoveTo((Instance smi) => targetCell.Get(smi), inhaling).ToggleStatusItem(CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME, CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		inhaling.DefaultState(inhaling.pre).ToggleStatusItem(CREATURES.STATUSITEMS.INHALING.NAME, CREATURES.STATUSITEMS.INHALING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		inhaling.pre.PlayAnim("inhale_pre").QueueAnim("inhale_loop", loop: true).Update("Consume", delegate(Instance smi, float dt)
		{
			smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().Consume(dt);
		})
			.EventTransition(GameHashes.ElementNoLongerAvailable, inhaling.pst)
			.Enter("StartInhaleSound", delegate(Instance smi)
			{
				smi.StartInhaleSound();
			})
			.Exit("StopInhaleSound", delegate(Instance smi)
			{
				smi.StopInhaleSound();
			})
			.ScheduleGoTo((Instance smi) => smi.def.inhaleTime, inhaling.pst);
		inhaling.pst.Transition(inhaling.full, IsFull).Transition(behaviourcomplete, GameStateMachine<InhaleStates, Instance, IStateMachineTarget, Def>.Not(IsFull));
		inhaling.full.QueueAnim("inhale_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.WantsToEat);
	}

	private static bool IsFull(Instance smi)
	{
		CreatureCalorieMonitor.Instance sMI = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		if (sMI != null)
		{
			return sMI.stomach.GetFullness() >= 1f;
		}
		return false;
	}
}
