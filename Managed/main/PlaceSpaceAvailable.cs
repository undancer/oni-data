using STRINGS;
using UnityEngine;

public class PlaceSpaceAvailable : SelectModuleCondition
{
	public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
	{
		BuildingAttachPoint component = existingModule.GetComponent<BuildingAttachPoint>();
		switch (selectionContext)
		{
		case SelectionContext.AddModuleAbove:
		{
			if (component != null && component.points[0].attachedBuilding != null && !component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells))
			{
				return false;
			}
			int cell = Grid.OffsetCell(Grid.PosToCell(existingModule), 0, existingModule.GetComponent<Building>().Def.HeightInCells);
			CellOffset[] placementOffsets = selectedPart.PlacementOffsets;
			foreach (CellOffset offset2 in placementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(cell, offset2), existingModule))
				{
					return false;
				}
			}
			return true;
		}
		case SelectionContext.AddModuleBelow:
		{
			if (!existingModule.GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells))
			{
				return false;
			}
			int cell2 = Grid.PosToCell(existingModule);
			CellOffset[] placementOffsets = selectedPart.PlacementOffsets;
			foreach (CellOffset offset3 in placementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(cell2, offset3), existingModule))
				{
					return false;
				}
			}
			return true;
		}
		case SelectionContext.ReplaceModule:
		{
			int moveAmount = selectedPart.HeightInCells - existingModule.GetComponent<Building>().Def.HeightInCells;
			if (component != null && component.points[0].attachedBuilding != null)
			{
				ReorderableBuilding component2 = existingModule.GetComponent<ReorderableBuilding>();
				if (!component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount, component2.gameObject))
				{
					return false;
				}
			}
			ReorderableBuilding component3 = existingModule.GetComponent<ReorderableBuilding>();
			CellOffset[] placementOffsets = selectedPart.PlacementOffsets;
			foreach (CellOffset offset in placementOffsets)
			{
				if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(Grid.PosToCell(component3), offset), component3.gameObject))
				{
					return false;
				}
			}
			return true;
		}
		default:
			return true;
		}
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.COMPLETE;
		}
		return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.FAILED;
	}
}
