using TUNING;
using UnityEngine;

public class DoctorStationConfig : IBuildingConfig
{
	public const string ID = "DoctorStation";

	private static Tag SUPPLY_TAG = "IntermediateCure";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("DoctorStation", 3, 2, "treatment_chair_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
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
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = SUPPLY_TAG;
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.DoctorFetch.IdHash;
		manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
		DoctorStation doctorStation = go.AddOrGet<DoctorStation>();
		doctorStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_treatment_chair_sick_kanim")
		};
		doctorStation.workLayer = Grid.SceneLayer.BuildingFront;
		doctorStation.supplyTag = SUPPLY_TAG;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
		roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
		roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
		DoctorStationDoctorWorkable doctorStationDoctorWorkable = go.AddOrGet<DoctorStationDoctorWorkable>();
		doctorStationDoctorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_treatment_chair_doctor_kanim")
		};
		doctorStationDoctorWorkable.SetWorkTime(40f);
		doctorStationDoctorWorkable.requiredSkillPerk = Db.Get().SkillPerks.CanDoctor.Id;
	}
}
