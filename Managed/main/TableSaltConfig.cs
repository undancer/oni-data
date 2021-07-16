using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class TableSaltConfig : IEntityConfig
{
	public static string ID = "TableSalt";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME, ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.DESC, 1f, unitMass: false, Assets.GetAnim("seed_saltPlant_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, isPickupable: true, SORTORDER.BUILDINGELEMENTS + TableSaltTuning.SORTORDER, SimHashes.Salt, new List<Tag>
		{
			GameTags.Other
		});
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
