using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ComplexRecipe
{
	public enum RecipeNameDisplay
	{
		Ingredient,
		Result,
		IngredientToResult,
		ResultWithIngredient
	}

	public class RecipeElement
	{
		public Tag material;

		public float amount
		{
			get;
			private set;
		}

		public RecipeElement(Tag material, float amount)
		{
			this.material = material;
			this.amount = amount;
		}
	}

	public string id;

	public RecipeElement[] ingredients;

	public RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public RecipeNameDisplay nameDisplay;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder;

	public string requiredTech;

	public Tag FirstResult => results[0].material;

	public ComplexRecipe(string id, RecipeElement[] ingredients, RecipeElement[] results)
	{
		this.id = id;
		this.ingredients = ingredients;
		this.results = results;
		ComplexRecipeManager.Get().Add(this);
	}

	public float TotalResultUnits()
	{
		float num = 0f;
		RecipeElement[] array = results;
		foreach (RecipeElement recipeElement in array)
		{
			num += recipeElement.amount;
		}
		return num;
	}

	public bool RequiresTechUnlock()
	{
		return !string.IsNullOrEmpty(requiredTech);
	}

	public bool IsRequiredTechUnlocked()
	{
		if (string.IsNullOrEmpty(requiredTech))
		{
			return true;
		}
		return Db.Get().Techs.Get(requiredTech).IsComplete();
	}

	public Sprite GetUIIcon()
	{
		Sprite result = null;
		KBatchedAnimController component = Assets.GetPrefab((nameDisplay == RecipeNameDisplay.Ingredient) ? ingredients[0].material : results[0].material).GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			result = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
		}
		return result;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName(bool includeAmounts)
	{
		switch (nameDisplay)
		{
		case RecipeNameDisplay.Result:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, results[0].material.ProperName(), results[0].amount);
			}
			return results[0].material.ProperName();
		case RecipeNameDisplay.IngredientToResult:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_INCLUDE_AMOUNTS, ingredients[0].material.ProperName(), results[0].material.ProperName(), ingredients[0].amount, results[0].amount);
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, ingredients[0].material.ProperName(), results[0].material.ProperName());
		case RecipeNameDisplay.ResultWithIngredient:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH_INCLUDE_AMOUNTS, ingredients[0].material.ProperName(), results[0].material.ProperName(), ingredients[0].amount, results[0].amount);
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, ingredients[0].material.ProperName(), results[0].material.ProperName());
		default:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, ingredients[0].material.ProperName(), ingredients[0].amount);
			}
			return ingredients[0].material.ProperName();
		}
	}
}
