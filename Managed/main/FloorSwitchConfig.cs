using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FloorSwitchConfig : IBuildingConfig
{
	public const string ID = "FloorSwitch";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FloorSwitch", 1, 1, "pressureswitch_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.IsFoundation = true;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		buildingDef.AlwaysOperational = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "FloorSwitch");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_2;
		simCellOccupier.notifyOnMelt = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicMassSensor logicMassSensor = go.AddOrGet<LogicMassSensor>();
		logicMassSensor.rangeMin = 0f;
		logicMassSensor.rangeMax = 2000f;
		logicMassSensor.Threshold = 10f;
		logicMassSensor.ActivateAboveThreshold = true;
		logicMassSensor.manuallyControlled = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
