using STRINGS;
using TUNING;
using UnityEngine;

public class ShellfishMeatConfig : IEntityConfig
{
	public const string ID = "ShellfishMeat";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("ShellfishMeat", ITEMS.FOOD.SHELLFISHMEAT.NAME, ITEMS.FOOD.SHELLFISHMEAT.DESC, 1f, unitMass: false, Assets.GetAnim("shellfish_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SHELLFISH_MEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
