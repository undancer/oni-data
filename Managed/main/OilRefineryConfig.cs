using TUNING;
using UnityEngine;

public class OilRefineryConfig : IBuildingConfig
{
	public const string ID = "OilRefinery";

	public const SimHashes INPUT_ELEMENT = SimHashes.CrudeOil;

	private const SimHashes OUTPUT_LIQUID_ELEMENT = SimHashes.Petroleum;

	private const SimHashes OUTPUT_GAS_ELEMENT = SimHashes.Methane;

	public const float CONSUMPTION_RATE = 10f;

	public const float OUTPUT_LIQUID_RATE = 5f;

	public const float OUTPUT_GAS_RATE = 0.09f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("OilRefinery", 4, 4, "oilrefinery_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.EnergyConsumptionWhenActive = 480f;
		obj.ExhaustKilowattsWhenActive = 2f;
		obj.SelfHeatKilowattsWhenActive = 8f;
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.AudioCategory = "HollowMetal";
		obj.InputConduitType = ConduitType.Liquid;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.OutputConduitType = ConduitType.Liquid;
		obj.UtilityOutputOffset = new CellOffset(1, 1);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		OilRefinery oilRefinery = go.AddOrGet<OilRefinery>();
		oilRefinery.overpressureWarningMass = 4.5f;
		oilRefinery.overpressureMass = 5f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = SimHashes.CrudeOil.CreateTag();
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.capacityKG = 100f;
		conduitConsumer.forceAlwaysSatisfied = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1] { SimHashes.CrudeOil };
		go.AddOrGet<Storage>().showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(SimHashes.CrudeOil.CreateTag(), 10f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(5f, SimHashes.Petroleum, 348.15f, useEntityTemperature: false, storeOutput: true, 0f, 1f),
			new ElementConverter.OutputElement(0.09f, SimHashes.Methane, 348.15f, useEntityTemperature: false, storeOutput: false, 0f, 3f)
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
