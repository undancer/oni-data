using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicMemoryConfig : IBuildingConfig
{
	public static string ID = "LogicMemory";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "logic_memory_kanim", 10, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.NONE);
		buildingDef.Deprecated = false;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.InitialOrientation = Orientation.R90;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.ObjectLayer = ObjectLayer.LogicGate;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LogicMemory.SET_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_INACTIVE, show_wire_missing_icon: true, LogicPortSpriteType.Input, display_custom_name: true),
			new LogicPorts.Port(LogicMemory.RESET_PORT_ID, new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_INACTIVE, show_wire_missing_icon: true, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LogicMemory.READ_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_INACTIVE, show_wire_missing_icon: true, LogicPortSpriteType.Output, display_custom_name: true)
		};
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicMemory>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
