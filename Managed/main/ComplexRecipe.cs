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
		ResultWithIngredient,
		Composite
	}

	public class RecipeElement
	{
		public enum TemperatureOperation
		{
			AverageTemperature,
			Heated,
			Melted
		}

		public Tag material;

		public TemperatureOperation temperatureOperation;

		public bool storeElement;

		public bool inheritElement = false;

		public float amount
		{
			get;
			private set;
		}

		public RecipeElement(Tag material, float amount, bool inheritElement)
		{
			this.material = material;
			this.amount = amount;
			temperatureOperation = TemperatureOperation.AverageTemperature;
			this.inheritElement = inheritElement;
		}

		public RecipeElement(Tag material, float amount)
		{
			this.material = material;
			this.amount = amount;
			temperatureOperation = TemperatureOperation.AverageTemperature;
		}

		public RecipeElement(Tag material, float amount, TemperatureOperation temperatureOperation, bool storeElement = false)
		{
			this.material = material;
			this.amount = amount;
			this.temperatureOperation = temperatureOperation;
			this.storeElement = storeElement;
		}
	}

	public string id;

	public RecipeElement[] ingredients;

	public RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public RecipeNameDisplay nameDisplay = RecipeNameDisplay.Ingredient;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder = 0;

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
		Tech tech = Db.Get().Techs.Get(requiredTech);
		return tech.IsComplete();
	}

	public Sprite GetUIIcon()
	{
		Sprite result = null;
		Tag tag = ((nameDisplay == RecipeNameDisplay.Ingredient) ? ingredients[0].material : results[0].material);
		GameObject prefab = Assets.GetPrefab(tag);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
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
		case RecipeNameDisplay.Composite:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE_INCLUDE_AMOUNTS, ingredients[0].material.ProperName(), results[0].material.ProperName(), results[1].material.ProperName(), ingredients[0].amount, results[0].amount, results[1].amount);
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE, ingredients[0].material.ProperName(), results[0].material.ProperName(), results[1].material.ProperName());
		default:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, ingredients[0].material.ProperName(), ingredients[0].amount);
			}
			return ingredients[0].material.ProperName();
		}
	}
}