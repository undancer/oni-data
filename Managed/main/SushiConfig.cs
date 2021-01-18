using STRINGS;
using TUNING;
using UnityEngine;

public class SushiConfig : IEntityConfig
{
	public const string ID = "Sushi";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("Sushi", ITEMS.FOOD.SUSHI.NAME, ITEMS.FOOD.SUSHI.DESC, 1f, unitMass: false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SUSHI);
		template.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
