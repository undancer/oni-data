using TUNING;
using UnityEngine;

public class FlowerVaseHangingFancyConfig : IBuildingConfig
{
	public const string ID = "FlowerVaseHangingFancy";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FlowerVaseHangingFancy", 1, 2, "flowervase_hanging_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.TRANSPARENTS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = BUILDINGS.DECOR.BONUS.TIER1.amount,
			radius = BUILDINGS.DECOR.BONUS.TIER3.radius
		});
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		buildingDef.GenerateOffsets(1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		plantablePlot.plantLayer = Grid.SceneLayer.BuildingFront;
		plantablePlot.occupyingObjectVisualOffset = new Vector3(0f, -0.45f, 0f);
		go.AddOrGet<FlowerVase>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
