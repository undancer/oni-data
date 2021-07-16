using TUNING;
using UnityEngine;

public class GasPermeableMembraneConfig : IBuildingConfig
{
	public const string ID = "GasPermeableMembrane";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasPermeableMembrane", 1, 1, "floor_gasperm_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.isKAnimTile = true;
		obj.BlockTileAtlas = Assets.GetTextureAtlas("tiles_gasmembrane");
		obj.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_gasmembrane_place");
		obj.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
		obj.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_mesh_tops_decor_info");
		obj.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_mesh_tops_decor_place_info");
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.setLiquidImpermeable = true;
		simCellOccupier.doReplaceElement = false;
		go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = MeshTileConfig.BlockTileConnectorID;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		go.AddComponent<SimTemperatureTransfer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddComponent<ZoneTile>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<KAnimGridTileVisualizer>();
	}
}
