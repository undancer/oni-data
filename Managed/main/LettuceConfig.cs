using STRINGS;
using TUNING;
using UnityEngine;

public class LettuceConfig : IEntityConfig
{
	public const string ID = "Lettuce";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Lettuce", ITEMS.FOOD.LETTUCE.NAME, ITEMS.FOOD.LETTUCE.DESC, 1f, unitMass: false, Assets.GetAnim("sea_lettuce_leaves_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true), FOOD.FOOD_TYPES.LETTUCE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
