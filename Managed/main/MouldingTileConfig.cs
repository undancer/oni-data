using TUNING;
using UnityEngine;

public class MouldingTileConfig : IBuildingConfig
{
	public const string ID = "MouldingTile";

	public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_bunker_tops");

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MouldingTile", 1, 1, "floor_moulding_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 10,
			radius = 1
		});
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.UseStructureTemperature = false;
		obj.IsFoundation = true;
		obj.TileLayer = ObjectLayer.FoundationTile;
		obj.ReplacementLayer = ObjectLayer.ReplacementTile;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		obj.isKAnimTile = true;
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_moulding");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_moulding_place");
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_place_info");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<SimCellOccupier>().doReplaceElement = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<KAnimGridTileVisualizer>();
	}
}
