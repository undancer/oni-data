using TUNING;
using UnityEngine;

public class SweepBotStationConfig : IBuildingConfig
{
	public const string ID = "SweepBotStation";

	public const float POWER_USAGE = 240f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SweepBotStation", 2, 2, "sweep_bot_base_station_kanim", 30, 30f, new float[1]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0] - SweepBotConfig.MASS
		}, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.ignoreSourcePriority = true;
		storage.showDescriptor = false;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.Building;
		storage.capacityKg = 25f;
		storage.allowClearable = false;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.showInUI = true;
		storage2.allowItemRemoval = true;
		storage2.ignoreSourcePriority = true;
		storage2.showDescriptor = true;
		storage2.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage2.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage2.fetchCategory = Storage.FetchCategory.StorageSweepOnly;
		storage2.capacityKg = 1000f;
		storage2.allowClearable = true;
		storage2.showCapacityStatusItem = true;
		CharacterOverlay characterOverlay = go.AddOrGet<CharacterOverlay>();
		characterOverlay.shouldShowName = true;
		SweepBotStation sweepBotStation = go.AddOrGet<SweepBotStation>();
		sweepBotStation.SetStorages(storage, storage2);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}
}
