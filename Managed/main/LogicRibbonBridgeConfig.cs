using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicRibbonBridgeConfig : IBuildingConfig
{
	public const string ID = "LogicRibbonBridge";

	public static readonly HashedString BRIDGE_LOGIC_RIBBON_IO_ID = new HashedString("BRIDGE_LOGIC_RIBBON_IO");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LogicRibbonBridge", 3, 1, "logic_ribbon_bridge_kanim", 30, 3f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.LogicBridge, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicGate;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.RibbonInputPort(BRIDGE_LOGIC_RIBBON_IO_ID, new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_INACTIVE),
			LogicPorts.Port.RibbonInputPort(BRIDGE_LOGIC_RIBBON_IO_ID, new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicRibbonBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		go.AddOrGet<LogicRibbonBridge>();
	}

	private LogicUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = go.AddOrGet<LogicUtilityNetworkLink>();
		logicUtilityNetworkLink.bitDepth = LogicWire.BitDepth.FourBit;
		logicUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		logicUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return logicUtilityNetworkLink;
	}
}
