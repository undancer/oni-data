using TUNING;
using UnityEngine;

public class LiquidHeaterConfig : IBuildingConfig
{
	public const string ID = "LiquidHeater";

	public const float CONSUMPTION_RATE = 1f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidHeater", 4, 1, "boiler_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 3200f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.Floodable = false;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.ExhaustKilowattsWhenActive = 4000f;
		buildingDef.SelfHeatKilowattsWhenActive = 64f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "SolidMetal";
		buildingDef.OverheatTemperature = 398.15f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		SpaceHeater spaceHeater = go.AddOrGet<SpaceHeater>();
		spaceHeater.SetLiquidHeater();
		spaceHeater.targetTemperature = 358.15f;
		spaceHeater.minimumCellMass = 400f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
