using TUNING;
using UnityEngine;

public class OxyliteRefineryConfig : IBuildingConfig
{
	public const string ID = "OxyliteRefinery";

	public const float EMIT_MASS = 10f;

	public const float INPUT_O2_PER_SECOND = 0.6f;

	public const float OXYLITE_PER_SECOND = 0.6f;

	public const float GOLD_PER_SECOND = 0.003f;

	public const float OUTPUT_TEMP = 303.15f;

	public const float REFILL_RATE = 2400f;

	public const float GOLD_STORAGE_AMOUNT = 7.2000003f;

	public const float O2_STORAGE_AMOUNT = 6f;

	public const float STORAGE_CAPACITY = 23.2f;

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"RefinedMetal",
			"Plastic"
		};
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		}, construction_materials: array, melting_point: 2400f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, id: "OxyliteRefinery", width: 3, height: 4, anim: "oxylite_refinery_kanim", hitpoints: 100, construction_time: 480f, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Tag tag = SimHashes.Oxygen.CreateTag();
		Tag tag2 = SimHashes.Gold.CreateTag();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		OxyliteRefinery oxyliteRefinery = go.AddOrGet<OxyliteRefinery>();
		oxyliteRefinery.emitTag = SimHashes.OxyRock.CreateTag();
		oxyliteRefinery.emitMass = 10f;
		oxyliteRefinery.dropOffset = new Vector3(0f, 1f);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1.2f;
		conduitConsumer.capacityTag = tag;
		conduitConsumer.capacityKG = 6f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 23.2f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = tag2;
		manualDeliveryKG.refillMass = 1.8000001f;
		manualDeliveryKG.capacity = 7.2000003f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(tag, 0.6f),
			new ElementConverter.ConsumedElement(tag2, 0.003f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.6f, SimHashes.OxyRock, 303.15f, useEntityTemperature: false, storeOutput: true)
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
