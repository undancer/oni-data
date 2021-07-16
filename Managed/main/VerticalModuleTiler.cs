using UnityEngine;

public class VerticalModuleTiler : KMonoBehaviour
{
	private enum AnimCapType
	{
		ThreeWide,
		FiveWide
	}

	private HandleVector<int>.Handle partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	private Extents extents;

	private AnimCapType topCapSetting;

	private AnimCapType bottomCapSetting;

	private bool manageTopCap = true;

	private bool manageBottomCap = true;

	private KAnimSynchronizedController topCapWide;

	private KAnimSynchronizedController bottomCapWide;

	private static readonly string topCapStr = "#cap_top_5";

	private static readonly string bottomCapStr = "#cap_bottom_5";

	private bool dirty;

	[MyCmpGet]
	private KBatchedAnimController controller;

	protected override void OnSpawn()
	{
		OccupyArea component = GetComponent<OccupyArea>();
		if (component != null)
		{
			extents = component.GetExtents();
		}
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		if (manageTopCap)
		{
			topCapWide = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), topCapStr);
		}
		if (manageBottomCap)
		{
			bottomCapWide = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), bottomCapStr);
		}
		PostReorderMove();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	public void PostReorderMove()
	{
		dirty = true;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (!(this == null) && !(base.gameObject == null) && partitionerEntry.IsValid())
		{
			UpdateEndCaps();
		}
	}

	private void UpdateEndCaps()
	{
		Grid.CellToXY(Grid.PosToCell(this), out var _, out var _);
		int cellTop = GetCellTop();
		int cellBottom = GetCellBottom();
		if (Grid.IsValidCell(cellTop))
		{
			if (HasWideNeighbor(cellTop))
			{
				topCapSetting = AnimCapType.FiveWide;
			}
			else
			{
				topCapSetting = AnimCapType.ThreeWide;
			}
		}
		if (Grid.IsValidCell(cellBottom))
		{
			if (HasWideNeighbor(cellBottom))
			{
				bottomCapSetting = AnimCapType.FiveWide;
			}
			else
			{
				bottomCapSetting = AnimCapType.ThreeWide;
			}
		}
		if (manageTopCap)
		{
			topCapWide.Enable(topCapSetting == AnimCapType.FiveWide);
		}
		if (manageBottomCap)
		{
			bottomCapWide.Enable(bottomCapSetting == AnimCapType.FiveWide);
		}
	}

	private int GetCellTop()
	{
		int cell = Grid.PosToCell(this);
		Grid.CellToXY(cell, out var _, out var y);
		CellOffset offset = new CellOffset(0, extents.y - y + extents.height);
		return Grid.OffsetCell(cell, offset);
	}

	private int GetCellBottom()
	{
		int cell = Grid.PosToCell(this);
		Grid.CellToXY(cell, out var _, out var y);
		CellOffset offset = new CellOffset(0, extents.y - y - 1);
		return Grid.OffsetCell(cell, offset);
	}

	private bool HasWideNeighbor(int neighbour_cell)
	{
		bool result = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)objectLayer];
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null && component.GetComponent<ReorderableBuilding>() != null && component.GetComponent<Building>().Def.WidthInCells >= 5)
			{
				result = true;
			}
		}
		return result;
	}

	private void LateUpdate()
	{
		bottomCapWide.Dirty();
		topCapWide.Dirty();
		if (dirty)
		{
			if (partitionerEntry != HandleVector<int>.InvalidHandle)
			{
				GameScenePartitioner.Instance.Free(ref partitionerEntry);
			}
			OccupyArea component = GetComponent<OccupyArea>();
			if (component != null)
			{
				this.extents = component.GetExtents();
			}
			Extents extents = new Extents(this.extents.x, this.extents.y - 1, this.extents.width, this.extents.height + 2);
			partitionerEntry = GameScenePartitioner.Instance.Add("VerticalModuleTiler.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated);
			UpdateEndCaps();
			dirty = false;
		}
	}
}
