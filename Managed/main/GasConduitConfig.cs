using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GasConduitConfig : IBuildingConfig
{
	public const string ID = "GasConduit";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasConduit", 1, 1, "utilities_gas_kanim", 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.ObjectLayer = ObjectLayer.GasConduit;
		buildingDef.TileLayer = ObjectLayer.GasConduitTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementGasConduit;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = 0f;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.SceneLayer = Grid.SceneLayer.GasConduits;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		buildingDef.ReplacementTags = new List<Tag>();
		buildingDef.ReplacementTags.Add(GameTags.Vents);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
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
