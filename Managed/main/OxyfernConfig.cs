using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class OxyfernConfig : IEntityConfig
{
	public const string ID = "Oxyfern";

	public const string SEED_ID = "OxyfernSeed";

	public const float WATER_CONSUMPTION_RATE = 19f / 600f;

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const float CO2_RATE = 0.00062500004f;

	private const float CONVERSION_RATIO = 50f;

	public const float OXYGEN_RATE = 0.031250004f;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("Oxyfern", STRINGS.CREATURES.SPECIES.OXYFERN.NAME, STRINGS.CREATURES.SPECIES.OXYFERN.DESC, 1f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("oxy_fern_kanim"), initialAnim: "idle_full", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2);
		gameObject.AddOrGet<ReceptacleMonitor>();
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<WiltCondition>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<DrowningMonitor>();
		gameObject.AddOrGet<MutantPlant>();
		TemperatureVulnerable temperatureVulnerable = gameObject.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(273.15f, 253.15f, 313.15f, 373.15f);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = tag,
			massConsumptionRate = 19f / 600f
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, array);
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = GameTags.Dirt,
			massConsumptionRate = 0.006666667f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array2);
		gameObject.AddOrGet<Oxyfern>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		pressureVulnerable.Configure(0.025f, 0f, 10f, 30f, new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		});
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component2 = inst.GetComponent<PressureVulnerable>();
			component2.safe_atmospheres.Add(ElementLoader.FindElementByHash(SimHashes.CarbonDioxide));
		};
		gameObject.AddOrGet<LoopingSounds>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.storage = storage;
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.EnableConsumption(enabled: true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.00015625001f;
		ElementConverter elementConverter = gameObject.AddOrGet<ElementConverter>();
		elementConverter.OutputMultiplier = 50f;
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.ToString().ToTag(), 0.00062500004f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.031250004f, SimHashes.Oxygen, 0f, useEntityTemperature: true, storeOutput: false, 0f, 1f, 0.75f)
		};
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "OxyfernSeed", STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.NAME, STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.DESC, Assets.GetAnim("seed_oxyfern_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.OXYFERN.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "Oxyfern_preview", Assets.GetAnim("oxy_fern_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<Oxyfern>().SetConsumptionRate();
	}
}
