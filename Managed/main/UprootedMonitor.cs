using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UprootedMonitor")]
public class UprootedMonitor : KMonoBehaviour
{
	private int position;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted = false;

	public CellOffset[] monitorCells = new CellOffset[1]
	{
		new CellOffset(0, -1)
	};

	private List<HandleVector<int>.Handle> partitionerEntries = new List<HandleVector<int>.Handle>();

	private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>(delegate(UprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted);
			component.uprooted = true;
			component.Trigger(-216549700);
		}
	});

	public bool IsUprooted => uprooted || GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
		position = Grid.PosToCell(base.gameObject);
		CellOffset[] array = monitorCells;
		foreach (CellOffset offset in array)
		{
			int cell = Grid.OffsetCell(position, offset);
			if (Grid.IsValidCell(position) && Grid.IsValidCell(cell))
			{
				partitionerEntries.Add(GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnGroundChanged));
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

	public bool CheckTileGrowable()
	{
		if (!canBeUprooted)
		{
			return true;
		}
		if (uprooted)
		{
			return false;
		}
		if (!IsSuitableFoundation(position))
		{
			return false;
		}
		return true;
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
		if (!CheckTileGrowable())
		{
			uprooted = true;
		}
		if (uprooted)
		{
			GetComponent<KPrefabID>().AddTag(GameTags.Uprooted);
			Trigger(-216549700);
		}
	}
}
