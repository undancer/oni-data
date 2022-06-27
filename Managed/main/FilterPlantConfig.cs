using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FilterPlantConfig : IEntityConfig
{
	public const string ID = "FilterPlant";

	public const string SEED_ID = "FilterPlantSeed";

	public const float SAND_CONSUMPTION_RATE = 1f / 120f;

	public const float WATER_CONSUMPTION_RATE = 13f / 120f;

	public const float OXYGEN_CONSUMPTION_RATE = 1f / 120f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("FilterPlant", STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME, STRINGS.CREATURES.SPECIES.FILTERPLANT.DESC, 2f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("cactus_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: 348.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 253.15f, 293.15f, 383.15f, 443.15f, crop_id: SimHashes.Water.ToString(), baseTraitName: STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME, safe_elements: new SimHashes[1] { SimHashes.Oxygen }, pressure_sensitive: true, pressure_lethal_low: 0f, pressure_warning_low: 0.025f, can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, max_age: 2400f, min_radiation: 0f, max_radiation: 2200f, baseTraitId: "FilterPlantOriginal");
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 1f / 120f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 13f / 120f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<SaltPlant>();
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.Oxygen;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 1f / 120f;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "FilterPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.DESC, Assets.GetAnim("seed_cactus_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 21, STRINGS.CREATURES.SPECIES.FILTERPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f), "FilterPlant_preview", Assets.GetAnim("cactus_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
