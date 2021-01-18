using STRINGS;
using TUNING;
using UnityEngine;

public class SushiConfig : IEntityConfig
{
	public const string ID = "Sushi";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Sushi", ITEMS.FOOD.SUSHI.NAME, ITEMS.FOOD.SUSHI.DESC, 1f, unitMass: false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, isPickupable: true), FOOD.FOOD_TYPES.SUSHI);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
