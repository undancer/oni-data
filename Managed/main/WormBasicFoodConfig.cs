using STRINGS;
using TUNING;
using UnityEngine;

public class WormBasicFoodConfig : IEntityConfig
{
	public const string ID = "WormBasicFood";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("WormBasicFood", ITEMS.FOOD.WORMBASICFOOD.NAME, ITEMS.FOOD.WORMBASICFOOD.DESC, 1f, unitMass: false, Assets.GetAnim("wormwood_roast_nuts_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.7f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.WORMBASICFOOD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
