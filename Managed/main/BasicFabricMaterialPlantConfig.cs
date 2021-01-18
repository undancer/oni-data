using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicFabricMaterialPlantConfig : IEntityConfig
{
	public static string ID = "BasicFabricPlant";

	public static string SEED_ID = "BasicFabricMaterialPlantSeed";

	public const float WATER_RATE = 4f / 15f;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME, STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DESC, 1f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim("swampreed_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 248.15f, 295.15f, 310.15f, 398.15f, crop_id: BasicFabricConfig.ID, safe_elements: new SimHashes[5]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.DirtyWater,
			SimHashes.Water
		}, pressure_sensitive: false, pressure_lethal_low: 0f, pressure_warning_low: 0.15f, can_drown: false);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 4f / 15f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, SEED_ID, STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.DESC, Assets.GetAnim("seed_swampreed_kanim"), "object", 0, new List<Tag>
		{
			GameTags.WaterSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DOMESTICATEDDESC), ID + "_preview", Assets.GetAnim("swampreed_kanim"), "place", 1, 3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
