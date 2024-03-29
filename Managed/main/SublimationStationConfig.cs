using TUNING;
using UnityEngine;

public class SublimationStationConfig : IBuildingConfig
{
	public const string ID = "SublimationStation";

	private const float DIRT_CONSUME_RATE = 1f;

	private const float DIRT_STORAGE = 600f;

	private const float OXYGEN_GENERATION_RATE = 0.66f;

	private const float OXYGEN_TEMPERATURE = 303.15f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SublimationStation", 2, 1, "sublimation_station_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		obj.ViewMode = OverlayModes.Oxygen.ID;
		obj.AudioCategory = "HollowMetal";
		obj.Breakable = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		CellOffset emissionOffset = new CellOffset(0, 0);
		Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
		electrolyzer.maxMass = 1.8f;
		electrolyzer.hasMeter = false;
		electrolyzer.emissionOffset = emissionOffset;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 600f;
		storage.showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(SimHashes.ToxicSand.CreateTag(), 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.66f, SimHashes.ContaminatedOxygen, 303.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y)
		};
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = SimHashes.ToxicSand.CreateTag();
		manualDeliveryKG.capacity = 600f;
		manualDeliveryKG.refillMass = 240f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
