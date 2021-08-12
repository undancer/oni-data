using UnityEngine;

public class OffsetTracker
{
	public static bool isExecutingWithinJob;

	protected CellOffset[] offsets;

	protected int previousCell = Grid.InvalidCell;

	public virtual CellOffset[] GetOffsets(int current_cell)
	{
		if (current_cell != previousCell)
		{
			Debug.Assert(!isExecutingWithinJob, "OffsetTracker.GetOffsets() is making a mutating call but is currently executing within a job");
			UpdateCell(previousCell, current_cell);
			previousCell = current_cell;
		}
		if (offsets == null)
		{
			Debug.Assert(!isExecutingWithinJob, "OffsetTracker.GetOffsets() is making a mutating call but is currently executing within a job");
			UpdateOffsets(previousCell);
		}
		return offsets;
	}

	public void ForceRefresh()
	{
		int cell = previousCell;
		previousCell = Grid.InvalidCell;
		Refresh(cell);
	}

	public void Refresh(int cell)
	{
		GetOffsets(cell);
	}

	protected virtual void UpdateCell(int previous_cell, int current_cell)
	{
	}

	protected virtual void UpdateOffsets(int current_cell)
	{
	}

	public virtual void Clear()
	{
	}

	public virtual void DebugDrawExtents()
	{
	}

	public virtual void DebugDrawEditor()
	{
	}

	public virtual void DebugDrawOffsets(int cell)
	{
		CellOffset[] array = GetOffsets(cell);
		foreach (CellOffset offset in array)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
			Gizmos.DrawWireCube(Grid.CellToPosCCC(cell2, Grid.SceneLayer.Move), new Vector3(0.95f, 0.95f, 0.95f));
		}
	}
}
