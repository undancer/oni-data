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

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ToePlant", STRINGS.CREATURES.SPECIES.TOEPLANT.NAME, STRINGS.CREATURES.SPECIES.TOEPLANT.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("potted_toes_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 283.15f, 303.15f, 398.15f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.15f, null, can_drown: true, can_tinker: false);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "ToePlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.DESC, Assets.GetAnim("seed_potted_toes_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, STRINGS.CREATURES.SPECIES.TOEPLANT.DOMESTICATEDDESC);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "ToePlant_preview", Assets.GetAnim("potted_toes_kanim"), "place", 1, 1);
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
