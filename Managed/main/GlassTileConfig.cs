using TUNING;
using UnityEngine;

public class GlassTileConfig : IBuildingConfig
{
	public const string ID = "GlassTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_glass_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GlassTile", 1, 1, "floor_glass_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.TRANSPARENTS, 800f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.GlassTile;
		obj.isKAnimTile = true;
		obj.BlockTileIsTransparent = true;
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_glass");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_glass_place");
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_glass_tops_decor_place_info");
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.setTransparent = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Window);
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
