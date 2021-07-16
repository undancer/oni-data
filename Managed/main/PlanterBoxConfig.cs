using TUNING;
using UnityEngine;

public class PlanterBoxConfig : IBuildingConfig
{
	public const string ID = "PlanterBox";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("PlanterBox", 1, 1, "planterbox_kanim", 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.FARMABLE, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.CropSeed);
		plantablePlot.SetFertilizationFlags(fertilizer: true, liquid_piping: false);
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
		BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<PlanterBox>();
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
