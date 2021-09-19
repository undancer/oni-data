using TUNING;
using UnityEngine;

public class MetalTileConfig : IBuildingConfig
{
	public const string ID = "MetalTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_metal_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MetalTile", 1, 1, "floor_metal_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER2);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.isKAnimTile = true;
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_metal");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_metal_place");
		obj.BlockTileShineAtlas = Assets.GetTextureAtlas("tiles_metal_spec");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_place_info");
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_3;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
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
