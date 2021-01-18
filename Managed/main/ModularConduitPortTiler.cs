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

	private Extents extents;

	private AnimCapType leftCapSetting = AnimCapType.Default;

	private AnimCapType rightCapSetting = AnimCapType.Default;

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
		KPrefabID component = GetComponent<KPrefabID>();
		component.AddTag(GameTags.ModularConduitPort, serialize: true);
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
		leftCapDefault = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), leftCapDefaultStr);
		leftCapLaunchpad = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), leftCapLaunchpadStr);
		leftCapConduit = new KAnimSynchronizedController(component2, (Grid.SceneLayer)(component2.GetLayer() + 1), leftCapConduitStr);
		rightCapDefault = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), rightCapDefaultStr);
		rightCapLaunchpad = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), rightCapLaunchpadStr);
		rightCapConduit = new KAnimSynchronizedController(component2, (Grid.SceneLayer)component2.GetLayer(), rightCapConduitStr);
		Extents extents = new Extents(this.extents.x - 1, this.extents.y, this.extents.width + 2, this.extents.height);
		partitionerEntry = GameScenePartitioner.Instance.Add("ModularConduitPort.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated);
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
		Grid.CellToXY(cell, out var x, out var _);
		CellOffset offset = new CellOffset(extents.x - x - 1, 0);
		CellOffset offset2 = new CellOffset(extents.x - x + extents.width, 0);
		int num = Grid.OffsetCell(cell, offset);
		int num2 = Grid.OffsetCell(cell, offset2);
		if (Grid.IsValidCell(num))
		{
			if (HasTileableNeighbour(num))
			{
				leftCapSetting = AnimCapType.Conduit;
			}
			else if (HasLaunchpadNeighbour(num))
			{
				leftCapSetting = AnimCapType.Launchpad;
			}
			else
			{
				leftCapSetting = AnimCapType.Default;
			}
		}
		if (Grid.IsValidCell(num2))
		{
			if (HasTileableNeighbour(num2))
			{
				rightCapSetting = AnimCapType.Conduit;
			}
			else if (HasLaunchpadNeighbour(num2))
			{
				rightCapSetting = AnimCapType.Launchpad;
			}
			else
			{
				rightCapSetting = AnimCapType.Default;
			}
		}
		leftCapDefault.Enable(leftCapSetting == AnimCapType.Default);
		leftCapConduit.Enable(leftCapSetting == AnimCapType.Conduit);
		leftCapLaunchpad.Enable(leftCapSetting == AnimCapType.Launchpad);
		rightCapDefault.Enable(rightCapSetting == AnimCapType.Default);
		rightCapConduit.Enable(rightCapSetting == AnimCapType.Conduit);
		rightCapLaunchpad.Enable(rightCapSetting == AnimCapType.Launchpad);
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
		if (gameObject != null)
		{
			LaunchPad component = gameObject.GetComponent<LaunchPad>();
			if (component != null)
			{
				return true;
			}
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
}
