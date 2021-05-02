using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BeanPlantConfig : IEntityConfig
{
	public const string ID = "BeanPlant";

	public const string SEED_ID = "BeanPlantSeed";

	public const float FERTILIZATION_RATE = 0.008333334f;

	public const float WATER_RATE = 71f / (678f * (float)Math.PI);

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BeanPlant", STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME, STRINGS.CREATURES.SPECIES.BEAN_PLANT.DESC, 2f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("beanplant_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: 258.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 198.15f, 248.15f, 273.15f, 323.15f, null, pressure_sensitive: true, 0f, 0.15f, "BeanPlantSeed", can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, 2400f, "BeanPlantOriginal", STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME);
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = SimHashes.Ethanol.CreateTag(),
			massConsumptionRate = 71f / (678f * (float)Math.PI)
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, array);
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = SimHashes.Dirt.CreateTag(),
			massConsumptionRate = 0.008333334f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array2);
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		pressureVulnerable.Configure(0.025f, 0f, 10f, 30f, new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		});
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "BeanPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.DESC, Assets.GetAnim("seed_beanplant_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.BEAN_PLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.3f);
		EntityTemplates.ExtendEntityToFood(gameObject2, FOOD.FOOD_TYPES.BEAN);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject2, "BeanPlant_preview", Assets.GetAnim("beanplant_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
