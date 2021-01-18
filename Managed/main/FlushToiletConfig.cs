using TUNING;
using UnityEngine;

public class FlushToiletConfig : IBuildingConfig
{
	private const float WATER_USAGE = 5f;

	public const string ID = "FlushToilet";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FlushToilet", 2, 3, "toiletflush_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.DiseaseCellVisName = "FoodPoisoning";
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		SoundEventVolumeCache.instance.AddVolume("toiletflush_kanim", "Lavatory_flush", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("toiletflush_kanim", "Lavatory_door_close", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("toiletflush_kanim", "Lavatory_door_open", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Toilet);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.FlushToilet);
		FlushToilet flushToilet = go.AddOrGet<FlushToilet>();
		flushToilet.massConsumedPerUse = 5f;
		flushToilet.massEmittedPerUse = 11.7f;
		flushToilet.newPeeTemperature = 310.15f;
		flushToilet.diseaseId = "FoodPoisoning";
		flushToilet.diseasePerFlush = 100000;
		flushToilet.diseaseOnDupePerFlush = 5000;
		KAnimFile[] overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_toiletflush_kanim")
		};
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.overrideAnims = overrideAnims;
		toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingFront;
		toiletWorkableUse.resetProgressOnStop = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 25f;
		storage.doDiseaseTransfer = false;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.Toilet.Id;
		ownable.canBePublic = true;
		RequireOutputs requireOutputs = go.AddOrGet<RequireOutputs>();
		requireOutputs.ignoreFullPipe = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
