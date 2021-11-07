using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SaltPlantConfig : IEntityConfig
{
	public const string ID = "SaltPlant";

	public const string SEED_ID = "SaltPlantSeed";

	public const float FERTILIZATION_RATE = 7f / 600f;

	public const float CHLORINE_CONSUMPTION_RATE = 0.006f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SaltPlant", STRINGS.CREATURES.SPECIES.SALTPLANT.NAME, STRINGS.CREATURES.SPECIES.SALTPLANT.DESC, 2f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("saltplant_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: new List<Tag> { GameTags.Hanging }, defaultTemperature: 258.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 198.15f, 248.15f, 323.15f, 393.15f, crop_id: SimHashes.Salt.ToString(), baseTraitName: STRINGS.CREATURES.SPECIES.SALTPLANT.NAME, safe_elements: new SimHashes[1] { SimHashes.ChlorineGas }, pressure_sensitive: true, pressure_lethal_low: 0f, pressure_warning_low: 0.025f, can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, max_age: 2400f, min_radiation: 0f, max_radiation: 740f, baseTraitId: "SaltPlantOriginal");
		gameObject.AddOrGet<SaltPlant>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 7f / 600f
			}
		});
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.ChlorineGas;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, -1f);
		elementConsumer.consumptionRate = 0.006f;
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[1]
		{
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<StandardCropPlant>();
		EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SaltPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.DESC, Assets.GetAnim("seed_saltplant_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 5, STRINGS.CREATURES.SPECIES.SALTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f), "SaltPlant_preview", Assets.GetAnim("saltplant_kanim"), "place", 1, 2), 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<ElementConsumer>().EnableConsumption(enabled: true);
	}
}
