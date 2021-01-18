using TUNING;
using UnityEngine;

public class OuthouseConfig : IBuildingConfig
{
	public const string ID = "Outhouse";

	private const int USES_PER_REFILL = 15;

	private const float DIRT_PER_REFILL = 200f;

	private const float DIRT_PER_USE = 13f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Outhouse", 2, 3, "outhouse_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER4);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.DiseaseCellVisName = "FoodPoisoning";
		buildingDef.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_close", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Toilet);
		Toilet toilet = go.AddOrGet<Toilet>();
		toilet.maxFlushes = 15;
		toilet.dirtUsedPerFlush = 13f;
		toilet.solidWastePerUse = new Toilet.SpawnInfo(SimHashes.ToxicSand, 6.7f, 0f);
		toilet.solidWasteTemperature = 310.15f;
		toilet.diseaseId = "FoodPoisoning";
		toilet.diseasePerFlush = 100000;
		toilet.diseaseOnDupePerFlush = 100000;
		KAnimFile[] overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_outhouse_kanim")
		};
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.overrideAnims = overrideAnims;
		toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingFront;
		ToiletWorkableClean toiletWorkableClean = go.AddOrGet<ToiletWorkableClean>();
		toiletWorkableClean.workTime = 90f;
		toiletWorkableClean.overrideAnims = overrideAnims;
		toiletWorkableClean.workLayer = Grid.SceneLayer.BuildingFront;
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Dirt");
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 0.01f;
		manualDeliveryKG.minimumMass = 200f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.Toilet.Id;
		ownable.canBePublic = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
