using TUNING;
using UnityEngine;

public class CarpetTileConfig : IBuildingConfig
{
	public const string ID = "CarpetTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_carpet_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("CarpetTile", 1, 1, "floor_carpet_kanim", 100, 30f, new float[2] { 200f, 2f }, new string[2] { "BuildableRaw", "BuildingFiber" }, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER3);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.UseStructureTemperature = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		obj.isKAnimTile = true;
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_carpet");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_carpet_place");
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_carpet_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_carpet_tops_decor_place_info");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_2;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
		go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
		go.GetComponent<KPrefabID>().AddTag(GameTags.Carpeted);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<KAnimGridTileVisualizer>();
	}
}
