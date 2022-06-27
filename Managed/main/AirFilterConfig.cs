using TUNING;
using UnityEngine;

public class AirFilterConfig : IBuildingConfig
{
	public const string ID = "AirFilter";

	public const float DIRTY_AIR_CONSUMPTION_RATE = 0.1f;

	private const float SAND_CONSUMPTION_RATE = 2f / 15f;

	private const float REFILL_RATE = 2400f;

	private const float SAND_STORAGE_AMOUNT = 320.00003f;

	private const float CLAY_PER_LOAD = 10f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("AirFilter", 1, 1, "co2filter_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER0, decor: BUILDINGS.DECOR.NONE);
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Oxygen.ID;
		obj.AudioCategory = "Metal";
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(0, 0);
		obj.EnergyConsumptionWhenActive = 5f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.capacityKg = 200f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.ContaminatedOxygen;
		elementConsumer.consumptionRate = 0.5f;
		elementConsumer.capacityKG = 0.5f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showDescriptor = false;
		elementConsumer.ignoreActiveChanged = true;
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = new Tag("Clay");
		elementDropper.emitOffset = new Vector3(0f, 0f, 0f);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Filter"), 2f / 15f),
			new ElementConverter.ConsumedElement(new Tag("ContaminatedOxygen"), 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(0.14333335f, SimHashes.Clay, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.25f),
			new ElementConverter.OutputElement(0.089999996f, SimHashes.Oxygen, 0f, useEntityTemperature: false, storeOutput: false, 0f, 0f, 0.75f)
		};
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Filter");
		manualDeliveryKG.capacity = 320.00003f;
		manualDeliveryKG.refillMass = 32.000004f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		go.AddOrGet<AirFilter>().filterTag = new Tag("Filter");
		go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<ActiveController.Def>();
	}
}
