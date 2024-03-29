using STRINGS;
using TUNING;
using UnityEngine;

public class SurfAndTurfConfig : IEntityConfig
{
	public const string ID = "SurfAndTurf";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SurfAndTurf", ITEMS.FOOD.SURFANDTURF.NAME, ITEMS.FOOD.SURFANDTURF.DESC, 1f, unitMass: false, Assets.GetAnim("surfnturf_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true), FOOD.FOOD_TYPES.SURF_AND_TURF);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
