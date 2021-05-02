using TUNING;
using UnityEngine;

public class SolarPanelConfig : IBuildingConfig
{
	public const string ID = "SolarPanel";

	public const float WATTS_PER_LUX = 0.00053f;

	public const float MAX_WATTS = 380f;

	private const int WIDTH = 7;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolarPanel", 7, 3, "solar_panel_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.GLASSES, 2400f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.GeneratorWattageRating = 380f;
		buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.BuildLocationRule = BuildLocationRule.Anywhere;
		buildingDef.HitPoints = 10;
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Repairable repairable = go.AddOrGet<Repairable>();
		repairable.expectedRepairTime = 52.5f;
		SolarPanel solarPanel = go.AddOrGet<SolarPanel>();
		solarPanel.powerDistributionOrder = 9;
		go.AddOrGetDef<PoweredActiveController.Def>();
		MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
		def.solidOffsets = new CellOffset[7];
		for (int i = 0; i < 7; i++)
		{
			def.solidOffsets[i] = new CellOffset(i - 3, 0);
		}
	}
}
