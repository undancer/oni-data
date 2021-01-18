using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AdvancedCureConfig : IEntityConfig
{
	public const string ID = "AdvancedCure";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("AdvancedCure", ITEMS.PILLS.ADVANCEDCURE.NAME, ITEMS.PILLS.ADVANCEDCURE.DESC, 1f, unitMass: true, Assets.GetAnim("vial_spore_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.MedicalSupplies);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 1f),
			new ComplexRecipe.RecipeElement("LightBugOrangeEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("AdvancedCure", 1f)
		};
		recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2), array, array2)
		{
			time = 200f,
			description = ITEMS.PILLS.ADVANCEDCURE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"Apothecary"
			},
			sortOrder = 20,
			requiredTech = "MedicineIV"
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
