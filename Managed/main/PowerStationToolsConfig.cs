using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PowerStationToolsConfig : IEntityConfig
{
	public const string ID = "PowerStationTools";

	public static readonly Tag tag = TagManager.Create("PowerStationTools");

	public const float MASS = 5f;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("PowerStationTools", ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME, ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.DESC, 5f, unitMass: true, Assets.GetAnim("kit_electrician_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.MiscPickupable
		});
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
