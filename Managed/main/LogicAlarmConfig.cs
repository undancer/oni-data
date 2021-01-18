using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicAlarmConfig : IBuildingConfig
{
	public static string ID = "LogicAlarm";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "alarm_sensor_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.AlwaysOperational = true;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicAlarm.INPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICALARM.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICALARM.INPUT_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicAlarm>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
