using TUNING;
using UnityEngine;

public class HydroponicFarmConfig : IBuildingConfig
{
	public const string ID = "HydroponicFarm";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HydroponicFarm", 1, 1, "farmtilehydroponicrotating_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		obj.PermittedRotations = PermittedRotations.FlipV;
		obj.InputConduitType = ConduitType.Liquid;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		return obj;
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
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
		BuildingTemplates.CreateDefaultStorage(go).SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
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
