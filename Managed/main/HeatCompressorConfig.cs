using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class HeatCompressorConfig : IBuildingConfig
{
	public const string ID = "HeatCompressor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HeatCompressor", 4, 4, "hqbase_kanim", 250, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER5);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = 400f;
		obj.DefaultAnimState = "idle";
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(2, 0);
		obj.EnergyConsumptionWhenActive = 1600f;
		obj.InputConduitType = ConduitType.Liquid;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.UtilityOutputOffset = new CellOffset(2, 0);
		obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 1), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE) };
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		storage.showDescriptor = false;
		storage.showInUI = true;
		storage.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		Storage storage2 = go.AddComponent<Storage>();
		storage2.showDescriptor = false;
		storage2.showInUI = true;
		storage2.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		Storage storage3 = go.AddComponent<Storage>();
		storage3.showDescriptor = false;
		storage3.showInUI = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityKG = 100f;
		conduitConsumer.storage = storage;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.storage = storage2;
		conduitDispenser.alwaysDispense = true;
		go.AddOrGet<HeatCompressor>().SetStorage(storage, storage2, storage3);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}
}
