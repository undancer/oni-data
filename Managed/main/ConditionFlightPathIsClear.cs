using STRINGS;
using UnityEngine;

public class ConditionFlightPathIsClear : ProcessCondition
{
	private GameObject module;

	private int bufferWidth;

	private bool hasClearSky = false;

	private int obstructedTile = -1;

	public static int maximumRocketHeight = 35;

	public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
	{
		this.module = module;
		this.bufferWidth = bufferWidth;
	}

	public override Status EvaluateCondition()
	{
		Update();
		return hasClearSky ? Status.Ready : Status.Failure;
	}

	public override StatusItem GetStatusItem(Status status)
	{
		if (status == Status.Failure)
		{
			return Db.Get().BuildingStatusItems.PathNotClear;
		}
		return null;
	}

	public override string GetStatusMessage(Status status)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.FAILURE;
		}
		if (status != Status.Ready)
		{
			return Db.Get().BuildingStatusItems.PathNotClear.notificationText;
		}
		Debug.LogError("ConditionFlightPathIsClear: You'll need to add new strings/status items if you want to show the ready state");
		return "";
	}

	public override string GetStatusTooltip(Status status)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.TOOLTIP.FAILURE;
		}
		if (status != Status.Ready)
		{
			return Db.Get().BuildingStatusItems.PathNotClear.notificationTooltipText;
		}
		Debug.LogError("ConditionFlightPathIsClear: You'll need to add new strings/status items if you want to show the ready state");
		return "";
	}

	public override bool ShowInUI()
	{
		return DlcManager.IsExpansion1Active();
	}

	public void Update()
	{
		Building component = module.GetComponent<Building>();
		Extents extents = component.GetExtents();
		int x = extents.x - bufferWidth;
		int x2 = extents.x + extents.width - 1 + bufferWidth;
		int y = extents.y;
		int num = Grid.XYToCell(x, y);
		int num2 = Grid.XYToCell(x2, y);
		hasClearSky = true;
		obstructedTile = -1;
		for (int i = num; i <= num2; i++)
		{
			if (!CanReachSpace(i, out obstructedTile))
			{
				hasClearSky = false;
				break;
			}
		}
	}

	public static int PadPositionDistanceToCeiling(GameObject launchpad)
	{
		return (int)launchpad.GetMyWorld().maximumBounds.y - Grid.TopBorderHeight - Grid.CellToXY(launchpad.GetComponent<LaunchPad>().PadPosition).y;
	}

	public static bool CheckFlightPathClear(CraftModuleInterface craft, GameObject launchpad, out int obstruction)
	{
		Vector2I vector2I = Grid.CellToXY(launchpad.GetComponent<LaunchPad>().PadPosition);
		int num = PadPositionDistanceToCeiling(launchpad);
		foreach (Ref<RocketModuleCluster> clusterModule in craft.ClusterModules)
		{
			Building component = clusterModule.Get().GetComponent<Building>();
			int widthInCells = component.Def.WidthInCells;
			int moduleRelativeVerticalPosition = craft.GetModuleRelativeVerticalPosition(clusterModule.Get().gameObject);
			if (moduleRelativeVerticalPosition + component.Def.HeightInCells <= num)
			{
				for (int i = moduleRelativeVerticalPosition; i < num; i++)
				{
					for (int j = 0; j < widthInCells; j++)
					{
						int num2 = Grid.XYToCell(j + (vector2I.x - widthInCells / 2), i + vector2I.y);
						GameObject gameObject = Grid.Objects[num2, 1];
						bool flag = Grid.Solid[num2] && (gameObject == null || !gameObject.HasTag(GameTags.DontBlockRockets));
						if (!Grid.IsValidCell(num2) || Grid.WorldIdx[num2] != Grid.WorldIdx[launchpad.GetComponent<LaunchPad>().PadPosition] || flag)
						{
							obstruction = num2;
							return false;
						}
					}
				}
				continue;
			}
			int num3 = (obstruction = Grid.XYToCell(vector2I.x, moduleRelativeVerticalPosition + vector2I.y));
			return false;
		}
		obstruction = -1;
		return true;
	}

	private static bool CanReachSpace(int startCell, out int obstruction)
	{
		WorldContainer worldContainer = ((startCell >= 0) ? ClusterManager.Instance.GetWorld(Grid.WorldIdx[startCell]) : null);
		int num = ((worldContainer == null) ? Grid.HeightInCells : ((int)worldContainer.maximumBounds.y));
		obstruction = -1;
		int num2 = startCell;
		while (Grid.CellRow(num2) < num)
		{
			if (!Grid.IsValidCell(num2) || Grid.Solid[num2])
			{
				GameObject gameObject = Grid.Objects[num2, 1];
				if (gameObject == null || !gameObject.HasTag(GameTags.DontBlockRockets))
				{
					obstruction = num2;
					return false;
				}
			}
			num2 = Grid.CellAbove(num2);
		}
		return true;
	}

	public string GetObstruction()
	{
		if (obstructedTile == -1)
		{
			return null;
		}
		if (Grid.Objects[obstructedTile, 1] != null)
		{
			GameObject gameObject = Grid.Objects[obstructedTile, 1];
			BuildingDef def = gameObject.GetComponent<Building>().Def;
			return def.Name;
		}
		return string.Format(BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, Grid.Element[obstructedTile].tag.ProperName());
	}
}
