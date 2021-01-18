using TUNING;
using UnityEngine;

public class FlowerVaseConfig : IBuildingConfig
{
	public const string ID = "FlowerVase";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("FlowerVase", 1, 1, "flowervase_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		go.AddOrGet<PlantablePlot>().AddDepositTag(GameTags.DecorSeed);
		go.AddOrGet<FlowerVase>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
