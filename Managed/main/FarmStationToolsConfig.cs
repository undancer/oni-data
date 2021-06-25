using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FarmStationToolsConfig : IEntityConfig
{
	public const string ID = "FarmStationTools";

	public static readonly Tag tag = TagManager.Create("FarmStationTools");

	public const float MASS = 5f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FarmStationTools", ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME, ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.DESC, 5f, unitMass: true, Assets.GetAnim("kit_planttender_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
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
