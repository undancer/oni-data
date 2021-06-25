using STRINGS;
using TUNING;
using UnityEngine;

public class BasicForagePlantConfig : IEntityConfig
{
	public const string ID = "BasicForagePlant";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("BasicForagePlant", ITEMS.FOOD.BASICFORAGEPLANT.NAME, ITEMS.FOOD.BASICFORAGEPLANT.DESC, 1f, unitMass: false, Assets.GetAnim("muckrootvegetable_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.BASICFORAGEPLANT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
