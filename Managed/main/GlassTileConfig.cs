using TUNING;
using UnityEngine;

public class GlassTileConfig : IBuildingConfig
{
	public const string ID = "GlassTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_glass_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GlassTile", 1, 1, "floor_glass_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.TRANSPARENTS, 800f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.UseStructureTemperature = false;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.GlassTile;
		buildingDef.isKAnimTile = true;
		buildingDef.isSolidTile = true;
		buildingDef.BlockTileIsTransparent = true;
		buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_glass");
		buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_glass_place");
		buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_info");
		buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_place_info");
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.setTransparent = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		KAnimGridTileVisualizer kAnimGridTileVisualizer = go.AddOrGet<KAnimGridTileVisualizer>();
		kAnimGridTileVisualizer.blockTileConnectorID = BlockTileConnectorID;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		buildingHP.destroyOnDamaged = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Window);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
		go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<KAnimGridTileVisualizer>();
	}
}
