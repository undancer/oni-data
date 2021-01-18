using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicRadPillConfig : IEntityConfig
{
	public const string ID = "BasicRadPill";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicRadPill", ITEMS.PILLS.BASICRADPILL.NAME, ITEMS.PILLS.BASICRADPILL.DESC, 1f, unitMass: true, Assets.GetAnim("pill_radiation_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.BASICRADPILL);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Carbon", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicRadPill".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		recipe = new ComplexRecipe(id, array, array2)
		{
			time = 50f,
			description = ITEMS.PILLS.BASICRADPILL.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"Apothecary"
			},
			sortOrder = 10
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
