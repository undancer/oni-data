using TUNING;
using UnityEngine;

public class LiquidHeaterConfig : IBuildingConfig
{
	public const string ID = "LiquidHeater";

	public const float CONSUMPTION_RATE = 1f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidHeater", 4, 1, "boiler_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 3200f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.Floodable = false;
		obj.EnergyConsumptionWhenActive = 960f;
		obj.ExhaustKilowattsWhenActive = 4000f;
		obj.SelfHeatKilowattsWhenActive = 64f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "SolidMetal";
		obj.OverheatTemperature = 398.15f;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
		return obj;
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
