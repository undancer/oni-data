public class WireBuildTool : BaseUtilityBuildTool
{
	public static WireBuildTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		viewMode = OverlayModes.Power.ID;
	}

	protected override void ApplyPathToConduitSystem()
	{
		if (path.Count < 2)
		{
			return;
		}
		for (int i = 1; i < path.Count; i++)
		{
			if (path[i - 1].valid && path[i].valid)
			{
				int cell = path[i - 1].cell;
				int cell2 = path[i].cell;
				UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, path[i].cell);
				if (utilityConnections != 0)
				{
					UtilityConnections new_connection = utilityConnections.InverseDirection();
					conduitMgr.AddConnection(utilityConnections, cell, is_physical_building: false);
					conduitMgr.AddConnection(new_connection, cell2, is_physical_building: false);
				}
			}
		}
	}
}
