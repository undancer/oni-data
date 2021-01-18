using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicCounterConfig : IBuildingConfig
{
	public static string ID = "LogicCounter";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 3, "logic_counter_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ObjectLayer = ObjectLayer.LogicGate;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicCounter.INPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.INPUT_PORT_INACTIVE, show_wire_missing_icon: true),
			new LogicPorts.Port(LogicCounter.RESET_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT_RESET, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.RESET_PORT_INACTIVE, show_wire_missing_icon: false, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicCounter.OUTPUT_PORT_ID, new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.OUTPUT_PORT_INACTIVE)
		};
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicCounter logicCounter = go.AddOrGet<LogicCounter>();
		logicCounter.manuallyControlled = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		Switch component = go.GetComponent<Switch>();
		component.defaultState = false;
	}
}
