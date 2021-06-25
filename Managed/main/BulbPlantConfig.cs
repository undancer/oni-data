using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BulbPlantConfig : IEntityConfig
{
	public const string ID = "BulbPlant";

	public const string SEED_ID = "BulbPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER1;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BulbPlant", STRINGS.CREATURES.SPECIES.BULBPLANT.NAME, STRINGS.CREATURES.SPECIES.BULBPLANT.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_bulb_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 288f, 293.15f, 313.15f, 333.15f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.15f, null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 220f, "BulbPlantOriginal", STRINGS.CREATURES.SPECIES.BULBPLANT.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "BulbPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.BULBPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.BULBPLANT.DESC, Assets.GetAnim("seed_potted_bulb_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 12, STRINGS.CREATURES.SPECIES.BULBPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "BulbPlant_preview", Assets.GetAnim("potted_bulb_kanim"), "place", 1, 1);
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
		def.singleEmitQuantity = 0;
		def.averageEmitPerSecond = 5000;
		def.emitFrequency = 5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
