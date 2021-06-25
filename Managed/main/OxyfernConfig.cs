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

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreatePlacedEntity("Oxyfern", STRINGS.CREATURES.SPECIES.OXYFERN.NAME, STRINGS.CREATURES.SPECIES.OXYFERN.DESC, 1f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("oxy_fern_kanim"), initialAnim: "idle_full", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2);
		template = EntityTemplates.ExtendEntityToBasicPlant(template, 253.15f, 273.15f, 313.15f, 373.15f, new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, 0f, 0.025f, null, can_drown: true, can_tinker: false, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 220f, "OxyfernOriginal", STRINGS.CREATURES.SPECIES.OXYFERN.NAME);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		GameObject template2 = template;
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = tag,
			massConsumptionRate = 19f / 600f
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(template2, array);
		GameObject template3 = template;
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = GameTags.Dirt,
			massConsumptionRate = 0.006666667f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(template3, array2);
		template.AddOrGet<Oxyfern>();
		template.AddOrGet<LoopingSounds>();
		Storage storage = template.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = template.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.storage = storage;
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.EnableConsumption(enabled: true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.00015625001f;
		ElementConverter elementConverter = template.AddOrGet<ElementConverter>();
		elementConverter.OutputMultiplier = 50f;
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.ToString().ToTag(), 0.00062500004f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.031250004f, SimHashes.Oxygen, 0f, useEntityTemperature: true, storeOutput: false, 0f, 1f, 0.75f)
		};
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, SeedProducer.ProductionType.Hidden, "OxyfernSeed", STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.NAME, STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.DESC, Assets.GetAnim("seed_oxyfern_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 20, STRINGS.CREATURES.SPECIES.OXYFERN.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "Oxyfern_preview", Assets.GetAnim("oxy_fern_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		return template;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<Oxyfern>().SetConsumptionRate();
	}
}
