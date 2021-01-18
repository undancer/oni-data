using STRINGS;
using TUNING;
using UnityEngine;

public class ChlorineGeyserConfig : IEntityConfig
{
	public const string ID = "ChlorineGeyser";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ChlorineGeyser", STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.NAME, STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise: NOISE_POLLUTION.NOISY.TIER5, anim: Assets.GetAnim("geyser_side_chlorine_kanim"), initialAnim: "inactive", sceneLayer: Grid.SceneLayer.BuildingBack, width: 4, height: 2);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		Geyser geyser = gameObject.AddOrGet<Geyser>();
		geyser.outputOffset = new Vector2I(0, 1);
		GeyserConfigurator geyserConfigurator = gameObject.AddOrGet<GeyserConfigurator>();
		geyserConfigurator.presetType = "chlorine_gas";
		geyserConfigurator.presetMin = 0.35f;
		geyserConfigurator.presetMax = 0.65f;
		Studyable studyable = gameObject.AddOrGet<Studyable>();
		studyable.meterTrackerSymbol = "geotracker_target";
		studyable.meterAnim = "tracker";
		gameObject.AddOrGet<LoopingSounds>();
		SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
