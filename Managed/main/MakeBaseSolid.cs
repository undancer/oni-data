public class MakeBaseSolid : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>
{
	public class Def : BaseDef
	{
		public CellOffset[] solidOffsets;
	}

	public new class Instance : GameInstance
	{
		[MyCmpGet]
		public BuildingComplete buildingComplete;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)103;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Enter(ConvertToSolid).Exit(ConvertToVacuum);
	}

	private static void ConvertToSolid(Instance smi)
	{
		if (!(smi.buildingComplete == null))
		{
			int cell = Grid.PosToCell(smi.gameObject);
			PrimaryElement component = smi.GetComponent<PrimaryElement>();
			Building component2 = smi.GetComponent<Building>();
			CellOffset[] solidOffsets = smi.def.solidOffsets;
			foreach (CellOffset offset in solidOffsets)
			{
				CellOffset rotatedOffset = component2.GetRotatedOffset(offset);
				int num = Grid.OffsetCell(cell, rotatedOffset);
				SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature);
				Grid.Objects[num, 9] = smi.gameObject;
				Grid.Foundation[num] = true;
				Grid.SetSolid(num, solid: true, CellEventLogger.Instance.SimCellOccupierForceSolid);
				SimMessages.SetCellProperties(num, 103);
				Grid.RenderedByWorld[num] = false;
				World.Instance.OnSolidChanged(num);
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			}
		}
	}

	private static void ConvertToVacuum(Instance smi)
	{
		if (!(smi.buildingComplete == null))
		{
			int cell = Grid.PosToCell(smi.gameObject);
			Building component = smi.GetComponent<Building>();
			CellOffset[] solidOffsets = smi.def.solidOffsets;
			foreach (CellOffset offset in solidOffsets)
			{
				CellOffset rotatedOffset = component.GetRotatedOffset(offset);
				int num = Grid.OffsetCell(cell, rotatedOffset);
				SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f);
				Grid.Objects[num, 9] = null;
				Grid.Foundation[num] = false;
				Grid.SetSolid(num, solid: false, CellEventLogger.Instance.SimCellOccupierDestroy);
				SimMessages.ClearCellProperties(num, 103);
				Grid.RenderedByWorld[num] = true;
				World.Instance.OnSolidChanged(num);
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			}
		}
	}
}
