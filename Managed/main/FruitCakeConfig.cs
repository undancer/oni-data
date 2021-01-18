using STRINGS;
using TUNING;
using UnityEngine;

public class FruitCakeConfig : IEntityConfig
{
	public const string ID = "FruitCake";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("FruitCake", ITEMS.FOOD.FRUITCAKE.NAME, ITEMS.FOOD.FRUITCAKE.DESC, 1f, unitMass: false, Assets.GetAnim("fruitcake_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.FRUITCAKE);
		ComplexRecipeManager.Get().GetRecipe(recipe.id).FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(template);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
