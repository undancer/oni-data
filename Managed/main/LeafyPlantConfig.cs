using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeafyPlantConfig : IEntityConfig
{
	public const string ID = "LeafyPlant";

	public const string SEED_ID = "LeafyPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("LeafyPlant", STRINGS.CREATURES.SPECIES.LEAFYPLANT.NAME, STRINGS.CREATURES.SPECIES.LEAFYPLANT.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_leafy_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 288f, 293.15f, 323.15f, 373f, new SimHashes[5]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.ChlorineGas,
			SimHashes.Hydrogen
		}, pressure_sensitive: true, 0f, 0.15f, null, can_drown: true, can_tinker: false);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "LeafyPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.DESC, Assets.GetAnim("seed_potted_leafy_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 7, STRINGS.CREATURES.SPECIES.LEAFYPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f), "LeafyPlant_preview", Assets.GetAnim("potted_leafy_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
