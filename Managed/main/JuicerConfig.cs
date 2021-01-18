using TUNING;
using UnityEngine;

public class JuicerConfig : IBuildingConfig
{
	public const string ID = "Juicer";

	public const float BERRY_CALS = 600000f;

	public const float MUSHROOM_CALS = 300000f;

	public const float LICE_CALS = 500000f;

	public const float WATER_MASS_PER_USE = 1f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Juicer", 3, 4, "juicer_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(1, 1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
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
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = MushroomConfig.ID.ToTag();
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = PrickleFruitConfig.ID.ToTag();
		manualDeliveryKG2.capacity = 10f;
		manualDeliveryKG2.refillMass = 5f;
		manualDeliveryKG2.minimumMass = 1f;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG3 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG3.SetStorage(storage);
		manualDeliveryKG3.requestedItemTag = "BasicPlantFood".ToTag();
		manualDeliveryKG3.capacity = 10f;
		manualDeliveryKG3.refillMass = 5f;
		manualDeliveryKG3.minimumMass = 1f;
		manualDeliveryKG3.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		JuicerWorkable juicerWorkable = go.AddOrGet<JuicerWorkable>();
		juicerWorkable.basePriority = RELAXATION.PRIORITY.TIER5;
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(MushroomConfig.ID);
		EdiblesManager.FoodInfo foodInfo2 = EdiblesManager.GetFoodInfo(PrickleFruitConfig.ID);
		EdiblesManager.FoodInfo foodInfo3 = EdiblesManager.GetFoodInfo("BasicPlantFood");
		Juicer juicer = go.AddOrGet<Juicer>();
		juicer.ingredientTags = new Tag[3]
		{
			MushroomConfig.ID.ToTag(),
			PrickleFruitConfig.ID.ToTag(),
			"BasicPlantFood".ToTag()
		};
		juicer.ingredientMassesPerUse = new float[3]
		{
			300000f / foodInfo.CaloriesPerUnit,
			600000f / foodInfo2.CaloriesPerUnit,
			500000f / foodInfo3.CaloriesPerUnit
		};
		juicer.specificEffect = "Juicer";
		juicer.trackingEffect = "RecentlyRecDrink";
		juicer.waterMassPerUse = 1f;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
