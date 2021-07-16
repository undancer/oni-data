using STRINGS;
using UnityEngine;

public class RocketHeightLimit : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		int num = selectedPart.HeightInCells;
		if (selectionContext == SelectionContext.ReplaceModule)
		{
			num -= existingModule.GetComponent<Building>().Def.HeightInCells;
		}
		if (existingModule == null)
		{
			return true;
		}
		RocketModuleCluster component = existingModule.GetComponent<RocketModuleCluster>();
		if (component == null)
		{
			return true;
		}
		if (component.CraftInterface.MaxHeight != -1)
		{
			return component.CraftInterface.RocketHeight + num <= component.CraftInterface.MaxHeight;
		}
		return true;
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.FAILED;
	}
}
