using TUNING;
using UnityEngine;

public class TemperatureControlledSwitchConfig : IBuildingConfig
{
	public static string ID = "TemperatureControlledSwitch";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchthermal_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Deprecated = true;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		TemperatureControlledSwitch temperatureControlledSwitch = go.AddOrGet<TemperatureControlledSwitch>();
		temperatureControlledSwitch.objectLayer = ObjectLayer.Wire;
		temperatureControlledSwitch.manuallyControlled = false;
		temperatureControlledSwitch.minTemp = 0f;
		temperatureControlledSwitch.maxTemp = 573.15f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddComponent<BuildingCellVisualizer>();
	}
}
