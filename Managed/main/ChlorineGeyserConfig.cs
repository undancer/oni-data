using STRINGS;
using TUNING;
using UnityEngine;

public class ChlorineGeyserConfig : IEntityConfig
{
	public const string ID = "ChlorineGeyser";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("ChlorineGeyser", STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.NAME, STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise: NOISE_POLLUTION.NOISY.TIER5, anim: Assets.GetAnim("geyser_side_chlorine_kanim"), initialAnim: "inactive", sceneLayer: Grid.SceneLayer.BuildingBack, width: 4, height: 2);
		obj.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		obj.AddOrGet<Geyser>().outputOffset = new Vector2I(0, 1);
		GeyserConfigurator geyserConfigurator = obj.AddOrGet<GeyserConfigurator>();
		geyserConfigurator.presetType = "chlorine_gas";
		geyserConfigurator.presetMin = 0.35f;
		geyserConfigurator.presetMax = 0.65f;
		Studyable studyable = obj.AddOrGet<Studyable>();
		studyable.meterTrackerSymbol = "geotracker_target";
		studyable.meterAnim = "tracker";
		obj.AddOrGet<LoopingSounds>();
		SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER6);
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
