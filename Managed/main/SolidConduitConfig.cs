using TUNING;
using UnityEngine;

public class SolidConduitConfig : IBuildingConfig
{
	public const string ID = "SolidConduit";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidConduit", 1, 1, "utilities_conveyor_kanim", 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.ObjectLayer = ObjectLayer.SolidConduit;
		obj.TileLayer = ObjectLayer.SolidConduitTile;
		obj.ReplacementLayer = ObjectLayer.ReplacementSolidConduit;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = 0f;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.SceneLayer = Grid.SceneLayer.SolidConduits;
		obj.isKAnimTile = true;
		obj.isUtility = true;
		obj.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduit");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<SolidConduit>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}
}
