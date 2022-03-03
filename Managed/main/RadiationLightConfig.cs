using TUNING;
using UnityEngine;

public class RadiationLightConfig : IBuildingConfig
{
	public const string ID = "RadiationLight";

	private Tag FUEL_ELEMENT = SimHashes.UraniumOre.CreateTag();

	private SimHashes WASTE_ELEMENT = SimHashes.DepletedUranium;

	private const float FUEL_PER_CYCLE = 10f;

	private const float CYCLES_PER_REFILL = 5f;

	private const float FUEL_TO_WASTE_RATIO = 0.5f;

	private const float FUEL_STORAGE_AMOUNT = 50f;

	private const float FUEL_CONSUMPTION_RATE = 0.016666668f;

	private const short RAD_LIGHT_SIZE_X = 16;

	private const short RAD_LIGHT_SIZE_Y = 4;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RadiationLight", 1, 1, "radiation_lamp_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnWall, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.capacityKg = 50f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = FUEL_ELEMENT;
		manualDeliveryKG.capacity = 50f;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
		radiationEmitter.emitAngle = 90f;
		radiationEmitter.emitDirection = 0f;
		radiationEmitter.emissionOffset = Vector3.right;
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.emitRadiusX = 16;
		radiationEmitter.emitRadiusY = 4;
		radiationEmitter.emitRads = 240f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(FUEL_ELEMENT, 0.016666668f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.008333334f, WASTE_ELEMENT, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.5f)
		};
		ElementDropper elementDropper = go.AddOrGet<ElementDropper>();
		elementDropper.emitTag = WASTE_ELEMENT.CreateTag();
		elementDropper.emitMass = 5f;
		RadiationLight radiationLight = go.AddComponent<RadiationLight>();
		radiationLight.elementToConsume = FUEL_ELEMENT;
		radiationLight.consumptionRate = 0.016666668f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}
}
