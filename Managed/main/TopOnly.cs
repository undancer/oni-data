using STRINGS;
using UnityEngine;

public class TopOnly : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		Debug.Assert(existingModule != null, "Existing module is null in top only condition");
		if (selectionContext == SelectionContext.ReplaceModule)
		{
			Debug.Assert(existingModule.GetComponent<LaunchPad>() == null, "Trying to replace launch pad with rocket module");
			if (!(existingModule.GetComponent<BuildingAttachPoint>() == null))
			{
				return existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding == null;
			}
			return true;
		}
		if (existingModule.GetComponent<LaunchPad>() != null)
		{
			return true;
		}
		if (existingModule.GetComponent<BuildingAttachPoint>() != null)
		{
			return existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding == null;
		}
		return false;
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.FAILED;
	}
}
