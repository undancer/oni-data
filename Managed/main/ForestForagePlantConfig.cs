using STRINGS;
using TUNING;
using UnityEngine;

public class ForestForagePlantConfig : IEntityConfig
{
	public const string ID = "ForestForagePlant";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("ForestForagePlant", ITEMS.FOOD.FORESTFORAGEPLANT.NAME, ITEMS.FOOD.FORESTFORAGEPLANT.DESC, 1f, unitMass: false, Assets.GetAnim("podmelon_fruit_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, isPickupable: true), FOOD.FOOD_TYPES.FORESTFORAGEPLANT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
