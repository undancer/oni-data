using STRINGS;
using TUNING;
using UnityEngine;

public class BurgerConfig : IEntityConfig
{
	public const string ID = "Burger";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("Burger", ITEMS.FOOD.BURGER.NAME, ITEMS.FOOD.BURGER.DESC, 1f, unitMass: false, Assets.GetAnim("frost_burger_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.BURGER);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
