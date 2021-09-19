using STRINGS;
using UnityEngine;

public class NoFreeRocketInterior : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		int num = 0;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (worldContainer.IsModuleInterior)
			{
				num++;
			}
		}
		return num < ClusterManager.MAX_ROCKET_INTERIOR_COUNT;
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.PASSENGER_MODULE_AVAILABLE.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.PASSENGER_MODULE_AVAILABLE.FAILED;
	}
}
