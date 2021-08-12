using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementQuery : PathFinderQuery
{
	public List<int> result_cells = new List<int>();

	private int max_results;

	private GameObject toPlace;

	private CellOffset[] cellOffsets;

	public BuildingPlacementQuery Reset(int max_results, GameObject toPlace)
	{
		this.max_results = max_results;
		this.toPlace = toPlace;
		cellOffsets = toPlace.GetComponent<OccupyArea>().OccupiedCellsOffsets;
		result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!result_cells.Contains(cell) && CheckValidPlaceCell(cell))
		{
			result_cells.Add(cell);
		}
		return result_cells.Count >= max_results;
	}

	private bool CheckValidPlaceCell(int testCell)
	{
		if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell) || Grid.ObjectLayers[1].ContainsKey(testCell))
		{
			return false;
		}
		bool flag = true;
		int widthInCells = toPlace.GetComponent<OccupyArea>().GetWidthInCells();
		int cell = testCell;
		for (int i = 0; i < widthInCells; i++)
		{
			int cellInDirection = Grid.GetCellInDirection(cell, Direction.Down);
			if (!Grid.IsValidCell(cellInDirection) || !Grid.IsSolidCell(cellInDirection))
			{
				flag = false;
				break;
			}
			cell = Grid.GetCellInDirection(cell, Direction.Right);
		}
		if (flag)
		{
			for (int j = 0; j < cellOffsets.Length; j++)
			{
				CellOffset offset = cellOffsets[j];
				int num = Grid.OffsetCell(testCell, offset);
				if (!Grid.IsValidCell(num) || Grid.IsSolidCell(num) || !Grid.IsValidBuildingCell(num) || Grid.ObjectLayers[1].ContainsKey(num))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}
}
