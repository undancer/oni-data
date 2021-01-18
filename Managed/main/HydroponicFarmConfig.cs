using TUNING;
using UnityEngine;

public class HydroponicFarmConfig : IBuildingConfig
{
	public const string ID = "HydroponicFarm";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HydroponicFarm", 1, 1, "farmtilehydroponicrotating_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.UseStructureTemperature = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		buildingDef.isSolidTile = true;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.CropSeed);
		plantablePlot.AddDepositTag(GameTags.WaterSeed);
		plantablePlot.occupyingObjectRelativePosition.y = 1f;
		plantablePlot.SetFertilizationFlags(fertilizer: true, liquid_piping: true);
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.Farm;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<PlanterBox>();
		go.AddOrGet<AnimTileable>();
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		FarmTileConfig.SetUpFarmPlotTags(go);
		go.GetComponent<KPrefabID>().AddTag(GameTags.FarmTiles);
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}
}
