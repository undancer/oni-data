using TUNING;
using UnityEngine;

public class BedConfig : IBuildingConfig
{
	public static string ID = "Bed";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "bedlg_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Bed);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		Bed bed = go.AddOrGet<Bed>();
		bed.effects = new string[2] { "BedStamina", "BedHealth" };
		bed.workLayer = Grid.SceneLayer.BuildingFront;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_sleep_bed_kanim") };
		sleepable.workLayer = Grid.SceneLayer.BuildingFront;
		go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Bed.Id;
		go.AddOrGetDef<RocketUsageRestriction.Def>();
	}
}
