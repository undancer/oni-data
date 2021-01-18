using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ObjectDispenserConfig : IBuildingConfig
{
	public const string ID = "ObjectDispenser";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ObjectDispenser", 1, 2, "object_dispenser_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(ObjectDispenser.PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT_INACTIVE)
		};
		SoundEventVolumeCache.instance.AddVolume("ventliquid_kanim", "LiquidVent_squirt", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		ObjectDispenser objectDispenser = go.AddOrGet<ObjectDispenser>();
		objectDispenser.dropOffset = new CellOffset(1, 0);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.StorageLocker;
		Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}
}
