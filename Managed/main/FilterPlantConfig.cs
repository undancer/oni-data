using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FilterPlantConfig : IEntityConfig
{
	public const string ID = "FilterPlant";

	public const string SEED_ID = "FilterPlantSeed";

	public const float SAND_CONSUMPTION_RATE = 0.008333334f;

	public const float WATER_CONSUMPTION_RATE = 13f / 120f;

	public const float OXYGEN_CONSUMPTION_RATE = 0.008333334f;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("FilterPlant", STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME, STRINGS.CREATURES.SPECIES.FILTERPLANT.DESC, 2f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("cactus_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: 348.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 253.15f, 293.15f, 383.15f, 443.15f, null, pressure_sensitive: true, 0f, 0.15f, SimHashes.Water.ToString(), can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, 2400f, "FilterPlantOriginal", STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME);
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = SimHashes.Sand.CreateTag(),
			massConsumptionRate = 0.008333334f
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array);
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = GameTags.DirtyWater,
			massConsumptionRate = 13f / 120f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, array2);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<SaltPlant>();
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		pressureVulnerable.Configure(0.025f, 0f, 10f, 30f, new SimHashes[1]
		{
			SimHashes.Oxygen
		});
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component2 = inst.GetComponent<PressureVulnerable>();
			component2.safe_atmospheres.Add(ElementLoader.FindElementByHash(SimHashes.Oxygen));
		};
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.Oxygen;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.008333334f;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "FilterPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.DESC, Assets.GetAnim("seed_cactus_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.FILTERPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f);
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "FilterPlant_preview", Assets.GetAnim("cactus_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
