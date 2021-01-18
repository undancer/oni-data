using STRINGS;
using TUNING;
using UnityEngine;

public class GeyserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("Geyser", STRINGS.CREATURES.SPECIES.GEYSER.NAME, STRINGS.CREATURES.SPECIES.GEYSER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise: NOISE_POLLUTION.NOISY.TIER6, anim: Assets.GetAnim("geyser_side_steam_kanim"), initialAnim: "inactive", sceneLayer: Grid.SceneLayer.BuildingBack, width: 4, height: 2);
		obj.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		obj.AddOrGet<Geyser>().outputOffset = new Vector2I(0, 1);
		GeyserConfigurator geyserConfigurator = obj.AddOrGet<GeyserConfigurator>();
		geyserConfigurator.presetType = "steam";
		geyserConfigurator.presetMin = 0.5f;
		geyserConfigurator.presetMax = 0.75f;
		Studyable studyable = obj.AddOrGet<Studyable>();
		studyable.meterTrackerSymbol = "geotracker_target";
		studyable.meterAnim = "tracker";
		obj.AddOrGet<LoopingSounds>();
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_steam_kanim", "Geyser_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
