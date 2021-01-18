using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceVineConfig : IEntityConfig
{
	public const string ID = "SpiceVine";

	public const string SEED_ID = "SpiceVineSeed";

	public const float FERTILIZATION_RATE = 0.0016666667f;

	public const float WATER_RATE = 7f / 120f;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SpiceVine", STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME, STRINGS.CREATURES.SPECIES.SPICE_VINE.DESC, 2f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("vinespicenut_kanim"), initialAnim: "idle_empty", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 3, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Hanging
		}, defaultTemperature: 320f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 308.15f, 358.15f, 448.15f, null, pressure_sensitive: true, 0f, 0.15f, SpiceNutConfig.ID);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[1];
		PlantElementAbsorber.ConsumeInfo consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = tag,
			massConsumptionRate = 7f / 120f
		};
		array[0] = consumeInfo;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, array);
		PlantElementAbsorber.ConsumeInfo[] array2 = new PlantElementAbsorber.ConsumeInfo[1];
		consumeInfo = new PlantElementAbsorber.ConsumeInfo
		{
			tag = GameTags.Phosphorite,
			massConsumptionRate = 0.0016666667f
		};
		array2[0] = consumeInfo;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array2);
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, 1);
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SpiceVineSeed", STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME, STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC, Assets.GetAnim("seed_spicenut_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		GameObject template = EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3);
		EntityTemplates.MakeHangingOffsets(template, 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
