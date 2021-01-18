public class UtilityBuildTool : BaseUtilityBuildTool
{
	public static UtilityBuildTool Instance;

	private int lastPathHead = -1;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		populateHitsList = true;
		canChangeDragAxis = false;
	}

	protected override void ApplyPathToConduitSystem()
	{
		if (path.Count < 2)
		{
			return;
		}
		for (int i = 1; i < path.Count; i++)
		{
			if (!path[i - 1].valid || !path[i].valid)
			{
				continue;
			}
			int cell = path[i - 1].cell;
			int cell2 = path[i].cell;
			UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, cell2);
			if (utilityConnections != 0)
			{
				UtilityConnections new_connection = utilityConnections.InverseDirection();
				if (conduitMgr.CanAddConnection(utilityConnections, cell, is_physical_building: false, out var fail_reason) && conduitMgr.CanAddConnection(new_connection, cell2, is_physical_building: false, out fail_reason))
				{
					conduitMgr.AddConnection(utilityConnections, cell, is_physical_building: false);
					conduitMgr.AddConnection(new_connection, cell2, is_physical_building: false);
				}
				else if (i == path.Count - 1 && lastPathHead != i)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, fail_reason, null, Grid.CellToPosCCC(cell2, (Grid.SceneLayer)0));
				}
			}
		}
		lastPathHead = path.Count - 1;
	}
}
