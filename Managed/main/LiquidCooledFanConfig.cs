using TUNING;
using UnityEngine;

public class LiquidCooledFanConfig : IBuildingConfig
{
	public const string ID = "LiquidCooledFan";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidCooledFan", 2, 2, "fanliquid_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.Deprecated = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 100f;
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		float num = 2426.72f;
		float num2 = 0.01f;
		LiquidCooledFan liquidCooledFan = go.AddOrGet<LiquidCooledFan>();
		liquidCooledFan.gasStorage = storage;
		liquidCooledFan.liquidStorage = storage2;
		liquidCooledFan.waterKGConsumedPerKJ = 1f / (num * num2);
		liquidCooledFan.coolingKilowatts = 80f;
		liquidCooledFan.minCooledTemperature = 290f;
		liquidCooledFan.minEnvironmentMass = 0.25f;
		liquidCooledFan.minCoolingRange = new Vector2I(-2, 0);
		liquidCooledFan.maxCoolingRange = new Vector2I(2, 4);
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = new Tag("Water");
		manualDeliveryKG.capacity = 500f;
		manualDeliveryKG.refillMass = 50f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.storeOnConsume = true;
		elementConsumer.storage = storage;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRadius = 8;
		elementConsumer.EnableConsumption(enabled: true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.showDescriptor = false;
		LiquidCooledFanWorkable liquidCooledFanWorkable = go.AddOrGet<LiquidCooledFanWorkable>();
		liquidCooledFanWorkable.SetWorkTime(20f);
		liquidCooledFanWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_liquidfan_kanim")
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
