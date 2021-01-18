using System;
using TUNING;
using UnityEngine;

public class OilWellCapConfig : IBuildingConfig
{
	private const float WATER_INTAKE_RATE = 1f;

	private const float WATER_TO_OIL_RATIO = 3.3333333f;

	private const float LIQUID_STORAGE = 10f;

	private const float GAS_RATE = 71f / (678f * (float)Math.PI);

	private const float OVERPRESSURE_TIME = 2400f;

	private const float PRESSURE_RELEASE_TIME = 180f;

	private const float PRESSURE_RELEASE_RATE = 0.44444448f;

	private static readonly Tag INPUT_WATER_TAG = SimHashes.Water.CreateTag();

	public const string ID = "OilWellCap";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OilWellCap", 4, 4, "geyser_oil_cap_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 1);
		buildingDef.PowerInputOffset = new CellOffset(1, 1);
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.AttachmentSlotTag = GameTags.OilWell;
		buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 2f;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.capacityTag = INPUT_WATER_TAG;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(INPUT_WATER_TAG, 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(3.3333333f, SimHashes.CrudeOil, 363.15f, useEntityTemperature: false, storeOutput: false, 2f, 1.5f, 0f)
		};
		OilWellCap oilWellCap = go.AddOrGet<OilWellCap>();
		oilWellCap.gasElement = SimHashes.Methane;
		oilWellCap.gasTemperature = 573.15f;
		oilWellCap.addGasRate = 71f / (678f * (float)Math.PI);
		oilWellCap.maxGasPressure = 80.00001f;
		oilWellCap.releaseGasRate = 0.44444448f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}
}
