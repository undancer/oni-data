using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class StorageLockerSmartConfig : IBuildingConfig
{
	public const string ID = "StorageLockerSmart";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("StorageLockerSmart", 1, 2, "smartstoragelocker_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.StorageLocker;
		go.AddOrGet<StorageLockerSmart>();
		go.AddOrGet<UserNameable>();
		go.AddOrGetDef<StorageController.Def>();
	}
}
