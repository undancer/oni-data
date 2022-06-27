using STRINGS;
using UnityEngine;

public class HugMinionStates : GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetFlopCell;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsAHug);
		}
	}

	public ApproachSubState<EggIncubator> moving;

	public State waiting;

	public State behaviourcomplete;

	public FloatParameter timeout;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		moving.MoveTo(FindFlopLocation, waiting, behaviourcomplete);
		waiting.Enter(delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
		}).ParamTransition(timeout, behaviourcomplete, (Instance smi, float p) => p > 60f && !smi.GetSMI<HugMonitor.Instance>().IsHugging()).Update(delegate(Instance smi, float dt)
		{
			smi.sm.timeout.Delta(dt, smi);
		})
			.PlayAnim("waiting_pre")
			.QueueAnim("waiting_loop", loop: true)
			.ToggleStatusItem(CREATURES.STATUSITEMS.HUGMINIONWAITING.NAME, CREATURES.STATUSITEMS.HUGMINIONWAITING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsAHug);
	}

	private static int FindFlopLocation(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		FloorCellQuery floorCellQuery = PathFinderQueries.floorCellQuery.Reset(1, 1);
		component.RunQuery(floorCellQuery);
		if (floorCellQuery.result_cells.Count > 0)
		{
			smi.targetFlopCell = floorCellQuery.result_cells[Random.Range(0, floorCellQuery.result_cells.Count)];
		}
		else
		{
			smi.targetFlopCell = Grid.InvalidCell;
		}
		return smi.targetFlopCell;
	}
}
