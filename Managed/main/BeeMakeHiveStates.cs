using UnityEngine;

public class BeeMakeHiveStates : GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetBuildCell;

		public bool builtHome;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToMakeHome);
		}

		public void BuildHome()
		{
			Vector3 position = Grid.CellToPos(targetBuildCell);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("BeeHive".ToTag()), position, Quaternion.identity);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.ElementID = SimHashes.Creature;
			component.Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
			gameObject.SetActive(value: true);
			BeeHive.StatesInstance sMI = gameObject.GetSMI<BeeHive.StatesInstance>();
			sMI.SetUpNewHive();
		}
	}

	public State findBuildLocation;

	public State moveToBuildLocation;

	public State doBuild;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = findBuildLocation;
		root.DoNothing();
		findBuildLocation.Enter(delegate(Instance smi)
		{
			FindBuildLocation(smi);
			if (smi.targetBuildCell != Grid.InvalidCell)
			{
				smi.GoTo(moveToBuildLocation);
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		moveToBuildLocation.MoveTo((Instance smi) => smi.targetBuildCell, doBuild, behaviourcomplete);
		doBuild.PlayAnim("hive_grow_pre").EventHandler(GameHashes.AnimQueueComplete, delegate(Instance smi)
		{
			Bee component = smi.gameObject.GetComponent<Bee>();
			KPrefabID x = component.FindHiveInRoom();
			if (x == null)
			{
				smi.builtHome = true;
				smi.BuildHome();
			}
			smi.GoTo(behaviourcomplete);
		});
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToMakeHome).Exit(delegate(Instance smi)
		{
			if (smi.builtHome)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			}
		});
	}

	private void FindBuildLocation(Instance smi)
	{
		smi.targetBuildCell = Grid.InvalidCell;
		GameObject prefab = Assets.GetPrefab("BeeHive".ToTag());
		BuildingPlacementQuery buildingPlacementQuery = PathFinderQueries.buildingPlacementQuery.Reset(1, prefab);
		Navigator component = smi.GetComponent<Navigator>();
		component.RunQuery(buildingPlacementQuery);
		if (buildingPlacementQuery.result_cells.Count > 0)
		{
			smi.targetBuildCell = buildingPlacementQuery.result_cells[Random.Range(0, buildingPlacementQuery.result_cells.Count)];
		}
	}
}
