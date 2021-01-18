using STRINGS;
using UnityEngine;

public class ModuleCountLimit : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		if (selectionContext == SelectionContext.ReplaceModule)
		{
			return true;
		}
		if (existingModule == null)
		{
			return true;
		}
		RocketModule component = existingModule.GetComponent<RocketModule>();
		if (component == null)
		{
			return true;
		}
		return component.CraftInterface.MaxModules == -1 || component.CraftInterface.Modules.Count < component.CraftInterface.MaxModules;
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_MODULES.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_MODULES.FAILED;
	}
}
