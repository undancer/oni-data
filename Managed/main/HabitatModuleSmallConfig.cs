using TUNING;
using UnityEngine;

public class HabitatModuleSmallConfig : IBuildingConfig
{
	public const string ID = "HabitatModuleSmall";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HabitatModuleSmall", 3, 3, "rocket_nosecone_kanim", 1000, 60f, BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER0, MATERIALS.RAW_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		buildingDef.ShowInBuildMenu = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.GetComponent<KPrefabID>().AddTag(GameTags.NoseRocketModule);
		go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);
		AssignmentGroupController assignmentGroupController = go.AddOrGet<AssignmentGroupController>();
		assignmentGroupController.generateGroupOnStart = true;
		go.AddOrGet<PassengerRocketModule>();
		ClustercraftExteriorDoor clustercraftExteriorDoor = go.AddOrGet<ClustercraftExteriorDoor>();
		clustercraftExteriorDoor.interiorTemplateName = "expansion1::interiors/habitat_small";
		go.AddOrGetDef<SimpleDoorController.Def>();
		go.AddOrGet<NavTeleporter>();
		go.AddOrGet<AccessControl>();
		go.AddOrGet<LaunchableRocketCluster>();
		go.AddOrGet<RocketCommandConditions>();
		go.AddOrGet<RocketProcessConditionDisplayTarget>();
		CharacterOverlay characterOverlay = go.AddOrGet<CharacterOverlay>();
		characterOverlay.shouldShowName = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR_PLUS);
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
		ownable.canBePublic = false;
		FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
		fakeFloorAdder.floorOffsets = new CellOffset[3]
		{
			new CellOffset(-1, -1),
			new CellOffset(0, -1),
			new CellOffset(1, -1)
		};
		fakeFloorAdder.initiallyActive = false;
		go.GetComponent<ReorderableBuilding>().buildConditions.Add(new LimitOneCommandModule());
		go.GetComponent<ReorderableBuilding>().buildConditions.Add(new TopOnly());
	}
}