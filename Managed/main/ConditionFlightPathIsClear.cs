using STRINGS;
using UnityEngine;

public class ConditionFlightPathIsClear : RocketFlightCondition
{
	private GameObject module;

	private int bufferWidth;

	private bool hasClearSky;

	private int obstructedTile = -1;

	public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
	{
		this.module = module;
		this.bufferWidth = bufferWidth;
	}

	public override bool EvaluateFlightCondition()
	{
		Update();
		return hasClearSky;
	}

	public override StatusItem GetFailureStatusItem()
	{
		return Db.Get().BuildingStatusItems.PathNotClear;
	}

	public void Update()
	{
		Extents extents = module.GetComponent<Building>().GetExtents();
		int x = extents.x - bufferWidth;
		int x2 = extents.x + extents.width - 1 + bufferWidth;
		int y = extents.y;
		int num = Grid.XYToCell(x, y);
		int num2 = Grid.XYToCell(x2, y);
		hasClearSky = true;
		obstructedTile = -1;
		for (int i = num; i <= num2; i++)
		{
			if (!CanReachSpace(i))
			{
				hasClearSky = false;
				break;
			}
		}
	}

	private bool CanReachSpace(int startCell)
	{
		int num = startCell;
		while (Grid.CellRow(num) < Grid.HeightInCells)
		{
			if (!Grid.IsValidCell(num) || Grid.Solid[num])
			{
				obstructedTile = num;
				return false;
			}
			num = Grid.CellAbove(num);
		}
		return true;
	}

	public string GetObstruction()
	{
		if (obstructedTile == -1)
		{
			return null;
		}
		if (Grid.Objects[obstructedTile, 1] != null)
		{
			return Grid.Objects[obstructedTile, 1].GetComponent<Building>().Def.Name;
		}
		return string.Format(BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, Grid.Element[obstructedTile].tag.ProperName());
	}
}
