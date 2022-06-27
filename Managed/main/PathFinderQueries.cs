public static class PathFinderQueries
{
	public static CellQuery cellQuery = new CellQuery();

	public static CellCostQuery cellCostQuery = new CellCostQuery();

	public static CellArrayQuery cellArrayQuery = new CellArrayQuery();

	public static CellOffsetQuery cellOffsetQuery = new CellOffsetQuery();

	public static SafeCellQuery safeCellQuery = new SafeCellQuery();

	public static IdleCellQuery idleCellQuery = new IdleCellQuery();

	public static BreathableCellQuery breathableCellQuery = new BreathableCellQuery();

	public static DrawNavGridQuery drawNavGridQuery = new DrawNavGridQuery();

	public static PlantableCellQuery plantableCellQuery = new PlantableCellQuery();

	public static MineableCellQuery mineableCellQuery = new MineableCellQuery();

	public static StaterpillarCellQuery staterpillarCellQuery = new StaterpillarCellQuery();

	public static FloorCellQuery floorCellQuery = new FloorCellQuery();

	public static BuildingPlacementQuery buildingPlacementQuery = new BuildingPlacementQuery();

	public static void Reset()
	{
		cellQuery = new CellQuery();
		cellCostQuery = new CellCostQuery();
		cellArrayQuery = new CellArrayQuery();
		cellOffsetQuery = new CellOffsetQuery();
		safeCellQuery = new SafeCellQuery();
		idleCellQuery = new IdleCellQuery();
		breathableCellQuery = new BreathableCellQuery();
		drawNavGridQuery = new DrawNavGridQuery();
		plantableCellQuery = new PlantableCellQuery();
		mineableCellQuery = new MineableCellQuery();
		staterpillarCellQuery = new StaterpillarCellQuery();
		floorCellQuery = new FloorCellQuery();
		buildingPlacementQuery = new BuildingPlacementQuery();
	}
}
