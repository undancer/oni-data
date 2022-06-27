using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MushroomPlantConfig : IEntityConfig
{
	public const float FERTILIZATION_RATE = 1f / 150f;

	public const string ID = "MushroomPlant";

	public const string SEED_ID = "MushroomSeed";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MushroomPlant", STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.NAME, STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DESC, 1f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("fungusplant_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 2);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 228.15f, 278.15f, 308.15f, 398.15f, new SimHashes[1] { SimHashes.CarbonDioxide }, pressure_sensitive: true, 0f, 0.15f, MushroomConfig.ID, can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: true, 2400f, 0f, 4600f, "MushroomPlantOriginal", STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.NAME);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.SlimeMold,
				massConsumptionRate = 1f / 150f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(prefersDarkness: true);
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "MushroomSeed", STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.DESC, Assets.GetAnim("seed_fungusplant_kanim"), "object", 0, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 3, STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.33f, 0.33f), "MushroomPlant_preview", Assets.GetAnim("fungusplant_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
