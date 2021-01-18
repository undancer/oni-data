using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceBreadConfig : IEntityConfig
{
	public const string ID = "SpiceBread";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("SpiceBread", ITEMS.FOOD.SPICEBREAD.NAME, ITEMS.FOOD.SPICEBREAD.DESC, 1f, unitMass: false, Assets.GetAnim("pepperbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SPICEBREAD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
