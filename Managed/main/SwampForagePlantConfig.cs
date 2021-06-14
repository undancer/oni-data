using STRINGS;
using TUNING;
using UnityEngine;

public class SwampForagePlantConfig : IEntityConfig
{
	public const string ID = "SwampForagePlant";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("SwampForagePlant", ITEMS.FOOD.SWAMPFORAGEPLANT.NAME, ITEMS.FOOD.SWAMPFORAGEPLANT.DESC, 1f, unitMass: false, Assets.GetAnim("swamptuber_vegetable_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SWAMPFORAGEPLANT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
