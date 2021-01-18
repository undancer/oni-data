using ProcGen;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ZoneTile")]
public class ZoneTile : KMonoBehaviour
{
	[MyCmpReq]
	public Building building;

	private bool wasReplaced;

	private static readonly EventSystem.IntraObjectHandler<ZoneTile> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<ZoneTile>(delegate(ZoneTile component, object data)
	{
		component.OnObjectReplaced(data);
	});

	protected override void OnSpawn()
	{
		int[] placementCells = building.PlacementCells;
		foreach (int cell in placementCells)
		{
			SimMessages.ModifyCellWorldZone(cell, 0);
		}
		Subscribe(1606648047, OnObjectReplacedDelegate);
	}

	protected override void OnCleanUp()
	{
		if (!wasReplaced)
		{
			ClearZone();
		}
	}

	private void OnObjectReplaced(object data)
	{
		ClearZone();
		wasReplaced = true;
	}

	private void ClearZone()
	{
		int[] placementCells = building.PlacementCells;
		foreach (int cell in placementCells)
		{
			SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
			byte zone_id = ((subWorldZoneType == SubWorld.ZoneType.Space) ? byte.MaxValue : ((byte)subWorldZoneType));
			SimMessages.ModifyCellWorldZone(cell, zone_id);
		}
	}
}
