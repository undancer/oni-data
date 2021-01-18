using STRINGS;
using UnityEngine;

public class BeeSleepStates : GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetSleepCell;

		public float co2Exposure = 0f;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.BeeWantsToSleep);
		}
	}

	public class SleepStates : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public SleepStates sleep;

	public State findSleepLocation;

	public State moveToSleepLocation;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = findSleepLocation;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		findSleepLocation.Enter(delegate(Instance smi)
		{
			FindSleepLocation(smi);
			if (smi.targetSleepCell != Grid.InvalidCell)
			{
				smi.GoTo(moveToSleepLocation);
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		moveToSleepLocation.MoveTo((Instance smi) => smi.targetSleepCell, sleep.pre, behaviourcomplete);
		sleep.TriggerOnEnter(GameHashes.SleepStarted).TriggerOnExit(GameHashes.SleepFinished).Transition(sleep.pst, ShouldWakeUp, UpdateRate.SIM_1000ms);
		sleep.pre.QueueAnim("sleep_pre").OnAnimQueueComplete(sleep.loop);
		sleep.loop.QueueAnim("sleep_loop", loop: true);
		sleep.pst.QueueAnim("sleep_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.BeeWantsToSleep);
	}

	private static void FindSleepLocation(Instance smi)
	{
		smi.targetSleepCell = Grid.InvalidCell;
		FloorCellQuery floorCellQuery = PathFinderQueries.floorCellQuery.Reset(1);
		Navigator component = smi.GetComponent<Navigator>();
		component.RunQuery(floorCellQuery);
		if (floorCellQuery.result_cells.Count > 0)
		{
			smi.targetSleepCell = floorCellQuery.result_cells[Random.Range(0, floorCellQuery.result_cells.Count)];
		}
	}

	public static bool ShouldWakeUp(Instance smi)
	{
		BeeSleepMonitor.Instance sMI = smi.GetSMI<BeeSleepMonitor.Instance>();
		return sMI.CO2Exposure <= 0f;
	}
}
