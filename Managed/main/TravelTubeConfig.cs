using TUNING;
using UnityEngine;

public class TravelTubeConfig : IBuildingConfig
{
	public const string ID = "TravelTube";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("TravelTube", 1, 1, "travel_tube_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.PLASTICS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.TileLayer = ObjectLayer.TravelTubeTile;
		obj.ReplacementLayer = ObjectLayer.ReplacementTravelTube;
		obj.AudioCategory = "Plastic";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = 0f;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.isKAnimTile = true;
		obj.isUtility = true;
		obj.DragBuild = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<TravelTube>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Tube;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Tube;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
	}
}
