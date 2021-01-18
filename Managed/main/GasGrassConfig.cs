using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasGrassConfig : IEntityConfig
{
	public const string ID = "GasGrass";

	public const string SEED_ID = "GasGrassSeed";

	public const float FERTILIZATION_RATE = 0.00083333335f;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("GasGrass", STRINGS.CREATURES.SPECIES.GASGRASS.NAME, STRINGS.CREATURES.SPECIES.GASGRASS.DESC, 1f, decor: DECOR.BONUS.TIER3, anim: Assets.GetAnim("gassygrass_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 3, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 0f, 348.15f, 373.15f, null, pressure_sensitive: true, 0f, 0.15f, "GasGrassHarvested");
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Chlorine,
				massConsumptionRate = 0.00083333335f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		HarvestDesignatable harvestDesignatable = gameObject.AddOrGet<HarvestDesignatable>();
		harvestDesignatable.defaultHarvestStateWhenPlanted = false;
		CropSleepingMonitor.Def def = gameObject.AddOrGetDef<CropSleepingMonitor.Def>();
		def.lightIntensityThreshold = 20000f;
		def.prefersDarkness = false;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "GasGrassSeed", STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.NAME, STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.DESC, Assets.GetAnim("seed_gassygrass_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.GASGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "GasGrass_preview", Assets.GetAnim("gassygrass_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
