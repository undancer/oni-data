using TUNING;
using UnityEngine;

public class FarmTileConfig : IBuildingConfig
{
	public const string ID = "FarmTile";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FarmTile", 1, 1, "farmtilerotating_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.FARMABLE, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.isSolidTile = false;
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.occupyingObjectRelativePosition = new Vector3(0f, 1f, 0f);
		plantablePlot.AddDepositTag(GameTags.CropSeed);
		plantablePlot.AddDepositTag(GameTags.WaterSeed);
		plantablePlot.SetFertilizationFlags(fertilizer: true, liquid_piping: false);
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.Farm;
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
		go.GetComponent<KPrefabID>().AddTag(GameTags.FarmTiles);
		SetUpFarmPlotTags(go);
	}

	public static void SetUpFarmPlotTags(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			Rotatable component2 = inst.GetComponent<Rotatable>();
			PlantablePlot component3 = inst.GetComponent<PlantablePlot>();
			switch (component2.GetOrientation())
			{
			case Orientation.Neutral:
			case Orientation.FlipH:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);
				break;
			case Orientation.R180:
			case Orientation.FlipV:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Bottom);
				break;
			case Orientation.R90:
			case Orientation.R270:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Side);
				break;
			case Orientation.NumRotations:
				break;
			}
		};
	}
}
