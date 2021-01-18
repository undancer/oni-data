using TUNING;
using UnityEngine;

public class DevGeneratorConfig : IBuildingConfig
{
	public const string ID = "DevGenerator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("DevGenerator", 1, 1, "dev_generator_kanim", 100, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.GeneratorWattageRating = 100000f;
		obj.GeneratorBaseCapacity = 200000f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		obj.Floodable = false;
		obj.DebugOnly = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.hasMeter = false;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.formula = default(EnergyGenerator.Formula);
		energyGenerator.powerDistributionOrder = 9;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
