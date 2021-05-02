using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Placeable")]
public class Placeable : KMonoBehaviour
{
	public enum PlacementRules
	{
		OnFoundation,
		VisibleToSpace,
		RestrictToWorld
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	public string kAnimName;

	public string animName;

	public List<PlacementRules> placementRules = new List<PlacementRules>();

	[NonSerialized]
	public int restrictWorldId;

	public bool IsValidPlaceLocation(int cell, out string reason)
	{
		if (placementRules.Contains(PlacementRules.RestrictToWorld) && Grid.WorldIdx[cell] != restrictWorldId)
		{
			reason = UI.TOOLS.PLACE.REASONS.RESTRICT_TO_WORLD;
			return false;
		}
		if (!occupyArea.CanOccupyArea(cell, occupyArea.objectLayers[0]))
		{
			reason = UI.TOOLS.PLACE.REASONS.CAN_OCCUPY_AREA;
			return false;
		}
		if (placementRules.Contains(PlacementRules.OnFoundation) && !occupyArea.TestAreaBelow(cell, null, FoundationTest))
		{
			reason = UI.TOOLS.PLACE.REASONS.ON_FOUNDATION;
			return false;
		}
		if (placementRules.Contains(PlacementRules.VisibleToSpace) && !occupyArea.TestArea(cell, null, SunnySpaceTest))
		{
			reason = UI.TOOLS.PLACE.REASONS.VISIBLE_TO_SPACE;
			return false;
		}
		reason = "ok!";
		return true;
	}

	private bool SunnySpaceTest(int cell, object data)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		Grid.CellToXY(cell, out var x, out var y);
		int id = Grid.WorldIdx[cell];
		WorldContainer world = ClusterManager.Instance.GetWorld(id);
		int top = world.WorldOffset.y + world.WorldSize.y;
		return !Grid.Solid[cell] && !Grid.Foundation[cell] && (Grid.ExposedToSunlight[cell] >= 253 || ClearPathToSky(x, y, top));
	}

	private bool ClearPathToSky(int x, int startY, int top)
	{
		for (int i = startY; i < top; i++)
		{
			int i2 = Grid.XYToCell(x, i);
			if (Grid.Solid[i2] || Grid.Foundation[i2])
			{
				return false;
			}
		}
		return true;
	}

	private bool FoundationTest(int cell, object data)
	{
		return Grid.IsValidBuildingCell(cell) && (Grid.Solid[cell] || Grid.Foundation[cell]);
	}
}
