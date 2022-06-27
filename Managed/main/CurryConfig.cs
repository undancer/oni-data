using STRINGS;
using TUNING;
using UnityEngine;

public class CurryConfig : IEntityConfig
{
	public const string ID = "Curry";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Curry", ITEMS.FOOD.CURRY.NAME, ITEMS.FOOD.CURRY.DESC, 1f, unitMass: false, Assets.GetAnim("curried_beans_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.5f, isPickupable: true), FOOD.FOOD_TYPES.CURRY);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
