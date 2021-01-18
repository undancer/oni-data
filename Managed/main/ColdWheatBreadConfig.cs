using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatBreadConfig : IEntityConfig
{
	public const string ID = "ColdWheatBread";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("ColdWheatBread", ITEMS.FOOD.COLDWHEATBREAD.NAME, ITEMS.FOOD.COLDWHEATBREAD.DESC, 1f, unitMass: false, Assets.GetAnim("frostbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.COLD_WHEAT_BREAD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
