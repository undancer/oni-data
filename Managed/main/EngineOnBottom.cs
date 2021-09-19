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
		switch (selectionContext)
		{
		case SelectionContext.ReplaceModule:
			if (!(existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null))
			{
				return false;
			}
			return true;
		case SelectionContext.AddModuleBelow:
			if (!(existingModule.GetComponent<AttachableBuilding>().GetAttachedTo() == null))
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.FAILED;
	}
}
