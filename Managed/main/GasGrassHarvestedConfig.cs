using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GasGrassHarvestedConfig : IEntityConfig
{
	public const string ID = "GasGrassHarvested";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GasGrassHarvested", CREATURES.SPECIES.GASGRASS.NAME, CREATURES.SPECIES.GASGRASS.DESC, 1f, unitMass: false, Assets.GetAnim("harvested_gassygrass_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, isPickupable: true, 0, SimHashes.Creature, new List<Tag> { GameTags.Other });
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
