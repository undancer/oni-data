using UnityEngine;

public static class NavTypeHelper
{
	public static Vector3 GetNavPos(int cell, NavType nav_type)
	{
		Vector3 zero = Vector3.zero;
		return nav_type switch
		{
			NavType.Floor => Grid.CellToPosCBC(cell, Grid.SceneLayer.Move), 
			NavType.LeftWall => Grid.CellToPosLCC(cell, Grid.SceneLayer.Move), 
			NavType.RightWall => Grid.CellToPosRCC(cell, Grid.SceneLayer.Move), 
			NavType.Ceiling => Grid.CellToPosCTC(cell, Grid.SceneLayer.Move), 
			NavType.Ladder => Grid.CellToPosCCC(cell, Grid.SceneLayer.Move), 
			NavType.Pole => Grid.CellToPosCCC(cell, Grid.SceneLayer.Move), 
			NavType.Tube => Grid.CellToPosCCC(cell, Grid.SceneLayer.Move), 
			NavType.Solid => Grid.CellToPosCCC(cell, Grid.SceneLayer.Move), 
			_ => Grid.CellToPosCCC(cell, Grid.SceneLayer.Move), 
		};
	}

	public static int GetAnchorCell(NavType nav_type, int cell)
	{
		int result = Grid.InvalidCell;
		if (Grid.IsValidCell(cell))
		{
			switch (nav_type)
			{
			case NavType.Floor:
				result = Grid.CellBelow(cell);
				break;
			case NavType.LeftWall:
				result = Grid.CellLeft(cell);
				break;
			case NavType.RightWall:
				result = Grid.CellRight(cell);
				break;
			case NavType.Ceiling:
				result = Grid.CellAbove(cell);
				break;
			case NavType.Solid:
				result = cell;
				break;
			}
		}
		return result;
	}
}
