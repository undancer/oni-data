using STRINGS;
using TUNING;
using UnityEngine;

public class FieldRationConfig : IEntityConfig
{
	public const string ID = "FieldRation";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("FieldRation", ITEMS.FOOD.FIELDRATION.NAME, ITEMS.FOOD.FIELDRATION.DESC, 1f, unitMass: false, Assets.GetAnim("fieldration_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.FIELDRATION);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
