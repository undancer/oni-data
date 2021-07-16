using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class WoodLogConfig : IEntityConfig
{
	public const string ID = "WoodLog";

	public static readonly Tag TAG = TagManager.Create("WoodLog");

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("WoodLog", ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME, ITEMS.INDUSTRIAL_PRODUCTS.WOOD.DESC, 1f, unitMass: false, Assets.GetAnim("wood_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Organics,
			GameTags.BuildingWood
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
