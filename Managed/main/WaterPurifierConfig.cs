using TUNING;
using UnityEngine;

public class WaterPurifierConfig : IBuildingConfig
{
	public const string ID = "WaterPurifier";

	private const float FILTER_INPUT_RATE = 1f;

	private const float DIRTY_WATER_INPUT_RATE = 5f;

	private const float FILTER_CAPACITY = 1200f;

	private const float USED_FILTER_OUTPUT_RATE = 0.2f;

	private const float CLEAN_WATER_OUTPUT_RATE = 5f;

	private const float TARGET_OUTPUT_TEMPERATURE = 313.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("WaterPurifier", 4, 3, "waterpurifier_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.InputConduitType = ConduitType.Liquid;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.AudioCategory = "HollowMetal";
		obj.PowerInputOffset = new CellOffset(2, 0);
		obj.UtilityInputOffset = new CellOffset(-1, 2);
		obj.UtilityOutputOffset = new CellOffset(2, 2);
		obj.PermittedRotations = PermittedRotations.FlipH;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "WaterPurifier");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<WaterPurifier>();
		Prioritizable.AddRef(go);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Filter"), 1f),
			new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 5f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(5f, SimHashes.Water, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.75f),
			new ElementConverter.OutputElement(0.2f, SimHashes.ToxicSand, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.25f)
		};
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = new Tag("ToxicSand");
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Filter");
		manualDeliveryKG.capacity = 1200f;
		manualDeliveryKG.refillMass = 300f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityKG = 20f;
		conduitConsumer.capacityTag = GameTags.AnyWater;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.DirtyWater
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
