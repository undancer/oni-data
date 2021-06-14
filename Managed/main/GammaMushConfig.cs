using STRINGS;
using TUNING;
using UnityEngine;

public class GammaMushConfig : IEntityConfig
{
	public const string ID = "GammaMush";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("GammaMush", ITEMS.FOOD.GAMMAMUSH.NAME, ITEMS.FOOD.GAMMAMUSH.DESC, 1f, unitMass: false, Assets.GetAnim("mushbarfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.GAMMAMUSH);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
