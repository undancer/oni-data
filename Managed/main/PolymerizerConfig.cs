using TUNING;
using UnityEngine;

public class PolymerizerConfig : IBuildingConfig
{
	public const string ID = "Polymerizer";

	private const ConduitType INPUT_CONDUIT_TYPE = ConduitType.Liquid;

	private const ConduitType OUTPUT_CONDUIT_TYPE = ConduitType.Gas;

	private const float CONSUMED_OIL_KG_PER_DAY = 500f;

	private const float GENERATED_PLASTIC_KG_PER_DAY = 300f;

	private const float SECONDS_PER_PLASTIC_BLOCK = 60f;

	private const float GENERATED_EXHAUST_STEAM_KG_PER_DAY = 5f;

	private const float GENERATED_EXHAUST_CO2_KG_PER_DAY = 5f;

	public const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;

	private const SimHashes PRODUCED_ELEMENT = SimHashes.Polypropylene;

	private const SimHashes EXHAUST_ENVIRONMENT_ELEMENT = SimHashes.Steam;

	private const SimHashes EXHAUST_CONDUIT_ELEMENT = SimHashes.CarbonDioxide;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Polymerizer", 3, 3, "plasticrefinery_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(obj);
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 32f;
		obj.PowerInputOffset = new CellOffset(0, 0);
		obj.InputConduitType = ConduitType.Liquid;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.OutputConduitType = ConduitType.Gas;
		obj.UtilityOutputOffset = new CellOffset(0, 1);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Polymerizer polymerizer = go.AddOrGet<Polymerizer>();
		polymerizer.emitMass = 30f;
		polymerizer.emitTag = GameTagExtensions.Create(SimHashes.Polypropylene);
		polymerizer.emitOffset = new Vector3(-1.45f, 1f, 0f);
		polymerizer.exhaustElement = SimHashes.Steam;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1.6666666f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Petroleum);
		conduitConsumer.capacityKG = 1.6666666f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.invertElementFilter = false;
		conduitDispenser.elementFilter = new SimHashes[1] { SimHashes.CarbonDioxide };
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.Petroleum), 5f / 6f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[3]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Polypropylene, 348.15f, useEntityTemperature: false, storeOutput: true),
			new ElementConverter.OutputElement(1f / 120f, SimHashes.Steam, 473.15f, useEntityTemperature: false, storeOutput: true),
			new ElementConverter.OutputElement(1f / 120f, SimHashes.CarbonDioxide, 423.15f, useEntityTemperature: false, storeOutput: true)
		};
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
