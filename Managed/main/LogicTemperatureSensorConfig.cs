using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicTemperatureSensorConfig : IBuildingConfig
{
	public static string ID = "LogicTemperatureSensor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchthermal_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>();
		buildingDef.LogicOutputPorts.Add(LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICTEMPERATURESENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICTEMPERATURESENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICTEMPERATURESENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true));
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicTemperatureSensor logicTemperatureSensor = go.AddOrGet<LogicTemperatureSensor>();
		logicTemperatureSensor.manuallyControlled = false;
		logicTemperatureSensor.minTemp = 0f;
		logicTemperatureSensor.maxTemp = 9999f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
