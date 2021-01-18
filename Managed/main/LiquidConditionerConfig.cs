using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LiquidConditionerConfig : IBuildingConfig
{
	public const string ID = "LiquidConditioner";

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidConditioner", 2, 2, "liquidconditioner_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER6, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(obj);
		obj.EnergyConsumptionWhenActive = 1200f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.InputConduitType = ConduitType.Liquid;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.Floodable = false;
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.OverheatTemperature = 398.15f;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidConditioner");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
		airConditioner.temperatureDelta = -14f;
		airConditioner.maxEnvironmentDelta = -50f;
		airConditioner.isLiquidConditioner = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.capacityKg = 2f * conduitConsumer.consumptionRate;
		storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
