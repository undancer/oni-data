using STRINGS;
using TUNING;
using UnityEngine;

public class WormSuperFruitConfig : IEntityConfig
{
	public const string ID = "WormSuperFruit";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("WormSuperFruit", ITEMS.FOOD.WORMSUPERFRUIT.NAME, ITEMS.FOOD.WORMSUPERFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("wormwood_super_fruits_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.WORMSUPERFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
