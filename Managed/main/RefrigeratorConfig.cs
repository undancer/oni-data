using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RefrigeratorConfig : IBuildingConfig
{
	public const string ID = "Refrigerator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Refrigerator", 1, 2, "fridge_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER0, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE)
		};
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.capacityKg = 100f;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		Prioritizable.AddRef(go);
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<Refrigerator>();
		go.AddOrGet<UserNameable>();
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGetDef<StorageController.Def>();
	}
}
