using STRINGS;
using UnityEngine;

public class MaterialsAvailable : SelectModuleCondition
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
		return ProductInfoScreen.MaterialsMet(selectedPart.CraftRecipe);
	}

	public override string GetStatusTooltip(bool ready, BuildingDef selectedPart)
	{
		if (ready)
		{
			return UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.COMPLETE;
		}
		string text = UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.FAILED;
		foreach (Recipe.Ingredient ingredient in selectedPart.CraftRecipe.Ingredients)
		{
			string text2 = "\n" + string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount));
			text += text2;
		}
		return text;
	}
}
