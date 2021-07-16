using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LiquidConduitConfig : IBuildingConfig
{
	public const string ID = "LiquidConduit";

	public static void CommonConduitPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidConduit", 1, 1, "utilities_liquid_kanim", 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.PLUMBABLE, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
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
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidConduit");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<Conduit>().type = ConduitType.Liquid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		go.AddComponent<EmptyConduitWorkable>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Pipes);
		CommonConduitPostConfigureComplete(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}
}
