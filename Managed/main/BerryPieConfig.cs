using STRINGS;
using TUNING;
using UnityEngine;

public class BerryPieConfig : IEntityConfig
{
	public const string ID = "BerryPie";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("BerryPie", ITEMS.FOOD.BERRYPIE.NAME, ITEMS.FOOD.BERRYPIE.DESC, 1f, unitMass: false, Assets.GetAnim("wormwood_berry_pie_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.55f, isPickupable: true), FOOD.FOOD_TYPES.BERRY_PIE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
