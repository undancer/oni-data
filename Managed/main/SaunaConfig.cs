using TUNING;
using UnityEngine;

public class SaunaConfig : IBuildingConfig
{
	public const string ID = "Sauna";

	private const float STEAM_PER_USE_KG = 25f;

	private const float WATER_OUTPUT_TEMP = 353.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Sauna", 3, 3, "sauna_kanim", 30, 60f, new float[2]
		{
			100f,
			100f
		}, new string[2]
		{
			"Metal",
			"BuildingWood"
		}, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER2);
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 2);
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Steam).tag;
		conduitConsumer.capacityKG = 50f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.alwaysConsume = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		SaunaWorkable saunaWorkable = go.AddOrGet<SaunaWorkable>();
		saunaWorkable.basePriority = RELAXATION.PRIORITY.TIER3;
		Sauna sauna = go.AddOrGet<Sauna>();
		sauna.steamPerUseKG = 25f;
		sauna.waterOutputTemp = 353.15f;
		sauna.specificEffect = "Sauna";
		sauna.trackingEffect = "RecentlySauna";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}
}
