using STRINGS;
using TUNING;
using UnityEngine;

public class SwampDelightsConfig : IEntityConfig
{
	public const string ID = "SwampDelights";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("SwampDelights", ITEMS.FOOD.SWAMPDELIGHTS.NAME, ITEMS.FOOD.SWAMPDELIGHTS.DESC, 1f, unitMass: false, Assets.GetAnim("swamp_delights_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SWAMP_DELIGHTS);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
