using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SwampHarvestPlantConfig : IEntityConfig
{
	public const string ID = "SwampHarvestPlant";

	public const string SEED_ID = "SwampHarvestPlantSeed";

	public const float WATER_RATE = 71f / (339f * (float)Math.PI);

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SwampHarvestPlant", STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.NAME, STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.DESC, 1f, decor: DECOR.PENALTY.TIER1, anim: Assets.GetAnim("swampcrop_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 283.15f, 303.15f, 398.15f, crop_id: SwampFruitConfig.ID, safe_elements: new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, pressure_sensitive: true, pressure_lethal_low: 0f, pressure_warning_low: 0.15f, can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, max_age: 2400f, min_radiation: 0f, max_radiation: 460f, baseTraitId: "SwampHarvestPlantOriginal", baseTraitName: gameObject.PrefabID().Name);
		IlluminationVulnerable illuminationVulnerable = gameObject.AddOrGet<IlluminationVulnerable>();
		illuminationVulnerable.SetPrefersDarkness(prefersDarkness: true);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 71f / (339f * (float)Math.PI)
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SwampHarvestPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.SWAMPHARVESTPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.SWAMPHARVESTPLANT.DESC, Assets.GetAnim("seed_swampcrop_kanim"), "object", 0, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "SwampHarvestPlant_preview", Assets.GetAnim("swampcrop_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
