using TUNING;
using UnityEngine;

public class AtmoicGardenConfig : IBuildingConfig
{
	public const string ID = "AtomicGarden";

	private const float FERTILIZER_PER_LOAD = 10f;

	private const float FERTILIZER_PRODUCTION_RATE = 0.12f;

	private const float METHANE_PRODUCTION_RATE = 0.01f;

	private const float _TOTAL_PRODUCTION = 0.13f;

	private const float DIRT_CONSUMPTION_RATE = 0.065f;

	private const float DIRTY_WATER_CONSUMPTION_RATE = 0.039f;

	private const float PHOSPHORITE_CONSUMPTION_RATE = 0.025999999f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AtomicGarden", 4, 3, "fertilizer_maker_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<WaterPurifier>();
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = new Tag("Dirt");
		manualDeliveryKG.capacity = 136.5f;
		manualDeliveryKG.refillMass = 19.5f;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG2.requestedItemTag = new Tag("Phosphorite");
		manualDeliveryKG2.capacity = 54.6f;
		manualDeliveryKG2.refillMass = 7.7999997f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		conduitConsumer.capacityKG = 0.19500001f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[3]
		{
			new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 0.039f),
			new ElementConverter.ConsumedElement(new Tag("Dirt"), 0.065f),
			new ElementConverter.ConsumedElement(new Tag("Phosphorite"), 0.025999999f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.12f, SimHashes.Fertilizer, 323.15f, useEntityTemperature: false, storeOutput: true)
		};
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.01f;
		buildingElementEmitter.temperature = 349.15f;
		buildingElementEmitter.element = SimHashes.Methane;
		buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = new Tag("Fertilizer");
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
