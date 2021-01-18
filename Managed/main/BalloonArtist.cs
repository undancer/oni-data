using System.Runtime.Serialization;
using KSerialization;
using TUNING;

public class BalloonArtist : GameStateMachine<BalloonArtist, BalloonArtist.Instance>
{
	public class OverjoyedStates : State
	{
		public State idle;

		public State balloon_stand;

		public State exitEarly;
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public int numBalloonsGiven;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			base.smi.sm.balloonsGivenOut.Set(numBalloonsGiven, base.smi);
		}

		public bool IsRecTime()
		{
			return base.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public void GiveBalloon()
		{
			numBalloonsGiven++;
			base.smi.sm.balloonsGivenOut.Set(numBalloonsGiven, base.smi);
		}

		public void ExitJoyReactionEarly()
		{
			JoyBehaviourMonitor.Instance sMI = base.master.gameObject.GetSMI<JoyBehaviourMonitor.Instance>();
			sMI.sm.exitEarly.Trigger(sMI);
		}
	}

	public IntParameter balloonsGivenOut;

	public State neutral;

	public OverjoyedStates overjoyed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = neutral;
		root.TagTransition(GameTags.Dead, null);
		neutral.TagTransition(GameTags.Overjoyed, overjoyed);
		overjoyed.TagTransition(GameTags.Overjoyed, neutral, on_remove: true).DefaultState(overjoyed.idle).ParamTransition(balloonsGivenOut, overjoyed.exitEarly, (Instance smi, int p) => p >= TRAITS.JOY_REACTIONS.BALLOON_ARTIST.NUM_BALLOONS_TO_GIVE)
			.Exit(delegate(Instance smi)
			{
				smi.numBalloonsGiven = 0;
				balloonsGivenOut.Set(0, smi);
			});
		overjoyed.idle.Enter(delegate(Instance smi)
		{
			if (smi.IsRecTime())
			{
				smi.GoTo(overjoyed.balloon_stand);
			}
		}).ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistPlanning).EventTransition(GameHashes.ScheduleBlocksChanged, overjoyed.balloon_stand, (Instance smi) => smi.IsRecTime());
		overjoyed.balloon_stand.ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistHandingOut).EventTransition(GameHashes.ScheduleBlocksChanged, overjoyed.idle, (Instance smi) => !smi.IsRecTime()).ToggleChore((Instance smi) => new BalloonArtistChore(smi.master), overjoyed.idle);
		overjoyed.exitEarly.Enter(delegate(Instance smi)
		{
			smi.ExitJoyReactionEarly();
		});
	}
}
