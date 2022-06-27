using TUNING;
using UnityEngine;

public class EthanolDistilleryConfig : IBuildingConfig
{
	public const string ID = "EthanolDistillery";

	public const float ORGANICS_CONSUME_PER_SECOND = 1f;

	public const float ORGANICS_STORAGE_AMOUNT = 600f;

	public const float ETHANOL_RATE = 0.5f;

	public const float SOLID_WASTE_RATE = 1f / 3f;

	public const float CO2_WASTE_RATE = 1f / 6f;

	public const float OUTPUT_TEMPERATURE = 346.5f;

	public const float WASTE_OUTPUT_TEMPERATURE = 366.5f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("EthanolDistillery", 4, 3, "ethanoldistillery_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Overheatable = false;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.AudioCategory = "HollowMetal";
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.PowerInputOffset = new CellOffset(2, 0);
		obj.UtilityOutputOffset = new CellOffset(-1, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = new SimHashes[1] { SimHashes.Ethanol };
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = WoodLogConfig.TAG;
		manualDeliveryKG.capacity = 600f;
		manualDeliveryKG.refillMass = 150f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(WoodLogConfig.TAG, 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[3]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Ethanol, 346.5f, useEntityTemperature: false, storeOutput: true),
			new ElementConverter.OutputElement(1f / 3f, SimHashes.ToxicSand, 366.5f, useEntityTemperature: false, storeOutput: true),
			new ElementConverter.OutputElement(1f / 6f, SimHashes.CarbonDioxide, 366.5f)
		};
		AlgaeDistillery algaeDistillery = go.AddOrGet<AlgaeDistillery>();
		algaeDistillery.emitMass = 20f;
		algaeDistillery.emitTag = new Tag("ToxicSand");
		algaeDistillery.emitOffset = new Vector3(2f, 1f);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
