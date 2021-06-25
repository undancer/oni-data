using STRINGS;
using TUNING;
using UnityEngine;

public class CookedMeatConfig : IEntityConfig
{
	public const string ID = "CookedMeat";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("CookedMeat", ITEMS.FOOD.COOKEDMEAT.NAME, ITEMS.FOOD.COOKEDMEAT.DESC, 1f, unitMass: false, Assets.GetAnim("barbeque_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.COOKED_MEAT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
