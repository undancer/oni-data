using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidReservoirConfig : IBuildingConfig
{
	public const string ID = "LiquidReservoir";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	private const int WIDTH = 2;

	private const int HEIGHT = 3;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidReservoir", 2, 3, "liquidreservoir_kanim", 100, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(1, 2);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidReservoir");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Reservoir>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage.capacityKg = 5000f;
		storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		go.AddOrGet<SmartReservoir>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.ignoreMinMassCheck = true;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.capacityKG = storage.capacityKg;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = null;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
