using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class IntermediateCureConfig : IEntityConfig
{
	public const string ID = "IntermediateCure";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("IntermediateCure", ITEMS.PILLS.INTERMEDIATECURE.NAME, ITEMS.PILLS.INTERMEDIATECURE.DESC, 1f, unitMass: true, Assets.GetAnim("iv_slimelung_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.MedicalSupplies);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SwampLilyFlowerConfig.ID, 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Phosphorite.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("IntermediateCure", 1f)
		};
		recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2), array, array2)
		{
			time = 100f,
			description = ITEMS.PILLS.INTERMEDIATECURE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"Apothecary"
			},
			sortOrder = 10,
			requiredTech = "MedicineII"
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
