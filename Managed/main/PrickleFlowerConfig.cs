using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public const float WATER_RATE = 71f / (678f * (float)Math.PI);

	public const string ID = "PrickleFlower";

	public const string SEED_ID = "PrickleFlowerSeed";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleFlower", STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME, STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC, 1f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("bristleblossom_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 278.15f, 303.15f, 398.15f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.15f, PrickleFruitConfig.ID);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 71f / (678f * (float)Math.PI)
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
		def.singleEmitQuantity = 1000000;
		IlluminationVulnerable illuminationVulnerable = gameObject.AddOrGet<IlluminationVulnerable>();
		illuminationVulnerable.SetPrefersDarkness();
		gameObject.AddOrGet<BlightVulnerable>();
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "PrickleFlowerSeed", STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME, STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC, Assets.GetAnim("seed_bristleblossom_kanim"), "object", 0, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "PrickleFlower_preview", Assets.GetAnim("bristleblossom_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_grow", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<PrimaryElement>().Temperature = 288.15f;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
