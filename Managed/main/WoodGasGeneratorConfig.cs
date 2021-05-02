using TUNING;
using UnityEngine;

public class WoodGasGeneratorConfig : IBuildingConfig
{
	public const string ID = "WoodGasGenerator";

	private const float BRANCHES_PER_GENERATOR = 8f;

	public const float CONSUMPTION_RATE = 1.2f;

	private const float WOOD_PER_REFILL = 360f;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_GAS2 = SimHashes.Syngas;

	public const float CO2_EXHAUST_RATE = 0.17f;

	private const int WIDTH = 2;

	private const int HEIGHT = 2;

	public override BuildingDef CreateBuildingDef()
	{
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, construction_materials: aLL_METALS, melting_point: 2400f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, id: "WoodGasGenerator", width: 2, height: 2, anim: "generatorwood_kanim", hitpoints: 100, construction_time: 120f, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.GeneratorWattageRating = 300f;
		buildingDef.GeneratorBaseCapacity = 20000f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		float max_stored_input_mass = 720f;
		go.AddOrGet<LoopingSounds>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = WoodLogConfig.TAG;
		manualDeliveryKG.capacity = 360f;
		manualDeliveryKG.refillMass = 180f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.hasMeter = true;
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(WoodLogConfig.TAG, 1.2f, max_stored_input_mass, SimHashes.CarbonDioxide, 0.17f, store_output_mass: false, new CellOffset(0, 1), 383.15f);
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
