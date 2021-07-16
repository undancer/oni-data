using TUNING;
using UnityEngine;

public class FlowerVaseHangingConfig : IBuildingConfig
{
	public const string ID = "FlowerVaseHanging";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("FlowerVaseHanging", 1, 2, "flowervase_hanging_basic_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "large";
		obj.GenerateOffsets(1, 1);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		plantablePlot.occupyingObjectVisualOffset = new Vector3(0f, -0.25f, 0f);
		go.AddOrGet<FlowerVase>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
