using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CactusPlantConfig : IEntityConfig
{
	public const string ID = "CactusPlant";

	public const string SEED_ID = "CactusPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("CactusPlant", STRINGS.CREATURES.SPECIES.CACTUSPLANT.NAME, STRINGS.CREATURES.SPECIES.CACTUSPLANT.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_cactus_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 200f, 273.15f, 373.15f, 400f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: false, 0f, 0.15f, null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 2200f, "CactusPlantOriginal", STRINGS.CREATURES.SPECIES.CACTUSPLANT.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "CactusPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.CACTUSPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.CACTUSPLANT.DESC, Assets.GetAnim("seed_potted_cactus_kanim"), "object", 1, new List<Tag> { GameTags.DecorSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 13, STRINGS.CREATURES.SPECIES.CACTUSPLANT.DOMESTICATEDDESC), "CactusPlant_preview", Assets.GetAnim("potted_cactus_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
