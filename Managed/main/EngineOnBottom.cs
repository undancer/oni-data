using STRINGS;
using UnityEngine;

public class EngineOnBottom : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		if (existingModule == null || existingModule.GetComponent<LaunchPad>() != null)
		{
			return true;
		}
		return selectionContext switch
		{
			SelectionContext.ReplaceModule => (existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null) ? true : false, 
			SelectionContext.AddModuleBelow => (existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null) ? true : false, 
			_ => false, 
		};
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.FAILED;
	}
}
