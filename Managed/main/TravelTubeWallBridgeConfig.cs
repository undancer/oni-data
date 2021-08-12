using TUNING;
using UnityEngine;

public class TravelTubeWallBridgeConfig : IBuildingConfig
{
	public const string ID = "TravelTubeWallBridge";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("TravelTubeWallBridge", 1, 1, "tube_tile_bridge_kanim", 100, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.PLASTICS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.AudioCategory = "Plastic";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R90;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 2);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.ForegroundLayer = Grid.SceneLayer.TileMain;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_3;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<TravelTubeBridge>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AddNetworkLink(go).visualizeOnly = false;
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
