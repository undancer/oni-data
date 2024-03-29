using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LiquidConduitRadiantConfig : IBuildingConfig
{
	public const string ID = "LiquidConduitRadiant";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidConduitRadiant", 1, 1, "utilities_liquid_radiant_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.REFINED_METALS, 3200f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.ThermalConductivity = 2f;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.ObjectLayer = ObjectLayer.LiquidConduit;
		obj.TileLayer = ObjectLayer.LiquidConduitTile;
		obj.ReplacementLayer = ObjectLayer.ReplacementLiquidConduit;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.SceneLayer = Grid.SceneLayer.LiquidConduits;
		obj.isKAnimTile = true;
		obj.isUtility = true;
		obj.DragBuild = true;
		obj.ReplacementTags = new List<Tag>();
		obj.ReplacementTags.Add(GameTags.Pipes);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidConduitRadiant");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<Conduit>().type = ConduitType.Liquid;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		go.AddComponent<EmptyConduitWorkable>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Pipes);
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}
}
