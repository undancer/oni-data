using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CritterTrapPlantConfig : IEntityConfig
{
	public const string ID = "CritterTrapPlant";

	public const float WATER_RATE = 0.016666668f;

	public const float GAS_RATE = 0.041666668f;

	public const float GAS_VENT_THRESHOLD = 33.25f;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("CritterTrapPlant", STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.NAME, STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.DESC, 4f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("venus_critter_trap_kanim"), initialAnim: "idle_open", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, TUNING.CREATURES.TEMPERATURE.FREEZING_10, TUNING.CREATURES.TEMPERATURE.FREEZING_9, TUNING.CREATURES.TEMPERATURE.FREEZING, TUNING.CREATURES.TEMPERATURE.COOL, null, pressure_sensitive: false, 0f, 0.15f, "PlantMeat", can_drown: true, can_tinker: true, require_solid_tile: true, should_grow_old: false, 2400f, 0f, 220f, "CritterTrapPlantOriginal", STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.NAME);
		Object.DestroyImmediate(gameObject.GetComponent<MutantPlant>());
		TrapTrigger trapTrigger = gameObject.AddOrGet<TrapTrigger>();
		trapTrigger.trappableCreatures = new Tag[2]
		{
			GameTags.Creatures.Walker,
			GameTags.Creatures.Hoverer
		};
		trapTrigger.trappedOffset = new Vector2(0.5f, 0f);
		trapTrigger.enabled = false;
		CritterTrapPlant critterTrapPlant = gameObject.AddOrGet<CritterTrapPlant>();
		critterTrapPlant.gasOutputRate = 0.041666668f;
		critterTrapPlant.outputElement = SimHashes.Hydrogen;
		critterTrapPlant.gasVentThreshold = 33.25f;
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Storage>();
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.016666668f
			}
		});
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "CritterTrapPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.CRITTERTRAPPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.CRITTERTRAPPLANT.DESC, Assets.GetAnim("seed_critter_trap_kanim"), "object", 1, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 21, STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "CritterTrapPlant_preview", Assets.GetAnim("venus_critter_trap_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
