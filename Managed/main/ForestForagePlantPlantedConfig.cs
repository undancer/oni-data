using STRINGS;
using TUNING;
using UnityEngine;

public class ForestForagePlantPlantedConfig : IEntityConfig
{
	public const string ID = "ForestForagePlantPlanted";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ForestForagePlantPlanted", STRINGS.CREATURES.SPECIES.FORESTFORAGEPLANTPLANTED.NAME, STRINGS.CREATURES.SPECIES.FORESTFORAGEPLANTPLANTED.DESC, 100f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("podmelon_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2);
		gameObject.AddOrGet<SimTemperatureTransfer>();
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<DrowningMonitor>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		SeedProducer seedProducer = gameObject.AddOrGet<SeedProducer>();
		seedProducer.Configure("ForestForagePlant", SeedProducer.ProductionType.DigOnly);
		gameObject.AddOrGet<BasicForagePlantPlanted>();
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}