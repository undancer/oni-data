using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class NuclearReactorConfig : IBuildingConfig
{
	public const string ID = "NuclearReactor";

	private const float FUEL_CAPACITY = 10f;

	public const float VENT_STEAM_TEMPERATURE = 673.15f;

	public const float MELT_DOWN_TEMPERATURE = 973.15f;

	public const float MAX_VENT_PRESSURE = 20f;

	public const float REACTION_STRENGTH = 10f;

	public const int OPERATIONAL_RADIATOR_INTENSITY = 35;

	public const int MELT_DOWN_RADIATOR_INTENSITY = 100;

	public const float FUEL_CONSUMPTION_SPEED = 0.016666668f;

	public const float MINIMUM_REACTION_MASS = 0.5f;

	public const float GERMS_PER_KILOGRAM_WASTE = 10000f;

	public const float DUMP_WASTE_AMOUNT = 100f;

	public const float COOLANT_AMOUNT = 30f;

	public const float COOLANT_CAPACITY = 60f;

	public const float MINIMUM_COOLANT_MASS = 3f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("NuclearReactor", 5, 6, "generatornuclear_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 3200f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.GeneratorWattageRating = 0f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.RequiresPowerInput = false;
		buildingDef.RequiresPowerOutput = false;
		buildingDef.ThermalConductivity = 0.1f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Overheatable = false;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.UtilityInputOffset = new CellOffset(-2, 2);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Breakable = false;
		buildingDef.Invincible = true;
		buildingDef.Deprecated = !Sim.IsRadiationEnabled();
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
		radiationEmitter.emitRadiusX = 20;
		radiationEmitter.emitRadiusY = 28;
		radiationEmitter.emitRads = 15f;
		radiationEmitter.emitRate = 10f;
		radiationEmitter.emissionOffset = new Vector3(0f, 3f, 0f);
		Storage storage = go.AddComponent<Storage>();
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Hide
		});
		Storage storage2 = go.AddComponent<Storage>();
		storage2.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Hide
		});
		Storage storage3 = go.AddComponent<Storage>();
		storage3.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Hide
		});
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag;
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.minimumMass = 0.5f;
		go.AddOrGet<Reactor>();
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityKG = 60f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		conduitConsumer.storage = storage;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}
}
