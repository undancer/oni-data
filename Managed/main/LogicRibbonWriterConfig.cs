using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicRibbonWriterConfig : IBuildingConfig
{
	public static string ID = "LogicRibbonWriter";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 2, 1, "logic_ribbon_writer_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.ObjectLayer = ObjectLayer.LogicGate;
		obj.SceneLayer = Grid.SceneLayer.LogicGates;
		obj.AlwaysOperational = true;
		obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(LogicRibbonWriter.INPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.INPUT_PORT_INACTIVE, show_wire_missing_icon: true) };
		obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(LogicRibbonWriter.OUTPUT_PORT_ID, new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONWRITER.OUTPUT_PORT_INACTIVE, show_wire_missing_icon: true) };
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicRibbonWriter>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
