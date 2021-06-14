using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SeaLettuceConfig : IEntityConfig
{
	public static string ID = "SeaLettuce";

	public const float WATER_RATE = 0.008333334f;

	public const float FERTILIZATION_RATE = 0.00083333335f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, STRINGS.CREATURES.SPECIES.SEALETTUCE.NAME, STRINGS.CREATURES.SPECIES.SEALETTUCE.DESC, 1f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim("sea_lettuce_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: 308.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 248.15f, 295.15f, 338.15f, 398.15f, new SimHashes[3]
		{
			SimHashes.Water,
			SimHashes.SaltWater,
			SimHashes.Brine
		}, pressure_sensitive: false, 0f, 0.15f, "Lettuce", can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 740f, ID + "Original", STRINGS.CREATURES.SPECIES.SEALETTUCE.NAME);
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = SimHashes.SaltWater.CreateTag(),
			massConsumptionRate = 0.008333334f
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, array);
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = SimHashes.BleachStone.CreateTag(),
			massConsumptionRate = 0.00083333335f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array2);
		gameObject.GetComponent<DrowningMonitor>().canDrownToDeath = false;
		gameObject.GetComponent<DrowningMonitor>().livesUnderWater = true;
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, ID + "Seed", STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.NAME, STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.DESC, Assets.GetAnim("seed_sealettuce_kanim"), "object", 0, new List<Tag>
		{
			GameTags.WaterSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 3, STRINGS.CREATURES.SPECIES.SEALETTUCE.DOMESTICATEDDESC);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, ID + "_preview", Assets.GetAnim("sea_lettuce_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
