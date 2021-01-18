using UnityEngine;

public class ProducePowerMonitor : GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	private class SleepSearchStates : State
	{
		public State looking;

		public State found;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	private State idle;

	private SleepSearchStates searching;

	public IntParameter targetSleepCell = new IntParameter(Grid.InvalidCell);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		base.serializable = SerializeType.ParamsOnly;
		idle.Enter(delegate(Instance smi)
		{
			targetSleepCell.Set(Grid.InvalidCell, smi);
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		}).EventTransition(GameHashes.Nighttime, (Instance smi) => GameClock.Instance, searching.looking, (Instance smi) => GameClock.Instance.IsNighttime());
		searching.Enter(TryRecoverSave).EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, idle, (Instance smi) => !GameClock.Instance.IsNighttime()).Exit(delegate(Instance smi)
		{
			targetSleepCell.Set(Grid.InvalidCell, smi);
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		});
		searching.looking.Update(delegate(Instance smi, float dt)
		{
			FindSleepLocation(smi);
		}, UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.NoSleepSpot).ParamTransition(targetSleepCell, searching.found, (Instance smi, int sleepCell) => sleepCell != Grid.InvalidCell);
		searching.found.Enter(delegate(Instance smi)
		{
			smi.GetComponent<Staterpillar>().SpawnGenerator(targetSleepCell.Get(smi));
		}).ParamTransition(targetSleepCell, searching.looking, (Instance smi, int sleepCell) => sleepCell == Grid.InvalidCell).ToggleBehaviour(GameTags.Creatures.WantsToProducePower, (Instance smi) => targetSleepCell.Get(smi) != Grid.InvalidCell);
	}

	private void TryRecoverSave(Instance smi)
	{
		Staterpillar component = smi.GetComponent<Staterpillar>();
		if (targetSleepCell.Get(smi) == Grid.InvalidCell && component.GetGenerator() != null)
		{
			KPrefabID generator = component.GetGenerator();
			int value = Grid.PosToCell(generator);
			targetSleepCell.Set(value, smi);
		}
	}

	private void FindSleepLocation(Instance smi)
	{
		StaterpillarCellQuery staterpillarCellQuery = PathFinderQueries.staterpillarCellQuery.Reset(10, smi.gameObject);
		Navigator component = smi.GetComponent<Navigator>();
		component.RunQuery(staterpillarCellQuery);
		if (staterpillarCellQuery.result_cells.Count <= 0)
		{
			return;
		}
		foreach (int result_cell in staterpillarCellQuery.result_cells)
		{
			int cellInDirection = Grid.GetCellInDirection(result_cell, Direction.Down);
			if (Grid.Objects[cellInDirection, 26] != null)
			{
				targetSleepCell.Set(result_cell, smi);
				break;
			}
		}
		if (targetSleepCell.Get(smi) == Grid.InvalidCell)
		{
			targetSleepCell.Set(staterpillarCellQuery.result_cells[Random.Range(0, staterpillarCellQuery.result_cells.Count)], smi);
		}
	}
}
