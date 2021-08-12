using STRINGS;
using UnityEngine;

public class ConditionFlightPathIsClear : ProcessCondition
{
	private GameObject module;

	private int bufferWidth;

	private bool hasClearSky;

	private int obstructedTile = -1;

	public const int MAXIMUM_ROCKET_HEIGHT = 35;

	public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
	{
		this.module = module;
		this.bufferWidth = bufferWidth;
	}

	public override Status EvaluateCondition()
	{
		Update();
		if (!hasClearSky)
		{
			return Status.Failure;
		}
		return Status.Ready;
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
		if (DlcManager.FeatureClusterSpaceEnabled())
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
		if (DlcManager.FeatureClusterSpaceEnabled())
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
		return DlcManager.FeatureClusterSpaceEnabled();
	}

	public void Update()
	{
		Extents extents = module.GetComponent<Building>().GetExtents();
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

	public static int PadTopEdgeDistanceToCeilingEdge(GameObject launchpad)
	{
		_ = launchpad.GetMyWorld().maximumBounds;
		int num = (int)launchpad.GetMyWorld().maximumBounds.y;
		int y = Grid.CellToXY(launchpad.GetComponent<LaunchPad>().RocketBottomPosition).y;
		return num - Grid.TopBorderHeight - y + 1;
	}

	public static bool CheckFlightPathClear(CraftModuleInterface craft, GameObject launchpad, out int obstruction)
	{
		Vector2I vector2I = Grid.CellToXY(launchpad.GetComponent<LaunchPad>().RocketBottomPosition);
		int num = PadTopEdgeDistanceToCeilingEdge(launchpad);
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
						bool flag = Grid.Solid[num2];
						if (!Grid.IsValidCell(num2) || Grid.WorldIdx[num2] != Grid.WorldIdx[launchpad.GetComponent<LaunchPad>().RocketBottomPosition] || flag)
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
				obstruction = num2;
				return false;
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
			return Grid.Objects[obstructedTile, 1].GetComponent<Building>().Def.Name;
		}
		return string.Format(BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, Grid.Element[obstructedTile].tag.ProperName());
	}
}
