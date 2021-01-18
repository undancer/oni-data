using System;

public class NavTableValidator
{
	public Action<int> onDirty;

	protected bool IsClear(int cell, CellOffset[] bounding_offsets, bool is_dupe)
	{
		foreach (CellOffset offset in bounding_offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			if (!Grid.IsValidCell(cell2) || !IsCellPassable(cell2, is_dupe))
			{
				return false;
			}
			int num = Grid.CellAbove(cell2);
			if (Grid.IsValidCell(num) && Grid.Element[num].IsUnstable)
			{
				return false;
			}
		}
		return true;
	}

	protected static bool IsCellPassable(int cell, bool is_dupe)
	{
		Grid.BuildFlags buildFlags = Grid.BuildMasks[cell] & (Grid.BuildFlags.Solid | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable);
		if (buildFlags == (Grid.BuildFlags)0)
		{
			return true;
		}
		if (is_dupe)
		{
			if ((buildFlags & Grid.BuildFlags.DupeImpassable) != 0)
			{
				return false;
			}
			if ((buildFlags & Grid.BuildFlags.Solid) != 0)
			{
				return (buildFlags & Grid.BuildFlags.DupePassable) != 0;
			}
			return true;
		}
		return (buildFlags & (Grid.BuildFlags.Solid | Grid.BuildFlags.CritterImpassable)) == 0;
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}
}
