using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class AntihistamineConfig : IEntityConfig
{
	public const string ID = "Antihistamine";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Antihistamine", ITEMS.PILLS.ANTIHISTAMINE.NAME, ITEMS.PILLS.ANTIHISTAMINE.DESC, 1f, unitMass: true, Assets.GetAnim("pill_allergies_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.ANTIHISTAMINE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("PrickleFlowerSeed", 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Dirt.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Antihistamine", 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		recipe = new ComplexRecipe(id, array, array2)
		{
			time = 100f,
			description = ITEMS.PILLS.ANTIHISTAMINE.RECIPEDESC,
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
