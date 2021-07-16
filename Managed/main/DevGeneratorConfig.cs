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
		obj.RequiresPowerOutput = true;
		obj.PowerOutputOffset = new CellOffset(0, 0);
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		obj.Floodable = false;
		obj.DebugOnly = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		DevGenerator devGenerator = go.AddOrGet<DevGenerator>();
		devGenerator.powerDistributionOrder = 9;
		devGenerator.wattageRating = 100000f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
