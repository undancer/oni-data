using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AntiCluster")]
public class MoverLayerOccupier : KMonoBehaviour, ISim200ms
{
	private int previousCell = Grid.InvalidCell;

	public ObjectLayer[] objectLayers;

	public CellOffset[] cellOffsets;

	private void RefreshCellOccupy()
	{
		int cell = Grid.PosToCell(this);
		CellOffset[] array = cellOffsets;
		foreach (CellOffset offset in array)
		{
			int current_cell = Grid.OffsetCell(cell, offset);
			if (previousCell != Grid.InvalidCell)
			{
				int previous_cell = Grid.OffsetCell(previousCell, offset);
				UpdateCell(previous_cell, current_cell);
			}
			else
			{
				UpdateCell(previousCell, current_cell);
			}
		}
		previousCell = cell;
	}

	public void Sim200ms(float dt)
	{
		RefreshCellOccupy();
	}

	private void UpdateCell(int previous_cell, int current_cell)
	{
		ObjectLayer[] array = objectLayers;
		foreach (ObjectLayer layer in array)
		{
			if (previous_cell != Grid.InvalidCell && previous_cell != current_cell && Grid.Objects[previous_cell, (int)layer] == base.gameObject)
			{
				Grid.Objects[previous_cell, (int)layer] = null;
			}
			GameObject gameObject = Grid.Objects[current_cell, (int)layer];
			if (gameObject == null)
			{
				Grid.Objects[current_cell, (int)layer] = base.gameObject;
				continue;
			}
			KPrefabID component = GetComponent<KPrefabID>();
			KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
			if (component.InstanceID > component2.InstanceID)
			{
				Grid.Objects[current_cell, (int)layer] = base.gameObject;
			}
		}
	}

	private void CleanUpOccupiedCells()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		CellOffset[] array = cellOffsets;
		foreach (CellOffset offset in array)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			ObjectLayer[] array2 = objectLayers;
			foreach (ObjectLayer layer in array2)
			{
				if (Grid.Objects[cell2, (int)layer] == base.gameObject)
				{
					Grid.Objects[cell2, (int)layer] = null;
				}
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		RefreshCellOccupy();
	}

	protected override void OnCleanUp()
	{
		CleanUpOccupiedCells();
		base.OnCleanUp();
	}
}
