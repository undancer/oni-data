using STRINGS;
using TUNING;
using UnityEngine;

public class GrilledPrickleFruitConfig : IEntityConfig
{
	public const string ID = "GrilledPrickleFruit";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("GrilledPrickleFruit", ITEMS.FOOD.GRILLEDPRICKLEFRUIT.NAME, ITEMS.FOOD.GRILLEDPRICKLEFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("gristleberry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, isPickupable: true), FOOD.FOOD_TYPES.GRILLED_PRICKLEFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
