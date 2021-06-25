using TUNING;
using UnityEngine;

public class ArtifactAnalysisStationConfig : IBuildingConfig
{
	public const string ID = "ArtifactAnalysisStation";

	public const float WORK_TIME = 150f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ArtifactAnalysisStation", 4, 4, "artifact_analysis_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.Deprecated = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGetDef<ArtifactAnalysisStation.Def>();
		ArtifactAnalysisStationWorkable artifactAnalysisStationWorkable = go.AddOrGet<ArtifactAnalysisStationWorkable>();
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = GameTags.CharmedArtifact;
		manualDeliveryKG.refillMass = 1.1f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.capacity = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}
}
