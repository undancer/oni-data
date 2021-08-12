using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/AnimTileableController")]
public class AnimTileableController : KMonoBehaviour
{
	private HandleVector<int>.Handle partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	public Tag[] tags;

	private Extents extents;

	public string leftName = "#cap_left";

	public string rightName = "#cap_right";

	public string topName = "#cap_top";

	public string bottomName = "#cap_bottom";

	private KAnimSynchronizedController left;

	private KAnimSynchronizedController right;

	private KAnimSynchronizedController top;

	private KAnimSynchronizedController bottom;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (tags == null || tags.Length == 0)
		{
			tags = new Tag[1] { GetComponent<KPrefabID>().PrefabTag };
		}
	}

	protected override void OnSpawn()
	{
		OccupyArea component = GetComponent<OccupyArea>();
		if (component != null)
		{
			this.extents = component.GetExtents();
		}
		else
		{
			Building component2 = GetComponent<Building>();
			this.extents = component2.GetExtents();
		}
		Extents extents = new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2);
		partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated);
		KBatchedAnimController component3 = GetComponent<KBatchedAnimController>();
		left = new KAnimSynchronizedController(component3, (Grid.SceneLayer)component3.GetLayer(), leftName);
		right = new KAnimSynchronizedController(component3, (Grid.SceneLayer)component3.GetLayer(), rightName);
		top = new KAnimSynchronizedController(component3, (Grid.SceneLayer)component3.GetLayer(), topName);
		bottom = new KAnimSynchronizedController(component3, (Grid.SceneLayer)component3.GetLayer(), bottomName);
		UpdateEndCaps();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void UpdateEndCaps()
	{
		int cell = Grid.PosToCell(this);
		bool enable = true;
		bool enable2 = true;
		bool enable3 = true;
		bool enable4 = true;
		Grid.CellToXY(cell, out var x, out var y);
		CellOffset offset = new CellOffset(extents.x - x - 1, 0);
		CellOffset offset2 = new CellOffset(extents.x - x + extents.width, 0);
		CellOffset offset3 = new CellOffset(0, extents.y - y + extents.height);
		CellOffset offset4 = new CellOffset(0, extents.y - y - 1);
		Rotatable component = GetComponent<Rotatable>();
		if ((bool)component)
		{
			offset = component.GetRotatedCellOffset(offset);
			offset2 = component.GetRotatedCellOffset(offset2);
			offset3 = component.GetRotatedCellOffset(offset3);
			offset4 = component.GetRotatedCellOffset(offset4);
		}
		int num = Grid.OffsetCell(cell, offset);
		int num2 = Grid.OffsetCell(cell, offset2);
		int num3 = Grid.OffsetCell(cell, offset3);
		int num4 = Grid.OffsetCell(cell, offset4);
		if (Grid.IsValidCell(num))
		{
			enable = !HasTileableNeighbour(num);
		}
		if (Grid.IsValidCell(num2))
		{
			enable2 = !HasTileableNeighbour(num2);
		}
		if (Grid.IsValidCell(num3))
		{
			enable3 = !HasTileableNeighbour(num3);
		}
		if (Grid.IsValidCell(num4))
		{
			enable4 = !HasTileableNeighbour(num4);
		}
		left.Enable(enable);
		right.Enable(enable2);
		top.Enable(enable3);
		bottom.Enable(enable4);
	}

	private bool HasTileableNeighbour(int neighbour_cell)
	{
		bool result = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)objectLayer];
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null && component.HasAnyTags(tags))
			{
				result = true;
			}
		}
		return result;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (!(this == null) && !(base.gameObject == null) && partitionerEntry.IsValid())
		{
			UpdateEndCaps();
		}
	}
}
