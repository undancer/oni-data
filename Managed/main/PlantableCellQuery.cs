using System.Collections.Generic;
using UnityEngine;

public class PlantableCellQuery : PathFinderQuery
{
	public List<int> result_cells = new List<int>();

	private PlantableSeed seed;

	private int max_results;

	private int plantDetectionRadius = 6;

	private int maxPlantsInRadius = 2;

	public PlantableCellQuery Reset(PlantableSeed seed, int max_results)
	{
		this.seed = seed;
		this.max_results = max_results;
		result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!result_cells.Contains(cell) && CheckValidPlotCell(seed, cell))
		{
			result_cells.Add(cell);
		}
		return result_cells.Count >= max_results;
	}

	private bool CheckValidPlotCell(PlantableSeed seed, int plant_cell)
	{
		if (!Grid.IsValidCell(plant_cell))
		{
			return false;
		}
		int num = ((seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(plant_cell) : Grid.CellAbove(plant_cell));
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.Solid[num])
		{
			return false;
		}
		if ((bool)Grid.Objects[plant_cell, 5])
		{
			return false;
		}
		if ((bool)Grid.Objects[plant_cell, 1])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[num, 1];
		if ((bool)gameObject)
		{
			PlantablePlot component = gameObject.GetComponent<PlantablePlot>();
			if (component == null)
			{
				return false;
			}
			if (component.Direction != seed.Direction)
			{
				return false;
			}
			if (component.Occupant != null)
			{
				return false;
			}
		}
		else
		{
			if (!seed.TestSuitableGround(plant_cell))
			{
				return false;
			}
			if (CountNearbyPlants(num, plantDetectionRadius) > maxPlantsInRadius)
			{
				return false;
			}
		}
		return true;
	}

	private static int CountNearbyPlants(int cell, int radius)
	{
		int x = 0;
		int y = 0;
		Grid.PosToXY(Grid.CellToPos(cell), out x, out y);
		int num = radius * 2;
		x -= radius;
		y -= radius;
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x, y, num, num, GameScenePartitioner.Instance.plants, pooledList);
		int num2 = 0;
		foreach (ScenePartitionerEntry item in pooledList)
		{
			KPrefabID kPrefabID = (KPrefabID)item.obj;
			if (!kPrefabID.GetComponent<TreeBud>())
			{
				num2++;
			}
		}
		pooledList.Recycle();
		return num2;
	}
}
