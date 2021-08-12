using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class IntermediateCureConfig : IEntityConfig
{
	public const string ID = "IntermediateCure";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("IntermediateCure", ITEMS.PILLS.INTERMEDIATECURE.NAME, ITEMS.PILLS.INTERMEDIATECURE.DESC, 1f, unitMass: true, Assets.GetAnim("iv_slimelung_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToMedicine(template, MEDICINE.INTERMEDIATECURE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SwampLilyFlowerConfig.ID, 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Phosphorite.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("IntermediateCure", 1f)
		};
		string text = "Apothecary";
		recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(text, array, array2), array, array2)
		{
			time = 100f,
			description = ITEMS.PILLS.INTERMEDIATECURE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { text },
			sortOrder = 10,
			requiredTech = "MedicineII"
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
