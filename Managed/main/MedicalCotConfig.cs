using TUNING;
using UnityEngine;

public class MedicalCotConfig : IBuildingConfig
{
	public const string ID = "MedicalCot";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MedicalCot", 3, 2, "medical_cot_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Clinic);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed"));
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.doctorVisitInterval = 300f;
		clinic.workerInjuredAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_healing_bed_kanim")
		};
		clinic.workerDiseasedAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_med_cot_sick_kanim")
		};
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		string text = "MedicalCot";
		string text2 = "MedicalCotDoctored";
		clinic.healthEffect = text;
		clinic.doctoredHealthEffect = text2;
		clinic.diseaseEffect = text;
		clinic.doctoredDiseaseEffect = text2;
		clinic.doctoredPlaceholderEffect = "DoctoredOffCotEffect";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
		roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
		roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
		go.AddOrGet<Sleepable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_med_cot_sick_kanim")
		};
		DoctorChoreWorkable doctorChoreWorkable = go.AddOrGet<DoctorChoreWorkable>();
		doctorChoreWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_med_cot_doctor_kanim")
		};
		doctorChoreWorkable.workTime = 45f;
		go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Clinic.Id;
	}
}
