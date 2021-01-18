using TUNING;
using UnityEngine;

public class OrbitalCargoModuleConfig : IBuildingConfig
{
	public const string ID = "OrbitalCargoModule";

	public const float NUM_CAPSULES = 3f;

	public const float TOTAL_STORAGE_MASS = 600f;

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
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.capacityKg = 600f;
		storage.showCapacityStatusItem = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		go.AddOrGet<StorageLocker>();
		OrbitalDeployCargoModule.Def def = go.AddOrGetDef<OrbitalDeployCargoModule.Def>();
		def.launchMass = 200f;
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		BuildingTemplates.ExtendBuildingToRocketModule(go, ROCKETRY.BURDEN.MODERATE, null);
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
