public class SafeCellQuery : PathFinderQuery
{
	public enum SafeFlags
	{
		IsClear = 1,
		IsLightOk = 2,
		IsNotLadder = 4,
		IsNotTube = 8,
		CorrectTemperature = 0x10,
		IsNotRadiated = 0x20,
		IsBreathable = 0x40,
		IsNotLiquidOnMyFace = 0x80,
		IsNotLiquid = 0x100
	}

	private MinionBrain brain;

	private int targetCell;

	private int targetCost;

	public SafeFlags targetCellFlags;

	private bool avoid_light;

	public SafeCellQuery Reset(MinionBrain brain, bool avoid_light)
	{
		this.brain = brain;
		targetCell = PathFinder.InvalidCell;
		targetCost = int.MaxValue;
		targetCellFlags = (SafeFlags)0;
		this.avoid_light = avoid_light;
		return this;
	}

	public static SafeFlags GetFlags(int cell, MinionBrain brain, bool avoid_light = false)
	{
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return (SafeFlags)0;
		}
		if (Grid.Solid[cell] || Grid.Solid[num])
		{
			return (SafeFlags)0;
		}
		if (Grid.IsTileUnderConstruction[cell] || Grid.IsTileUnderConstruction[num])
		{
			return (SafeFlags)0;
		}
		bool flag = brain.IsCellClear(cell);
		bool num2 = !Grid.Element[cell].IsLiquid;
		bool flag2 = !Grid.Element[num].IsLiquid;
		bool num3 = Grid.Temperature[cell] > 285.15f && Grid.Temperature[cell] < 303.15f;
		bool flag3 = Grid.Radiation[cell] < 250f;
		bool flag4 = brain.OxygenBreather.IsBreathableElementAtCell(cell, Grid.DefaultOffset);
		bool flag5 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole);
		bool flag6 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Tube);
		bool flag7 = !avoid_light || SleepChore.IsDarkAtCell(cell);
		if (cell == Grid.PosToCell(brain))
		{
			flag4 = !brain.OxygenBreather.IsSuffocating;
		}
		SafeFlags safeFlags = (SafeFlags)0;
		if (flag)
		{
			safeFlags |= SafeFlags.IsClear;
		}
		if (num3)
		{
			safeFlags |= SafeFlags.CorrectTemperature;
		}
		if (flag3)
		{
			safeFlags |= SafeFlags.IsNotRadiated;
		}
		if (flag4)
		{
			safeFlags |= SafeFlags.IsBreathable;
		}
		if (flag5)
		{
			safeFlags |= SafeFlags.IsNotLadder;
		}
		if (flag6)
		{
			safeFlags |= SafeFlags.IsNotTube;
		}
		if (num2)
		{
			safeFlags |= SafeFlags.IsNotLiquid;
		}
		if (flag2)
		{
			safeFlags |= SafeFlags.IsNotLiquidOnMyFace;
		}
		if (flag7)
		{
			safeFlags |= SafeFlags.IsLightOk;
		}
		return safeFlags;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeFlags flags = GetFlags(cell, brain, avoid_light);
		bool num = flags > targetCellFlags;
		bool flag = flags == targetCellFlags && cost < targetCost;
		if (num || flag)
		{
			targetCellFlags = flags;
			targetCost = cost;
			targetCell = cell;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return targetCell;
	}
}
