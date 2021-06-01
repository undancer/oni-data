using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public const string ID = "PrickleGrass";

	public const string SEED_ID = "PrickleGrassSeed";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleGrass", STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME, STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("bristlebriar_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 283.15f, 303.15f, 398.15f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.15f, null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 90f, "PrickleGrassOriginal", STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "PrickleGrassSeed", STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME, STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC, Assets.GetAnim("seed_bristlebriar_kanim"), "object", 1, new List<Tag>
		{
			GameTags.DecorSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 10, STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "PrickleGrass_preview", Assets.GetAnim("bristlebriar_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
