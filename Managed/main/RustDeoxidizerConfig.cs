using TUNING;
using UnityEngine;

public class RustDeoxidizerConfig : IBuildingConfig
{
	public const string ID = "RustDeoxidizer";

	private const float RUST_KG_CONSUMPTION_RATE = 0.75f;

	private const float SALT_KG_CONSUMPTION_RATE = 0.25f;

	private const float RUST_KG_PER_REFILL = 585f;

	private const float SALT_KG_PER_REFILL = 195f;

	private const float TOTAL_CONSUMPTION_RATE = 1f;

	private const float IRON_CONVERSION_RATIO = 0.4f;

	private const float OXYGEN_CONVERSION_RATIO = 0.57f;

	private const float CHLORINE_CONVERSION_RATIO = 0.029999971f;

	public const float OXYGEN_TEMPERATURE = 348.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RustDeoxidizer", 2, 3, "rust_deoxidizer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		obj.ViewMode = OverlayModes.Oxygen.ID;
		obj.AudioCategory = "HollowMetal";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<RustDeoxidizer>().maxMass = 1.8f;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Rust");
		manualDeliveryKG.capacity = 585f;
		manualDeliveryKG.refillMass = 193.05f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = new Tag("Salt");
		manualDeliveryKG2.capacity = 195f;
		manualDeliveryKG2.refillMass = 64.350006f;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Rust"), 0.75f),
			new ElementConverter.ConsumedElement(new Tag("Salt"), 0.25f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[3]
		{
			new ElementConverter.OutputElement(0.57f, SimHashes.Oxygen, 348.15f, useEntityTemperature: false, storeOutput: false, 0f, 1f),
			new ElementConverter.OutputElement(0.029999971f, SimHashes.ChlorineGas, 348.15f, useEntityTemperature: false, storeOutput: false, 0f, 1f),
			new ElementConverter.OutputElement(0.4f, SimHashes.IronOre, 348.15f, useEntityTemperature: false, storeOutput: true, 0f, 1f)
		};
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 24f;
		elementDropper.emitTag = SimHashes.IronOre.CreateTag();
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
