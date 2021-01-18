using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GasConduitRadiantConfig : IBuildingConfig
{
	public const string ID = "GasConduitRadiant";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasConduitRadiant", 1, 1, "utilities_gas_radiant_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.ThermalConductivity = 2f;
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.ObjectLayer = ObjectLayer.GasConduit;
		obj.TileLayer = ObjectLayer.GasConduitTile;
		obj.ReplacementLayer = ObjectLayer.ReplacementGasConduit;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = 0f;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.SceneLayer = Grid.SceneLayer.GasConduits;
		obj.isKAnimTile = true;
		obj.isUtility = true;
		obj.DragBuild = true;
		obj.ReplacementTags = new List<Tag>();
		obj.ReplacementTags.Add(GameTags.Vents);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasConduitRadiant");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<Conduit>().type = ConduitType.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		go.AddComponent<EmptyConduitWorkable>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Vents);
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}
}
