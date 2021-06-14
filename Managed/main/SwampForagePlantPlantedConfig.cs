using STRINGS;
using TUNING;
using UnityEngine;

public class SwampForagePlantPlantedConfig : IEntityConfig
{
	public const string ID = "SwampForagePlantPlanted";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SwampForagePlantPlanted", STRINGS.CREATURES.SPECIES.SWAMPFORAGEPLANTPLANTED.NAME, STRINGS.CREATURES.SPECIES.SWAMPFORAGEPLANTPLANTED.DESC, 100f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("swamptuber_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.BuildingBack, width: 1, height: 2);
		gameObject.AddOrGet<SimTemperatureTransfer>();
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		SeedProducer seedProducer = gameObject.AddOrGet<SeedProducer>();
		seedProducer.Configure("SwampForagePlant", SeedProducer.ProductionType.DigOnly);
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
