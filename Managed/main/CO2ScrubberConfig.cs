using TUNING;
using UnityEngine;

public class CO2ScrubberConfig : IBuildingConfig
{
	public const string ID = "CO2Scrubber";

	private const float CO2_CONSUMPTION_RATE = 0.3f;

	private const float H2O_CONSUMPTION_RATE = 1f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CO2Scrubber", 2, 2, "co2scrubber_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.Oxygen.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.capacityKg = 30000f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		AirFilter airFilter = go.AddOrGet<AirFilter>();
		airFilter.filterTag = GameTagExtensions.Create(SimHashes.Water);
		ElementConsumer elementConsumer = go.AddOrGet<PassiveElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.6f;
		elementConsumer.capacityKG = 0.6f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showDescriptor = false;
		elementConsumer.ignoreActiveChanged = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.Water), 1f),
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.CarbonDioxide), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 0f, useEntityTemperature: false, storeOutput: true)
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 2f;
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		conduitConsumer.forceAlwaysSatisfied = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
