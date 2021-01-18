using System;

public static class UtilityConnectionsExtensions
{
	public static UtilityConnections InverseDirection(this UtilityConnections direction)
	{
		return direction switch
		{
			UtilityConnections.Up => UtilityConnections.Down, 
			UtilityConnections.Down => UtilityConnections.Up, 
			UtilityConnections.Left => UtilityConnections.Right, 
			UtilityConnections.Right => UtilityConnections.Left, 
			_ => throw new ArgumentException("Unexpected enum value: " + direction, "direction"), 
		};
	}

	public static UtilityConnections LeftDirection(this UtilityConnections direction)
	{
		return direction switch
		{
			UtilityConnections.Up => UtilityConnections.Left, 
			UtilityConnections.Left => UtilityConnections.Down, 
			UtilityConnections.Down => UtilityConnections.Right, 
			UtilityConnections.Right => UtilityConnections.Up, 
			_ => throw new ArgumentException("Unexpected enum value: " + direction, "direction"), 
		};
	}

	public static UtilityConnections RightDirection(this UtilityConnections direction)
	{
		return direction switch
		{
			UtilityConnections.Up => UtilityConnections.Right, 
			UtilityConnections.Right => UtilityConnections.Down, 
			UtilityConnections.Down => UtilityConnections.Left, 
			UtilityConnections.Left => UtilityConnections.Up, 
			_ => throw new ArgumentException("Unexpected enum value: " + direction, "direction"), 
		};
	}

	public static int CellInDirection(this UtilityConnections direction, int from_cell)
	{
		return direction switch
		{
			UtilityConnections.Up => from_cell + Grid.WidthInCells, 
			UtilityConnections.Down => from_cell - Grid.WidthInCells, 
			UtilityConnections.Left => from_cell - 1, 
			UtilityConnections.Right => from_cell + 1, 
			_ => throw new ArgumentException("Unexpected enum value: " + direction, "direction"), 
		};
	}

	public static UtilityConnections DirectionFromToCell(int from_cell, int to_cell)
	{
		if (to_cell == from_cell - 1)
		{
			return UtilityConnections.Left;
		}
		if (to_cell == from_cell + 1)
		{
			return UtilityConnections.Right;
		}
		if (to_cell == from_cell + Grid.WidthInCells)
		{
			return UtilityConnections.Up;
		}
		if (to_cell == from_cell - Grid.WidthInCells)
		{
			return UtilityConnections.Down;
		}
		return (UtilityConnections)0;
	}
}
