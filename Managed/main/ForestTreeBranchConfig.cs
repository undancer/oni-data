using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ForestTreeBranchConfig : IEntityConfig
{
	public const string ID = "ForestTreeBranch";

	public const float WOOD_AMOUNT = 300f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ForestTreeBranch", STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME, STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC, 8f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("tree_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: new List<Tag>(), defaultTemperature: 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, pressure_sensitive: true, 0f, 0.15f, "WoodLog", can_drown: true, can_tinker: true, require_solid_tile: false, should_grow_old: true, 12000f, 0f, 980f, "ForestTreeBranchOriginal", STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME);
		gameObject.AddOrGet<TreeBud>();
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<BudUprootedMonitor>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
