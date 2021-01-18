using TUNING;
using UnityEngine;

public class MineralDeoxidizerConfig : IBuildingConfig
{
	public const string ID = "MineralDeoxidizer";

	private const float ALGAE_BURN_RATE = 0.55f;

	private const float ALGAE_STORAGE = 330f;

	private const float OXYGEN_GENERATION_RATE = 0.5f;

	private const float OXYGEN_TEMPERATURE = 303.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MineralDeoxidizer", 1, 2, "mineraldeoxidizer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		buildingDef.ViewMode = OverlayModes.Oxygen.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.Breakable = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
		electrolyzer.maxMass = 1.8f;
		electrolyzer.hasMeter = false;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 330f;
		storage.showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.55f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 303.15f, useEntityTemperature: false, storeOutput: false, 0f, 1f)
		};
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 330f;
		manualDeliveryKG.refillMass = 132f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
