using TUNING;
using UnityEngine;

public class TilePOIConfig : IBuildingConfig
{
	public static string ID = "TilePOI";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "floor_mesh_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_MINERALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.Repairable = false;
		obj.Replaceable = false;
		obj.Invincible = true;
		obj.IsFoundation = true;
		obj.UseStructureTemperature = false;
		obj.TileLayer = ObjectLayer.FoundationTile;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		obj.isKAnimTile = true;
		obj.DebugOnly = true;
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_POI");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_POI");
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_POI_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_POI_tops_decor_info");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<SimCellOccupier>().doReplaceElement = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<KAnimGridTileVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
		go.AddComponent<SimTemperatureTransfer>();
		go.GetComponent<Deconstructable>().allowDeconstruction = true;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<KAnimGridTileVisualizer>();
	}
}
