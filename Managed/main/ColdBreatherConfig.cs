using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdBreatherConfig : IEntityConfig
{
	public const string ID = "ColdBreather";

	public static readonly Tag TAG = TagManager.Create("ColdBreather");

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const SimHashes FERTILIZER = SimHashes.Phosphorite;

	public const float TEMP_DELTA = -5f;

	public const float CONSUMPTION_RATE = 1f;

	public const float RADIATION_RATE = 5f;

	public const string SEED_ID = "ColdBreatherSeed";

	public static readonly Tag SEED_TAG = TagManager.Create("ColdBreatherSeed");

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ColdBreather", STRINGS.CREATURES.SPECIES.COLDBREATHER.NAME, STRINGS.CREATURES.SPECIES.COLDBREATHER.DESC, 400f, decor: DECOR.BONUS.TIER1, noise: NOISE_POLLUTION.NOISY.TIER2, anim: Assets.GetAnim("coldbreather_kanim"), initialAnim: "grow_seed", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2);
		gameObject.AddOrGet<ReceptacleMonitor>();
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<WiltCondition>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<DrowningMonitor>();
		gameObject.AddOrGet<MutantPlant>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Phosphorite.CreateTag(),
				massConsumptionRate = 0.006666667f
			}
		});
		TemperatureVulnerable temperatureVulnerable = gameObject.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(213.15f, 183.15f, 368.15f, 463.15f);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		ColdBreather coldBreather = gameObject.AddOrGet<ColdBreather>();
		coldBreather.deltaEmitTemperature = -5f;
		coldBreather.emitOffsetCell = new Vector3(0f, 1f);
		coldBreather.consumptionRate = 1f;
		coldBreather.radiationRate = 5f;
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		Storage storage = BuildingTemplates.CreateDefaultStorage(gameObject);
		storage.showInUI = false;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.storeOnConsume = true;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.capacityKG = 2f;
		elementConsumer.consumptionRate = 0.25f;
		elementConsumer.consumptionRadius = 1;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		component.SurfaceArea = 10f;
		component.Thickness = 0.001f;
		RadiationEmitter radiationEmitter = gameObject.AddComponent<RadiationEmitter>();
		radiationEmitter.emitRadiusX = 2;
		radiationEmitter.emitRadiusY = 2;
		radiationEmitter.emitRads = 5f;
		radiationEmitter.emitRate = 7f;
		radiationEmitter.emissionOffset = new Vector3(0f, 1f, 0f);
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "ColdBreatherSeed", STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.NAME, STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.DESC, Assets.GetAnim("seed_coldbreather_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.COLDBREATHER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "ColdBreather_preview", Assets.GetAnim("coldbreather_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_intake", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
