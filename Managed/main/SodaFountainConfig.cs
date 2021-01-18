using TUNING;
using UnityEngine;

public class SodaFountainConfig : IBuildingConfig
{
	public const string ID = "SodaFountain";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SodaFountain", 2, 2, "sodamaker_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(1, 1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 20f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = SimHashes.CarbonDioxide.CreateTag();
		manualDeliveryKG.capacity = 4f;
		manualDeliveryKG.refillMass = 1f;
		manualDeliveryKG.minimumMass = 0.5f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		SodaFountainWorkable sodaFountainWorkable = go.AddOrGet<SodaFountainWorkable>();
		sodaFountainWorkable.basePriority = RELAXATION.PRIORITY.TIER5;
		SodaFountain sodaFountain = go.AddOrGet<SodaFountain>();
		sodaFountain.specificEffect = "SodaFountain";
		sodaFountain.trackingEffect = "RecentlyRecDrink";
		sodaFountain.ingredientTag = SimHashes.CarbonDioxide.CreateTag();
		sodaFountain.ingredientMassPerUse = 1f;
		sodaFountain.waterMassPerUse = 5f;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
