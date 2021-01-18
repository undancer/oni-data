using UnityEngine;

public abstract class SelectModuleCondition
{
	public enum SelectionContext
	{
		AddModuleAbove,
		AddModuleBelow,
		ReplaceModule
	}

	public abstract bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext);

	public abstract string GetStatusTooltip(bool ready, BuildingDef selectedPart);

	public virtual bool IgnoreInSanboxMode()
	{
		return false;
	}
}
