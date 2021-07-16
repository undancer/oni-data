using STRINGS;
using TUNING;
using UnityEngine;

public class CookedFishConfig : IEntityConfig
{
	public const string ID = "CookedFish";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("CookedFish", ITEMS.FOOD.COOKEDFISH.NAME, ITEMS.FOOD.COOKEDFISH.DESC, 1f, unitMass: false, Assets.GetAnim("grilled_pacu_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true), FOOD.FOOD_TYPES.COOKED_FISH);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
