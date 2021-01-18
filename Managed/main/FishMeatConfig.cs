using STRINGS;
using TUNING;
using UnityEngine;

public class FishMeatConfig : IEntityConfig
{
	public const string ID = "FishMeat";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FishMeat", ITEMS.FOOD.FISHMEAT.NAME, ITEMS.FOOD.FISHMEAT.DESC, 1f, unitMass: false, Assets.GetAnim("pacufillet_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FISH_MEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
