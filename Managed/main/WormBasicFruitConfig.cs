using STRINGS;
using TUNING;
using UnityEngine;

public class WormBasicFruitConfig : IEntityConfig
{
	public const string ID = "WormBasicFruit";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("WormBasicFruit", ITEMS.FOOD.WORMBASICFRUIT.NAME, ITEMS.FOOD.WORMBASICFRUIT.DESC, 1f, unitMass: false, Assets.GetAnim("wormwood_basic_fruit_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.7f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.WORMBASICFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
