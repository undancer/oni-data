using STRINGS;
using TUNING;
using UnityEngine;

public class RawEggConfig : IEntityConfig
{
	public const string ID = "RawEgg";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("RawEgg", ITEMS.FOOD.RAWEGG.NAME, ITEMS.FOOD.RAWEGG.DESC, 1f, unitMass: false, Assets.GetAnim("rawegg_kanim"), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.RAWEGG);
		TemperatureCookable temperatureCookable = gameObject.AddOrGet<TemperatureCookable>();
		temperatureCookable.cookTemperature = 344.15f;
		temperatureCookable.cookedID = "CookedEgg";
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
