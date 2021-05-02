using TUNING;
using UnityEngine;

public class PetroleumGeneratorConfig : IBuildingConfig
{
	public const string ID = "PetroleumGenerator";

	public const float CONSUMPTION_RATE = 2f;

	private const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_LIQUID = SimHashes.DirtyWater;

	public const float EFFICIENCY_RATE = 0.5f;

	public const float EXHAUST_GAS_RATE = 0.5f;

	public const float EXHAUST_LIQUID_RATE = 0.75f;

	private const int WIDTH = 3;

	private const int HEIGHT = 4;

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[1]
		{
			"Metal"
		};
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[1]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0]
		}, construction_materials: array, melting_point: 2400f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, id: "PetroleumGenerator", width: 3, height: 4, anim: "generatorpetrol_kanim", hitpoints: 100, construction_time: 480f, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.ExhaustKilowattsWhenActive = 4f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.InputConduitType = ConduitType.Liquid;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Storage>();
		BuildingDef def = go.GetComponent<Building>().Def;
		float num = 20f;
		go.AddOrGet<LoopingSounds>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = def.InputConduitType;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = GameTags.CombustibleLiquid;
		conduitConsumer.capacityKG = num;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.hasMeter = true;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[1]
		{
			new EnergyGenerator.InputItem(GameTags.CombustibleLiquid, 2f, num)
		};
		formula.outputs = new EnergyGenerator.OutputItem[2]
		{
			new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, store: false, new CellOffset(0, 3), 383.15f),
			new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.75f, store: false, new CellOffset(1, 1), 313.15f)
		};
		energyGenerator.formula = formula;
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
