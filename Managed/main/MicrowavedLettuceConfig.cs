using STRINGS;
using TUNING;
using UnityEngine;

public class MicrowavedLettuceConfig : IEntityConfig
{
	public const string ID = "MicrowavedLettuce";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("MicrowavedLettuce", ITEMS.FOOD.MICROWAVEDLETTUCE.NAME, ITEMS.FOOD.MICROWAVEDLETTUCE.DESC, 1f, unitMass: false, Assets.GetAnim("sea_lettuce_leaves_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.MICROWAVED_LETTUCE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
