using TUNING;
using UnityEngine;

public class AirConditionerConfig : IBuildingConfig
{
	public const string ID = "AirConditioner";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("AirConditioner", 2, 2, "airconditioner_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(obj);
		obj.EnergyConsumptionWhenActive = 240f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.ThermalConductivity = 5f;
		obj.InputConduitType = ConduitType.Gas;
		obj.OutputConduitType = ConduitType.Gas;
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
		airConditioner.temperatureDelta = -14f;
		airConditioner.maxEnvironmentDelta = -50f;
		BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
