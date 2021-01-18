using TUNING;
using UnityEngine;

public class ClusterTelescopeConfig : IBuildingConfig
{
	public const string ID = "ClusterTelescope";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ClusterTelescope", 3, 3, "telescope_low_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		ClusterTelescope.Def def = go.AddOrGetDef<ClusterTelescope.Def>();
		def.clearScanCellRadius = 5;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		go.AddOrGetDef<PoweredController.Def>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
