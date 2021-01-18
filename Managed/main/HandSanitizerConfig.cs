using TUNING;
using UnityEngine;

public class HandSanitizerConfig : IBuildingConfig
{
	public const string ID = "HandSanitizer";

	private const float STORAGE_SIZE = 15f;

	private const float MASS_PER_USE = 0.07f;

	private const int DISEASE_REMOVAL_COUNT = 480000;

	private const float WORK_TIME = 1.8f;

	private const SimHashes CONSUMED_ELEMENT = SimHashes.BleachStone;

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"Metal",
			"BleachStone"
		};
		BuildingDef result = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: array, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "HandSanitizer", width: 2, height: 3, anim: "handsanitizer_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_out", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_in", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_slurp", NOISE_POLLUTION.NOISY.TIER0);
		return result;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.WashStation);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.AdvancedWashStation);
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 0.07f;
		handSanitizer.consumedElement = SimHashes.BleachStone;
		handSanitizer.diseaseRemovalCount = 480000;
		HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
		work.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_handsanitizer_kanim")
		};
		work.workTime = 1.8f;
		work.trackUses = true;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<DirectionControl>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTagExtensions.Create(SimHashes.BleachStone);
		manualDeliveryKG.capacity = 15f;
		manualDeliveryKG.refillMass = 3f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
