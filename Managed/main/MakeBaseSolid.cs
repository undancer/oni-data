public class MakeBaseSolid : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Enter(ConvertToSolid).Exit(ConvertToVacuum);
	}

	private static void ConvertToSolid(Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		PrimaryElement component = smi.GetComponent<PrimaryElement>();
		SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature);
		Grid.Objects[num, 9] = smi.gameObject;
		Grid.Foundation[num] = true;
		Grid.SetSolid(num, solid: true, CellEventLogger.Instance.SimCellOccupierForceSolid);
		SimMessages.SetCellProperties(num, 64);
		Grid.RenderedByWorld[num] = false;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}

	private static void ConvertToVacuum(Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f);
		Grid.Objects[num, 9] = null;
		Grid.Foundation[num] = false;
		Grid.SetSolid(num, solid: false, CellEventLogger.Instance.SimCellOccupierDestroy);
		SimMessages.ClearCellProperties(num, 64);
		Grid.RenderedByWorld[num] = true;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}
}
