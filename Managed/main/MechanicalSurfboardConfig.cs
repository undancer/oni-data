using TUNING;
using UnityEngine;

public class MechanicalSurfboardConfig : IBuildingConfig
{
	public const string ID = "MechanicalSurfboard";

	private const float TANK_SIZE_KG = 20f;

	private const float SPILL_RATE_KG = 0.05f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MechanicalSurfboard", 2, 3, "mechanical_surfboard_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(1, 0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
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
		MechanicalSurfboardWorkable mechanicalSurfboardWorkable = go.AddOrGet<MechanicalSurfboardWorkable>();
		mechanicalSurfboardWorkable.basePriority = RELAXATION.PRIORITY.TIER3;
		MechanicalSurfboard mechanicalSurfboard = go.AddOrGet<MechanicalSurfboard>();
		mechanicalSurfboard.waterSpillRateKG = 0.05f;
		mechanicalSurfboard.minOperationalWaterKG = 2f;
		mechanicalSurfboard.specificEffect = "MechanicalSurfboard";
		mechanicalSurfboard.trackingEffect = "RecentlyMechanicalSurfboard";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
