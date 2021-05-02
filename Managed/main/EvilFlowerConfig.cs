using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class EvilFlowerConfig : IEntityConfig
{
	public const string ID = "EvilFlower";

	public const string SEED_ID = "EvilFlowerSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER7;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER5;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("EvilFlower", STRINGS.CREATURES.SPECIES.EVILFLOWER.NAME, STRINGS.CREATURES.SPECIES.EVILFLOWER.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_evilflower_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 168.15f, 258.15f, 513.15f, 563.15f, new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.15f, null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, 2400f, "EvilFlowerOriginal", STRINGS.CREATURES.SPECIES.EVILFLOWER.NAME);
		EvilFlower evilFlower = gameObject.AddOrGet<EvilFlower>();
		evilFlower.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		evilFlower.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "EvilFlowerSeed", STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME, STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.DESC, Assets.GetAnim("seed_potted_evilflower_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, STRINGS.CREATURES.SPECIES.EVILFLOWER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "EvilFlower_preview", Assets.GetAnim("potted_evilflower_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex("ZombieSpores");
		def.emitFrequency = 1f;
		def.averageEmitPerSecond = 1000;
		def.singleEmitQuantity = 100000;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
