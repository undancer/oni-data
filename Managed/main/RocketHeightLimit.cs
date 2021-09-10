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
		int num2 = component.CraftInterface.MaxHeight;
		RocketEngineCluster component2 = existingModule.GetComponent<RocketEngineCluster>();
		RocketEngineCluster component3 = selectedPart.BuildingComplete.GetComponent<RocketEngineCluster>();
		if (selectionContext == SelectionContext.ReplaceModule && component2 != null)
		{
			num2 = ((!(component3 != null)) ? (-1) : component3.maxHeight);
		}
		if (component3 != null && selectionContext == SelectionContext.AddModuleBelow)
		{
			num2 = component3.maxHeight;
		}
		if (num2 != -1)
		{
			return component.CraftInterface.RocketHeight + num <= num2;
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
