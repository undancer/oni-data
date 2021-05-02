using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OrbitalCargoModuleConfig : IBuildingConfig
{
	public const string ID = "OrbitalCargoModule";

	public static int NUM_CAPSULES = 3 * Mathf.RoundToInt(ROCKETRY.CARGO_CAPACITY_SCALE);

	public static float TOTAL_STORAGE_MASS = 200f * (float)NUM_CAPSULES;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OrbitalCargoModule", 3, 2, "rocket_orbital_deploy_cargo_module_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER2, MATERIALS.RAW_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.DefaultAnimState = "deployed";
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		List<Tag> list = new List<Tag>();
		list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
		list.AddRange(STORAGEFILTERS.FOOD);
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.capacityKg = TOTAL_STORAGE_MASS;
		storage.showCapacityStatusItem = true;
		storage.showDescriptor = true;
		storage.storageFilters = list;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		go.AddOrGet<StorageLocker>();
		OrbitalDeployCargoModule.Def def = go.AddOrGetDef<OrbitalDeployCargoModule.Def>();
		def.numCapsules = NUM_CAPSULES;
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE);
		FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
		fakeFloorAdder.floorOffsets = new CellOffset[3]
		{
			new CellOffset(-1, -1),
			new CellOffset(0, -1),
			new CellOffset(1, -1)
		};
		fakeFloorAdder.initiallyActive = false;
	}
}
