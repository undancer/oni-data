using TUNING;
using UnityEngine;

public class ExobaseHeadquartersConfig : IBuildingConfig
{
	public const string ID = "ExobaseHeadquarters";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ExobaseHeadquarters", 3, 3, "porta_pod_y_kanim", 250, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER5);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.ShowInBuildMenu = true;
		buildingDef.DefaultAnimState = "idle";
		buildingDef.OnePerWorld = true;
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoreBearer>();
		Telepad telepad = go.AddOrGet<Telepad>();
		telepad.startingSkillPoints = 1f;
		SocialGatheringPoint socialGatheringPoint = go.AddOrGet<SocialGatheringPoint>();
		socialGatheringPoint.choreOffsets = new CellOffset[6]
		{
			new CellOffset(-1, 0),
			new CellOffset(-2, 0),
			new CellOffset(2, 0),
			new CellOffset(3, 0),
			new CellOffset(0, 0),
			new CellOffset(1, 0)
		};
		socialGatheringPoint.choreCount = 4;
		socialGatheringPoint.basePriority = RELAXATION.PRIORITY.TIER0;
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.Color = LIGHT2D.HEADQUARTERS_COLOR;
		light2D.Range = 5f;
		light2D.Offset = LIGHT2D.EXOBASE_HEADQUARTERS_OFFSET;
		light2D.overlayColour = LIGHT2D.HEADQUARTERS_OVERLAYCOLOR;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
		go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental);
		RoleStation roleStation = go.AddOrGet<RoleStation>();
		roleStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_hqbase_skill_upgrade_kanim")
		};
		roleStation.workAnims = new HashedString[1]
		{
			"upgrade"
		};
		roleStation.workingPstComplete = null;
		roleStation.workingPstFailed = null;
		Activatable activatable = go.AddOrGet<Activatable>();
		activatable.synchronizeAnims = false;
		activatable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_warp_conduit_sender_kanim")
		};
		activatable.workAnims = new HashedString[2]
		{
			"sending_pre",
			"sending_loop"
		};
		activatable.workingPstComplete = new HashedString[1]
		{
			"sending_pst"
		};
		activatable.workingPstFailed = new HashedString[1]
		{
			"sending_pre"
		};
		activatable.SetWorkTime(30f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
