using STRINGS;
using UnityEngine;

public class ConditionFlightPathIsClear : ProcessCondition
{
	private GameObject module;

	private int bufferWidth;

	private bool hasClearSky = false;

	private int obstructedTile = -1;

	public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
	{
		this.module = module;
		this.bufferWidth = bufferWidth;
	}

	public override Status EvaluateCondition()
	{
		Update();
		return (!hasClearSky) ? Status.Failure : Status.Ready;
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
		if (status != 0)
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
			return (status == Status.Ready) ? UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.FAILURE;
		}
		if (status != 0)
		{
			return Db.Get().BuildingStatusItems.PathNotClear.notificationTooltipText;
		}
		Debug.LogError("ConditionFlightPathIsClear: You'll need to add new strings/status items if you want to show the ready state");
		return "";
	}

	public override bool ShowInUI()
	{
		return false;
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
			if (!CanReachSpace(i))
			{
				hasClearSky = false;
				break;
			}
		}
	}

	private bool CanReachSpace(int startCell)
	{
		WorldContainer worldContainer = ((startCell >= 0) ? ClusterManager.Instance.GetWorld(Grid.WorldIdx[startCell]) : null);
		int num = ((worldContainer == null) ? Grid.HeightInCells : ((int)worldContainer.maximumBounds.y));
		int num2 = startCell;
		while (Grid.CellRow(num2) < num)
		{
			if (!Grid.IsValidCell(num2) || Grid.Solid[num2])
			{
				obstructedTile = num2;
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
			GameObject gameObject = Grid.Objects[obstructedTile, 1];
			BuildingDef def = gameObject.GetComponent<Building>().Def;
			return def.Name;
		}
		return string.Format(BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, Grid.Element[obstructedTile].tag.ProperName());
	}
}
