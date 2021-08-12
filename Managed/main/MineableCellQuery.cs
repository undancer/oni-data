using System.Collections.Generic;

public class MineableCellQuery : PathFinderQuery
{
	public List<int> result_cells = new List<int>();

	private Tag element;

	private int max_results;

	public static List<Direction> DIRECTION_CHECKS = new List<Direction>
	{
		Direction.Down,
		Direction.Right,
		Direction.Left,
		Direction.Up
	};

	public MineableCellQuery Reset(Tag element, int max_results)
	{
		this.element = element;
		this.max_results = max_results;
		result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!result_cells.Contains(cell) && CheckValidMineCell(element, cell))
		{
			result_cells.Add(cell);
		}
		return result_cells.Count >= max_results;
	}

	private bool CheckValidMineCell(Tag element, int testCell)
	{
		if (!Grid.IsValidCell(testCell))
		{
			return false;
		}
		foreach (Direction dIRECTION_CHECK in DIRECTION_CHECKS)
		{
			int cellInDirection = Grid.GetCellInDirection(testCell, dIRECTION_CHECK);
			if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && Grid.Element[cellInDirection].tag == element)
			{
				return true;
			}
		}
		return false;
	}
}
