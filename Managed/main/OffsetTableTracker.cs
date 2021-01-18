public class OffsetTableTracker : OffsetTracker
{
	private readonly CellOffset[][] table;

	public HandleVector<int>.Handle solidPartitionerEntry;

	public HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private static NavGrid navGridImpl;

	private KMonoBehaviour cmp;

	private static NavGrid navGrid
	{
		get
		{
			if (navGridImpl == null)
			{
				navGridImpl = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
			}
			return navGridImpl;
		}
	}

	public OffsetTableTracker(CellOffset[][] table, KMonoBehaviour cmp)
	{
		this.table = table;
		this.cmp = cmp;
	}

	protected override void UpdateCell(int previous_cell, int current_cell)
	{
		if (previous_cell != current_cell)
		{
			base.UpdateCell(previous_cell, current_cell);
			if (!solidPartitionerEntry.IsValid())
			{
				Extents extents = new Extents(current_cell, table);
				extents.height += 2;
				extents.y--;
				solidPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", cmp.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnCellChanged);
				validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", cmp.gameObject, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, OnCellChanged);
			}
			else
			{
				GameScenePartitioner.Instance.UpdatePosition(solidPartitionerEntry, current_cell);
				GameScenePartitioner.Instance.UpdatePosition(validNavCellChangedPartitionerEntry, current_cell);
			}
			offsets = null;
		}
	}

	private static bool IsValidRow(int current_cell, CellOffset[] row)
	{
		for (int i = 1; i < row.Length; i++)
		{
			int num = Grid.OffsetCell(current_cell, row[i]);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.Solid[num])
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateOffsets(int cell, CellOffset[][] table)
	{
		HashSetPool<CellOffset, OffsetTableTracker>.PooledHashSet pooledHashSet = HashSetPool<CellOffset, OffsetTableTracker>.Allocate();
		if (Grid.IsValidCell(cell))
		{
			foreach (CellOffset[] array in table)
			{
				if (pooledHashSet.Contains(array[0]))
				{
					continue;
				}
				int cell2 = Grid.OffsetCell(cell, array[0]);
				for (int j = 0; j < navGrid.ValidNavTypes.Length; j++)
				{
					NavType navType = navGrid.ValidNavTypes[j];
					if (navType != NavType.Tube && navGrid.NavTable.IsValid(cell2, navType) && IsValidRow(cell, array))
					{
						pooledHashSet.Add(array[0]);
						break;
					}
				}
			}
		}
		if (offsets == null || offsets.Length != pooledHashSet.Count)
		{
			offsets = new CellOffset[pooledHashSet.Count];
		}
		pooledHashSet.CopyTo(offsets);
		pooledHashSet.Recycle();
	}

	protected override void UpdateOffsets(int current_cell)
	{
		base.UpdateOffsets(current_cell);
		UpdateOffsets(current_cell, table);
	}

	private void OnCellChanged(object data)
	{
		offsets = null;
	}

	public override void Clear()
	{
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref validNavCellChangedPartitionerEntry);
	}

	public static void OnPathfindingInvalidated()
	{
		navGridImpl = null;
	}
}
