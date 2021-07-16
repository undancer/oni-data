using TUNING;
using UnityEngine;

public class RadiationLightConfig : IBuildingConfig
{
	public const string ID = "RadiationLight";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RadiationLight", 1, 1, "ceilinglight_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 10f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Radiator radiator = go.AddOrGet<Radiator>();
		radiator.intensity = 80;
		radiator.projectionCount = 12;
		radiator.direction = -90;
		radiator.angle = 90;
		go.AddOrGetDef<LightController.Def>();
	}
}
