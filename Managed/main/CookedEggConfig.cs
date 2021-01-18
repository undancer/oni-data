using STRINGS;
using TUNING;
using UnityEngine;

public class CookedEggConfig : IEntityConfig
{
	public const string ID = "CookedEgg";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("CookedEgg", ITEMS.FOOD.COOKEDEGG.NAME, ITEMS.FOOD.COOKEDEGG.DESC, 1f, unitMass: false, Assets.GetAnim("cookedegg_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.COOKED_EGG);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
