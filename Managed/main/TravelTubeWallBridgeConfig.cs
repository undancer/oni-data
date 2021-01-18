using TUNING;
using UnityEngine;

public class TravelTubeWallBridgeConfig : IBuildingConfig
{
	public const string ID = "TravelTubeWallBridge";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TravelTubeWallBridge", 1, 1, "tube_tile_bridge_kanim", 100, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.PLASTICS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.AudioCategory = "Plastic";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_3;
		simCellOccupier.notifyOnMelt = true;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		buildingHP.destroyOnDamaged = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<TravelTubeBridge>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = AddNetworkLink(go);
		travelTubeUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = AddNetworkLink(go);
		travelTubeUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = AddNetworkLink(go);
		travelTubeUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		go.AddOrGet<KPrefabID>().AddTag(GameTags.TravelTubeBridges);
	}

	protected virtual TravelTubeUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = go.AddOrGet<TravelTubeUtilityNetworkLink>();
		travelTubeUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		travelTubeUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return travelTubeUtilityNetworkLink;
	}
}
