using TUNING;
using UnityEngine;

public class HydrogenGeneratorConfig : IBuildingConfig
{
	public const string ID = "HydrogenGenerator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HydrogenGenerator", 4, 3, "generatormerc_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.GeneratorWattageRating = 800f;
		obj.GeneratorBaseCapacity = 1000f;
		obj.ExhaustKilowattsWhenActive = 2f;
		obj.SelfHeatKilowattsWhenActive = 2f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.PowerOutputOffset = new CellOffset(1, 0);
		obj.InputConduitType = ConduitType.Gas;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Storage>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Hydrogen.CreateTag(), 0.1f, 2f);
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.meterOffset = Meter.Offset.Behind;
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
