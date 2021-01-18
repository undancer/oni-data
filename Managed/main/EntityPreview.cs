using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityPreview")]
public class EntityPreview : KMonoBehaviour
{
	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpGet]
	private Storage storage;

	public ObjectLayer objectLayer = ObjectLayer.NumLayers;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle objectPartitionerEntry;

	private static readonly Func<int, object, bool> ValidTestDelegate = (int cell, object data) => ValidTest(cell, data);

	public bool Valid
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		solidPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, OnAreaChanged);
		if (objectLayer != ObjectLayer.NumLayers)
		{
			objectPartitionerEntry = GameScenePartitioner.Instance.Add("EntityPreview", base.gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnAreaChanged);
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "EntityPreview.OnSpawn");
		OnAreaChanged(null);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref objectPartitionerEntry);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		int cell = Grid.PosToCell(this);
		GameScenePartitioner.Instance.UpdatePosition(solidPartitionerEntry, cell);
		GameScenePartitioner.Instance.UpdatePosition(objectPartitionerEntry, cell);
		OnAreaChanged(null);
	}

	public void SetSolid()
	{
		occupyArea.ApplyToCells = true;
	}

	private void OnAreaChanged(object obj)
	{
		UpdateValidity();
	}

	public void UpdateValidity()
	{
		bool valid = Valid;
		Valid = occupyArea.TestArea(Grid.PosToCell(this), this, ValidTestDelegate);
		if (Valid)
		{
			animController.TintColour = Color.white;
		}
		else
		{
			animController.TintColour = Color.red;
		}
		if (valid != Valid)
		{
			Trigger(-1820564715, Valid);
		}
	}

	private static bool ValidTest(int cell, object data)
	{
		EntityPreview entityPreview = (EntityPreview)data;
		return Grid.IsValidCell(cell) && !Grid.Solid[cell] && (entityPreview.objectLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)entityPreview.objectLayer] == entityPreview.gameObject || Grid.Objects[cell, (int)entityPreview.objectLayer] == null);
	}
}
