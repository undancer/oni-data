using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GingerConfig : IEntityConfig
{
	public static string ID = "GingerConfig";

	public static int SORTORDER = 1;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.INGREDIENTS.GINGER.NAME, ITEMS.INGREDIENTS.GINGER.DESC, 1f, unitMass: true, Assets.GetAnim("ginger_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.45f, 0.4f, isPickupable: true, TUNING.SORTORDER.BUILDINGELEMENTS + SORTORDER, SimHashes.Creature, new List<Tag> { GameTags.IndustrialIngredient });
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
