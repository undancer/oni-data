using STRINGS;
using TUNING;
using UnityEngine;

public class MushroomWrapConfig : IEntityConfig
{
	public const string ID = "MushroomWrap";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("MushroomWrap", ITEMS.FOOD.MUSHROOMWRAP.NAME, ITEMS.FOOD.MUSHROOMWRAP.DESC, 1f, unitMass: false, Assets.GetAnim("mushroom_wrap_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.MUSHROOM_WRAP);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
