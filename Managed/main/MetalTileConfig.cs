using TUNING;
using UnityEngine;

public class MetalTileConfig : IBuildingConfig
{
	public const string ID = "MetalTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_metal_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MetalTile", 1, 1, "floor_metal_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER2);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.UseStructureTemperature = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.isKAnimTile = true;
		buildingDef.isSolidTile = true;
		buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_metal");
		buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_metal_place");
		buildingDef.BlockTileShineAtlas = Assets.GetTextureAtlas("tiles_metal_spec");
		buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
		buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_place_info");
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_3;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		KAnimGridTileVisualizer kAnimGridTileVisualizer = go.AddOrGet<KAnimGridTileVisualizer>();
		kAnimGridTileVisualizer.blockTileConnectorID = BlockTileConnectorID;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		buildingHP.destroyOnDamaged = true;
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
