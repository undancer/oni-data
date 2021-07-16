using UnityEngine;

public class ModularConduitPortTiler : KMonoBehaviour
{
	private enum AnimCapType
	{
		Default,
		Conduit,
		Launchpad
	}

	private HandleVector<int>.Handle partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	public Tag[] tags;

	public bool manageLeftCap = true;

	public bool manageRightCap = true;

	public int leftCapDefaultSceneLayerAdjust;

	public int rightCapDefaultSceneLayerAdjust;

	private Extents extents;

	private AnimCapType leftCapSetting;

	private AnimCapType rightCapSetting;

	private static readonly string leftCapDefaultStr = "#cap_left_default";

	private static readonly string leftCapLaunchpadStr = "#cap_left_launchpad";

	private static readonly string leftCapConduitStr = "#cap_left_conduit";

	private static readonly string rightCapDefaultStr = "#cap_right_default";

	private static readonly string rightCapLaunchpadStr = "#cap_right_launchpad";

	private static readonly string rightCapConduitStr = "#cap_right_conduit";

	private KAnimSynchronizedController leftCapDefault;

	private KAnimSynchronizedController leftCapLaunchpad;

	private KAnimSynchronizedController leftCapConduit;

	private KAnimSynchronizedController rightCapDefault;

	private KAnimSynchronizedController rightCapLaunchpad;

	private KAnimSynchronizedController rightCapConduit;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<KPrefabID>().AddTag(GameTags.ModularConduitPort, serialize: true);
		if (tags == null || tags.Length == 0)
		{
			tags = new Tag[1]
			{
				GameTags.ModularConduitPort
			};
		}
	}

	protected override void OnSpawn()
	{
		OccupyArea component = GetComponent<OccupyArea>();
		if (component != null)
		{
			this.extents = component.GetExtents();
		}
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		leftCapDefault = new KAnimSynchronizedController(component2, (Grid.SceneLayer)(component2.GetLayer() + leftCapDefaultSceneLayerAdjust), leftCapDefaultStr);
		if (manageLeftCap)
		{
			leftCapLaunchpad = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), leftCapLaunchpadStr);
			leftCapConduit = new KAnimSynchronizedController(component2, (Grid.SceneLayer)(component2.GetLayer() + 1), leftCapConduitStr);
		}
		rightCapDefault = new KAnimSynchronizedController(component2, (Grid.SceneLayer)(component2.GetLayer() + rightCapDefaultSceneLayerAdjust), rightCapDefaultStr);
		if (manageRightCap)
		{
			rightCapLaunchpad = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), rightCapLaunchpadStr);
			rightCapConduit = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), rightCapConduitStr);
		}
		Extents extents = new Extents(this.extents.x - 1, this.extents.y, this.extents.width + 2, this.extents.height);
		partitionerEntry = GameScenePartitioner.Instance.Add("ModularConduitPort.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated);
		UpdateEndCaps();
		CorrectAdjacentLaunchPads();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void UpdateEndCaps()
	{
		Grid.CellToXY(Grid.PosToCell(this), out var _, out var _);
		int cellLeft = GetCellLeft();
		int cellRight = GetCellRight();
		if (Grid.IsValidCell(cellLeft))
		{
			if (HasTileableNeighbour(cellLeft))
			{
				leftCapSetting = AnimCapType.Conduit;
			}
			else if (HasLaunchpadNeighbour(cellLeft))
			{
				leftCapSetting = AnimCapType.Launchpad;
			}
			else
			{
				leftCapSetting = AnimCapType.Default;
			}
		}
		if (Grid.IsValidCell(cellRight))
		{
			if (HasTileableNeighbour(cellRight))
			{
				rightCapSetting = AnimCapType.Conduit;
			}
			else if (HasLaunchpadNeighbour(cellRight))
			{
				rightCapSetting = AnimCapType.Launchpad;
			}
			else
			{
				rightCapSetting = AnimCapType.Default;
			}
		}
		if (manageLeftCap)
		{
			leftCapDefault.Enable(leftCapSetting == AnimCapType.Default);
			leftCapConduit.Enable(leftCapSetting == AnimCapType.Conduit);
			leftCapLaunchpad.Enable(leftCapSetting == AnimCapType.Launchpad);
		}
		if (manageRightCap)
		{
			rightCapDefault.Enable(rightCapSetting == AnimCapType.Default);
			rightCapConduit.Enable(rightCapSetting == AnimCapType.Conduit);
			rightCapLaunchpad.Enable(rightCapSetting == AnimCapType.Launchpad);
		}
	}

	private int GetCellLeft()
	{
		int cell = Grid.PosToCell(this);
		Grid.CellToXY(cell, out var x, out var _);
		CellOffset offset = new CellOffset(extents.x - x - 1, 0);
		return Grid.OffsetCell(cell, offset);
	}

	private int GetCellRight()
	{
		int cell = Grid.PosToCell(this);
		Grid.CellToXY(cell, out var x, out var _);
		CellOffset offset = new CellOffset(extents.x - x + extents.width, 0);
		return Grid.OffsetCell(cell, offset);
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

	private bool HasLaunchpadNeighbour(int neighbour_cell)
	{
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)objectLayer];
		if (gameObject != null && gameObject.GetComponent<LaunchPad>() != null)
		{
			return true;
		}
		return false;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (!(this == null) && !(base.gameObject == null) && partitionerEntry.IsValid())
		{
			UpdateEndCaps();
		}
	}

	private void CorrectAdjacentLaunchPads()
	{
		int cellRight = GetCellRight();
		if (Grid.IsValidCell(cellRight) && HasLaunchpadNeighbour(cellRight))
		{
			Grid.Objects[cellRight, 1].GetComponent<ModularConduitPortTiler>().UpdateEndCaps();
		}
		int cellLeft = GetCellLeft();
		if (Grid.IsValidCell(cellLeft) && HasLaunchpadNeighbour(cellLeft))
		{
			Grid.Objects[cellLeft, 1].GetComponent<ModularConduitPortTiler>().UpdateEndCaps();
		}
	}
}
