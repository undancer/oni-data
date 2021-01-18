using STRINGS;
using TUNING;
using UnityEngine;

public class GrilledPrickleFruitConfig : IEntityConfig
{
	public const string ID = "GrilledPrickleFruit";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("GrilledPrickleFruit", ITEMS.FOOD.GRILLEDPRICKLEFRUIT.NAME, ITEMS.FOOD.GRILLEDPRICKLEFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("gristleberry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.GRILLED_PRICKLEFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
