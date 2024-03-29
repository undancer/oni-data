using TUNING;
using UnityEngine;

public class GeneratorConfig : IBuildingConfig
{
	public const string ID = "Generator";

	private const float COAL_BURN_RATE = 1f;

	private const float COAL_CAPACITY = 600f;

	public const float CO2_OUTPUT_TEMPERATURE = 383.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Generator", 3, 3, "generatorphos_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.GeneratorWattageRating = 600f;
		obj.GeneratorBaseCapacity = 20000f;
		obj.ExhaustKilowattsWhenActive = 8f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.RequiresPowerOutput = true;
		obj.PowerOutputOffset = new CellOffset(0, 0);
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Carbon.CreateTag(), 1f, 600f, SimHashes.CarbonDioxide, 0.02f, store_output_mass: false, new CellOffset(1, 2), 383.15f);
		energyGenerator.meterOffset = Meter.Offset.Behind;
		energyGenerator.SetSliderValue(50f, 0);
		energyGenerator.powerDistributionOrder = 9;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 600f;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Coal");
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
		Tinkerable.MakePowerTinkerable(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
