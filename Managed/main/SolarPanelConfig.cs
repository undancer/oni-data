using TUNING;
using UnityEngine;

public class SolarPanelConfig : IBuildingConfig
{
	public const string ID = "SolarPanel";

	public const float WATTS_PER_LUX = 0.00053f;

	public const float MAX_WATTS = 380f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolarPanel", 7, 3, "solar_panel_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.GLASSES, 2400f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.GeneratorWattageRating = 380f;
		obj.GeneratorBaseCapacity = obj.GeneratorWattageRating;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.BuildLocationRule = BuildLocationRule.Anywhere;
		obj.HitPoints = 10;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<Repairable>().expectedRepairTime = 52.5f;
		go.AddOrGet<SolarPanel>().powerDistributionOrder = 9;
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
