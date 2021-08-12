using STRINGS;
using TUNING;
using UnityEngine;

public class SpicyTofuConfig : IEntityConfig
{
	public const string ID = "SpicyTofu";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SpicyTofu", ITEMS.FOOD.SPICYTOFU.NAME, ITEMS.FOOD.SPICYTOFU.DESC, 1f, unitMass: false, Assets.GetAnim("spicey_tofu_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true), FOOD.FOOD_TYPES.SPICY_TOFU);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
