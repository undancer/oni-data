using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class WormPlantConfig : IEntityConfig
{
	public const string ID = "WormPlant";

	public const string SEED_ID = "WormPlantSeed";

	public const float SULFUR_CONSUMPTION_RATE = 0.016666668f;

	public static readonly EffectorValues BASIC_DECOR = DECOR.PENALTY.TIER0;

	public const string BASIC_CROP_ID = "WormBasicFruit";

	private static StandardCropPlant.AnimSet animSet = new StandardCropPlant.AnimSet
	{
		grow = "basic_grow",
		grow_pst = "basic_grow_pst",
		idle_full = "basic_idle_full",
		wilt_base = "basic_wilt",
		harvest = "basic_harvest"
	};

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public static GameObject BaseWormPlant(string id, string name, string desc, string animFile, EffectorValues decor, string cropID)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 1f, Assets.GetAnim(animFile), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, decor, default(EffectorValues), SimHashes.Creature, null, 307.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 273.15f, 288.15f, 323.15f, 373.15f, crop_id: cropID, safe_elements: new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sulfur.CreateTag(),
				massConsumptionRate = 0.016666668f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = BaseWormPlant("WormPlant", STRINGS.CREATURES.SPECIES.WORMPLANT.NAME, STRINGS.CREATURES.SPECIES.WORMPLANT.DESC, "wormwood_kanim", BASIC_DECOR, "WormBasicFruit");
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "WormPlantSeed", STRINGS.CREATURES.SPECIES.SEEDS.WORMPLANT.NAME, STRINGS.CREATURES.SPECIES.SEEDS.WORMPLANT.DESC, Assets.GetAnim("seed_wormwood_kanim"), "object", 0, new List<Tag>
		{
			GameTags.CropSeed
		}, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, STRINGS.CREATURES.SPECIES.WORMPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "WormPlant_preview", Assets.GetAnim("wormwood_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		TransformingPlant transformingPlant = prefab.AddOrGet<TransformingPlant>();
		transformingPlant.transformPlantId = "SuperWormPlant";
		transformingPlant.SubscribeToTransformEvent(GameHashes.CropTended);
		transformingPlant.useGrowthTimeRatio = true;
		transformingPlant.eventDataCondition = delegate(object data)
		{
			CropTendingStates.CropTendingEventData cropTendingEventData = (CropTendingStates.CropTendingEventData)data;
			if (cropTendingEventData != null)
			{
				CreatureBrain component = cropTendingEventData.source.GetComponent<CreatureBrain>();
				if (component != null && component.species == GameTags.Creatures.Species.DivergentSpecies)
				{
					return true;
				}
			}
			return false;
		};
		transformingPlant.fxKAnim = "plant_transform_fx_kanim";
		transformingPlant.fxAnim = "plant_transform";
		StandardCropPlant standardCropPlant = prefab.AddOrGet<StandardCropPlant>();
		standardCropPlant.anims = animSet;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
