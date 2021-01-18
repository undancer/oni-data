using STRINGS;
using TUNING;
using UnityEngine;

public class SwampFruitConfig : IEntityConfig
{
	public static string ID = "SwampFruit";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.SWAMPFRUIT.NAME, ITEMS.FOOD.SWAMPFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("swampcrop_fruit_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 1f, 0.72f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SWAMPFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
