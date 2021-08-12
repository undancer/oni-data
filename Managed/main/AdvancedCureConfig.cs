using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class AdvancedCureConfig : IEntityConfig
{
	public const string ID = "AdvancedCure";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("AdvancedCure", ITEMS.PILLS.ADVANCEDCURE.NAME, ITEMS.PILLS.ADVANCEDCURE.DESC, 1f, unitMass: true, Assets.GetAnim("vial_spore_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToMedicine(template, MEDICINE.ADVANCEDCURE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 1f),
			new ComplexRecipe.RecipeElement("LightBugOrangeEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("AdvancedCure", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string text = "Apothecary";
		recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(text, array, array2), array, array2)
		{
			time = 200f,
			description = ITEMS.PILLS.ADVANCEDCURE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { text },
			sortOrder = 20,
			requiredTech = "MedicineIV"
		};
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
