using TUNING;
using UnityEngine;

public class GeneticAnalysisStationConfig : IBuildingConfig
{
	public const string ID = "GeneticAnalysisStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeneticAnalysisStation", 7, 2, "genetic_analysisstation_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.Deprecated = !DlcManager.FeaturePlantMutationsEnabled();
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGetDef<GeneticAnalysisStation.Def>();
		go.AddOrGet<GeneticAnalysisStationWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		Storage storage = go.AddOrGet<Storage>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = Tag.Invalid;
		manualDeliveryKG.refillMass = 1f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.capacity = 3f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
