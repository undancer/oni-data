using UnityEngine;

public class BeeMakeHiveStates : GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Bee bee;

		private GameObject hive;

		public int targetBuildCell;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			bee = base.smi.master.GetComponent<Bee>();
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToMakeHome);
		}

		public void BuildHome()
		{
			Vector3 position = Grid.CellToPos(targetBuildCell);
			hive = Util.KInstantiate(Assets.GetPrefab("BeeHive".ToTag()), position, Quaternion.identity);
			PrimaryElement component = hive.GetComponent<PrimaryElement>();
			component.ElementID = SimHashes.Creature;
			component.Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
			hive.SetActive(value: true);
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
		doBuild.Enter(delegate(Instance smi)
		{
			smi.BuildHome();
		}).GoTo(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToMakeHome).Exit(delegate(Instance smi)
		{
			Util.KDestroyGameObject(smi.master.gameObject);
		});
	}

	private static void FindBuildLocation(Instance smi)
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
