using STRINGS;
using TUNING;
using UnityEngine;

public class SwampFruitConfig : IEntityConfig
{
	public static string ID = "SwampFruit";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.SWAMPFRUIT.NAME, ITEMS.FOOD.SWAMPFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("swampcrop_fruit_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 1f, 0.72f, isPickupable: true), FOOD.FOOD_TYPES.SWAMPFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
