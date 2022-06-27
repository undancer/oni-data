using TUNING;
using UnityEngine;

public class HeadquartersConfig : IBuildingConfig
{
	public const string ID = "Headquarters";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Headquarters", 4, 4, "hqbase_kanim", 250, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER5);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = 400f;
		obj.ShowInBuildMenu = false;
		obj.DefaultAnimState = "idle";
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoreBearer>();
		Telepad telepad = go.AddOrGet<Telepad>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Telepad);
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
		light2D.Offset = LIGHT2D.HEADQUARTERS_OFFSET;
		light2D.overlayColour = LIGHT2D.HEADQUARTERS_OVERLAYCOLOR;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
		go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental);
		RoleStation roleStation = go.AddOrGet<RoleStation>();
		roleStation.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_hqbase_skill_upgrade_kanim") };
		roleStation.workAnims = new HashedString[1] { "upgrade" };
		roleStation.workingPstComplete = null;
		roleStation.workingPstFailed = null;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
