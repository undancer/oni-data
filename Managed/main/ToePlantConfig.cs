using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ToePlantConfig : IEntityConfig
{
	public const string ID = "ToePlant";

	public const string SEED_ID = "ToePlantSeed";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ToePlant", STRINGS.CREATURES.SPECIES.TOEPLANT.NAME, STRINGS.CREATURES.SPECIES.TOEPLANT.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_toes_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, safe_elements: new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, temperature_lethal_low: TUNING.CREATURES.TEMPERATURE.FREEZING_10, temperature_warning_low: TUNING.CREATURES.TEMPERATURE.FREEZING_9, temperature_warning_high: TUNING.CREATURES.TEMPERATURE.FREEZING, temperature_lethal_high: TUNING.CREATURES.TEMPERATURE.COOL, pressure_sensitive: true, pressure_lethal_low: 0f, pressure_warning_low: 0.15f, crop_id: null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, max_age: 2400f, min_radiation: 0f, max_radiation: 220f, baseTraitId: "ToePlantOriginal", baseTraitName: STRINGS.CREATURES.SPECIES.TOEPLANT.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "ToePlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.DESC, Assets.GetAnim("seed_potted_toes_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 12, STRINGS.CREATURES.SPECIES.TOEPLANT.DOMESTICATEDDESC);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "ToePlant_preview", Assets.GetAnim("potted_toes_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
