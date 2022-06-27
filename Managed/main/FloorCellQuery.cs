using System.Collections.Generic;

public class FloorCellQuery : PathFinderQuery
{
	public List<int> result_cells = new List<int>();

	private int max_results;

	private int adjacent_cells_buffer;

	public FloorCellQuery Reset(int max_results, int adjacent_cells_buffer = 0)
	{
		this.max_results = max_results;
		this.adjacent_cells_buffer = adjacent_cells_buffer;
		result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!result_cells.Contains(cell) && CheckValidFloorCell(cell))
		{
			result_cells.Add(cell);
		}
		return result_cells.Count >= max_results;
	}

	private bool CheckValidFloorCell(int testCell)
	{
		if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell))
		{
			return false;
		}
		int cellInDirection = Grid.GetCellInDirection(testCell, Direction.Up);
		int cellInDirection2 = Grid.GetCellInDirection(testCell, Direction.Down);
		if (!Grid.ObjectLayers[1].ContainsKey(testCell) && Grid.IsValidCell(cellInDirection2) && Grid.IsSolidCell(cellInDirection2) && Grid.IsValidCell(cellInDirection) && !Grid.IsSolidCell(cellInDirection))
		{
			int cell = testCell;
			int cell2 = testCell;
			for (int i = 0; i < adjacent_cells_buffer; i++)
			{
				cell = Grid.CellLeft(cell);
				cell2 = Grid.CellRight(cell2);
				if (!Grid.IsValidCell(cell) || Grid.IsSolidCell(cell))
				{
					return false;
				}
				if (!Grid.IsValidCell(cell2) || Grid.IsSolidCell(cell2))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
