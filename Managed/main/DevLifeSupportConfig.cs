using TUNING;
using UnityEngine;

public class DevLifeSupportConfig : IBuildingConfig
{
	public const string ID = "DevLifeSupport";

	private const float OXYGEN_GENERATION_RATE = 50.000004f;

	private const float OXYGEN_TEMPERATURE = 303.15f;

	private const float OXYGEN_MAX_PRESSURE = 1.5f;

	private const float CO2_CONSUMPTION_RATE = 50.000004f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("DevLifeSupport", 1, 1, "dev_generator_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER3);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		obj.DebugOnly = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.capacityKg = 200f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		CellOffset cellOffset = new CellOffset(0, 1);
		ElementEmitter elementEmitter = go.AddOrGet<ElementEmitter>();
		elementEmitter.outputElement = new ElementConverter.OutputElement(50.000004f, SimHashes.Oxygen, 303.15f, useEntityTemperature: false, storeOutput: false, cellOffset.x, cellOffset.y);
		elementEmitter.emissionFrequency = 1f;
		elementEmitter.maxPressure = 1.5f;
		PassiveElementConsumer passiveElementConsumer = go.AddOrGet<PassiveElementConsumer>();
		passiveElementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		passiveElementConsumer.consumptionRate = 50.000004f;
		passiveElementConsumer.capacityKG = 50.000004f;
		passiveElementConsumer.consumptionRadius = 10;
		passiveElementConsumer.showInStatusPanel = true;
		passiveElementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		passiveElementConsumer.isRequired = false;
		passiveElementConsumer.storeOnConsume = false;
		passiveElementConsumer.showDescriptor = false;
		passiveElementConsumer.ignoreActiveChanged = true;
		go.AddOrGet<DevLifeSupport>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
