using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicCounterConfig : IBuildingConfig
{
	public static string ID = "LogicCounter";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 3, "logic_counter_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.PermittedRotations = PermittedRotations.FlipV;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.ObjectLayer = ObjectLayer.LogicGate;
		obj.SceneLayer = Grid.SceneLayer.LogicGates;
		obj.AlwaysOperational = true;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicCounter.INPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.INPUT_PORT_INACTIVE, show_wire_missing_icon: true),
			new LogicPorts.Port(LogicCounter.RESET_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT_RESET, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.RESET_PORT_INACTIVE, show_wire_missing_icon: false, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		obj.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicCounter.OUTPUT_PORT_ID, new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICCOUNTER.OUTPUT_PORT_INACTIVE)
		};
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicCounter>().manuallyControlled = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		go.GetComponent<Switch>().defaultState = false;
	}
}
