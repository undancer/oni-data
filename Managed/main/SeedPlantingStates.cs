using STRINGS;
using UnityEngine;

public class SeedPlantingStates : GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>
{
	public class Def : BaseDef
	{
		public string prefix;

		public Def(string prefix)
		{
			this.prefix = prefix;
		}
	}

	public new class Instance : GameInstance
	{
		public PlantablePlot targetPlot;

		public int targetDirtPlotCell = Grid.InvalidCell;

		public Element plantElement = ElementLoader.FindElementByHash(SimHashes.Dirt);

		public Pickupable targetSeed;

		public int seed_cell = Grid.InvalidCell;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToPlantSeed);
		}
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	public State findSeed;

	public State moveToSeed;

	public State pickupSeed;

	public State findPlantLocation;

	public State moveToPlantLocation;

	public State moveToPlot;

	public State moveToDirt;

	public State planting;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = findSeed;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.PLANTINGSEED.NAME, CREATURES.STATUSITEMS.PLANTINGSEED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Exit(UnreserveSeed).Exit(DropAll)
			.Exit(RemoveMouthOverride);
		findSeed.Enter(delegate(Instance smi)
		{
			FindSeed(smi);
			if (smi.targetSeed == null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else
			{
				ReserveSeed(smi);
				smi.GoTo(moveToSeed);
			}
		});
		moveToSeed.MoveTo(GetSeedCell, findPlantLocation, behaviourcomplete);
		findPlantLocation.Enter(delegate(Instance smi)
		{
			if ((bool)smi.targetSeed)
			{
				FindDirtPlot(smi);
				if (smi.targetPlot != null || smi.targetDirtPlotCell != Grid.InvalidCell)
				{
					smi.GoTo(pickupSeed);
				}
				else
				{
					smi.GoTo(behaviourcomplete);
				}
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		pickupSeed.PlayAnim("gather").Enter(PickupComplete).OnAnimQueueComplete(moveToPlantLocation);
		moveToPlantLocation.Enter(delegate(Instance smi)
		{
			if (smi.targetSeed == null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else if (smi.targetPlot != null)
			{
				smi.GoTo(moveToPlot);
			}
			else if (smi.targetDirtPlotCell != Grid.InvalidCell)
			{
				smi.GoTo(moveToDirt);
			}
			else
			{
				smi.GoTo(behaviourcomplete);
			}
		});
		moveToDirt.MoveTo((Instance smi) => smi.targetDirtPlotCell, planting, behaviourcomplete);
		moveToPlot.Enter(delegate(Instance smi)
		{
			if (smi.targetPlot == null || smi.targetSeed == null)
			{
				smi.GoTo(behaviourcomplete);
			}
		}).MoveTo(GetPlantableCell, planting, behaviourcomplete);
		planting.Enter(RemoveMouthOverride).PlayAnim("plant").Exit(PlantComplete)
			.OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToPlantSeed);
	}

	private static void AddMouthOverride(Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		KAnim.Build.Symbol symbol = smi.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(smi.def.prefix + "sq_mouth_cheeks");
		if (symbol != null)
		{
			component.AddSymbolOverride("sq_mouth", symbol, 1);
		}
	}

	private static void RemoveMouthOverride(Instance smi)
	{
		smi.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride("sq_mouth", 1);
	}

	private static void PickupComplete(Instance smi)
	{
		if (!smi.targetSeed)
		{
			Debug.LogWarningFormat("PickupComplete seed {0} is null", smi.targetSeed);
			return;
		}
		UnreserveSeed(smi);
		int num = Grid.PosToCell(smi.targetSeed);
		if (smi.seed_cell != num)
		{
			Debug.LogWarningFormat("PickupComplete seed {0} moved {1} != {2}", smi.targetSeed, num, smi.seed_cell);
			smi.targetSeed = null;
		}
		else if (smi.targetSeed.HasTag(GameTags.Stored))
		{
			Debug.LogWarningFormat("PickupComplete seed {0} was stored by {1}", smi.targetSeed, smi.targetSeed.storage);
			smi.targetSeed = null;
		}
		else
		{
			smi.targetSeed = EntitySplitter.Split(smi.targetSeed, 1f);
			smi.GetComponent<Storage>().Store(smi.targetSeed.gameObject);
			AddMouthOverride(smi);
		}
	}

	private static void PlantComplete(Instance smi)
	{
		PlantableSeed plantableSeed = (smi.targetSeed ? smi.targetSeed.GetComponent<PlantableSeed>() : null);
		if ((bool)plantableSeed && CheckValidPlotCell(smi, plantableSeed, smi.targetDirtPlotCell, out var plot))
		{
			if ((bool)plot)
			{
				if (plot.Occupant == null)
				{
					plot.ForceDeposit(smi.targetSeed.gameObject);
				}
			}
			else
			{
				plantableSeed.TryPlant(allow_plant_from_storage: true);
			}
		}
		smi.targetSeed = null;
		smi.seed_cell = Grid.InvalidCell;
		smi.targetPlot = null;
	}

	private static void DropAll(Instance smi)
	{
		smi.GetComponent<Storage>().DropAll();
	}

	private static int GetPlantableCell(Instance smi)
	{
		int num = Grid.PosToCell(smi.targetPlot);
		if (Grid.IsValidCell(num))
		{
			return Grid.CellAbove(num);
		}
		return num;
	}

	private static void FindDirtPlot(Instance smi)
	{
		smi.targetDirtPlotCell = Grid.InvalidCell;
		PlantableSeed component = smi.targetSeed.GetComponent<PlantableSeed>();
		PlantableCellQuery plantableCellQuery = PathFinderQueries.plantableCellQuery.Reset(component, 20);
		smi.GetComponent<Navigator>().RunQuery(plantableCellQuery);
		if (plantableCellQuery.result_cells.Count > 0)
		{
			smi.targetDirtPlotCell = plantableCellQuery.result_cells[Random.Range(0, plantableCellQuery.result_cells.Count)];
		}
	}

	private static bool CheckValidPlotCell(Instance smi, PlantableSeed seed, int cell, out PlantablePlot plot)
	{
		plot = null;
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = ((seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(cell) : Grid.CellAbove(cell));
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.Solid[num])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[num, 1];
		if ((bool)gameObject)
		{
			plot = gameObject.GetComponent<PlantablePlot>();
			return plot != null;
		}
		return seed.TestSuitableGround(cell);
	}

	private static int GetSeedCell(Instance smi)
	{
		Debug.Assert(smi.targetSeed);
		Debug.Assert(smi.seed_cell != Grid.InvalidCell);
		return smi.seed_cell;
	}

	private static void FindSeed(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Pickupable targetSeed = null;
		int num = 100;
		foreach (PlantableSeed plantableSeed in Components.PlantableSeeds)
		{
			if ((plantableSeed.HasTag(GameTags.Seed) || plantableSeed.HasTag(GameTags.CropSeed)) && !plantableSeed.HasTag(GameTags.Creatures.ReservedByCreature) && !(Vector2.Distance(smi.transform.position, plantableSeed.transform.position) > 25f))
			{
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(plantableSeed));
				if (navigationCost != -1 && navigationCost < num)
				{
					targetSeed = plantableSeed.GetComponent<Pickupable>();
					num = navigationCost;
				}
			}
		}
		smi.targetSeed = targetSeed;
		smi.seed_cell = (smi.targetSeed ? Grid.PosToCell(smi.targetSeed) : Grid.InvalidCell);
	}

	private static void ReserveSeed(Instance smi)
	{
		GameObject gameObject = (smi.targetSeed ? smi.targetSeed.gameObject : null);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveSeed(Instance smi)
	{
		GameObject go = (smi.targetSeed ? smi.targetSeed.gameObject : null);
		if (smi.targetSeed != null)
		{
			go.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}
}
