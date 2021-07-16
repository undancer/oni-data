using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FoundationMonitor")]
public class FoundationMonitor : KMonoBehaviour
{
	private int position;

	[Serialize]
	public bool needsFoundation = true;

	[Serialize]
	private bool hasFoundation = true;

	public CellOffset[] monitorCells = new CellOffset[1]
	{
		new CellOffset(0, -1)
	};

	private List<HandleVector<int>.Handle> partitionerEntries = new List<HandleVector<int>.Handle>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		position = Grid.PosToCell(base.gameObject);
		CellOffset[] array = monitorCells;
		foreach (CellOffset offset in array)
		{
			int cell = Grid.OffsetCell(position, offset);
			if (Grid.IsValidCell(position) && Grid.IsValidCell(cell))
			{
				partitionerEntries.Add(GameScenePartitioner.Instance.Add("FoundationMonitor.OnSpawn", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnGroundChanged));
			}
			OnGroundChanged(null);
		}
	}

	protected override void OnCleanUp()
	{
		foreach (HandleVector<int>.Handle partitionerEntry in partitionerEntries)
		{
			HandleVector<int>.Handle handle = partitionerEntry;
			GameScenePartitioner.Instance.Free(ref handle);
		}
		base.OnCleanUp();
	}

	public bool CheckFoundationValid()
	{
		if (!needsFoundation)
		{
			return true;
		}
		return IsSuitableFoundation(position);
	}

	public bool IsSuitableFoundation(int cell)
	{
		bool flag = true;
		CellOffset[] array = monitorCells;
		foreach (CellOffset offset in array)
		{
			if (!Grid.IsCellOffsetValid(cell, offset))
			{
				return false;
			}
			int i2 = Grid.OffsetCell(cell, offset);
			flag = Grid.Solid[i2];
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!hasFoundation && CheckFoundationValid())
		{
			hasFoundation = true;
			GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.HasNoFoundation);
			Trigger(-1960061727);
		}
		if (hasFoundation && !CheckFoundationValid())
		{
			hasFoundation = false;
			GetComponent<KPrefabID>().AddTag(GameTags.Creatures.HasNoFoundation);
			Trigger(-1960061727);
		}
	}
}
