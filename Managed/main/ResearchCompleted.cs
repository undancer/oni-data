using STRINGS;
using UnityEngine;

public class ResearchCompleted : SelectModuleCondition
{
	public override bool IgnoreInSanboxMode()
	{
		return true;
	}

	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		if (existingModule == null)
		{
			return true;
		}
		TechItem techItem = Db.Get().TechItems.TryGet(selectedPart.PrefabID);
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && techItem != null)
		{
			return techItem.IsComplete();
		}
		return true;
	}

	public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.RESEARCHED.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.RESEARCHED.FAILED;
	}
}
