using TUNING;
using UnityEngine;

public class SolidBoosterConfig : IBuildingConfig
{
	public const string ID = "SolidBooster";

	public const float capacity = 400f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidBooster", 7, 5, "rocket_solid_booster_kanim", 1000, 480f, BUILDINGS.ROCKETRY_MASS_KG.ENGINE_MASS_SMALL, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.ForbiddenDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.Invincible = true;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SolidBooster solidBooster = go.AddOrGet<SolidBooster>();
		solidBooster.mainEngine = false;
		solidBooster.efficiency = ROCKETRY.ENGINE_EFFICIENCY.BOOSTER;
		solidBooster.fuelTag = ElementLoader.FindElementByHash(SimHashes.Iron).tag;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.capacityKg = 800f;
		solidBooster.fuelStorage = storage;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = solidBooster.fuelTag;
		manualDeliveryKG.refillMass = storage.capacityKg / 2f;
		manualDeliveryKG.capacity = storage.capacityKg / 2f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = ElementLoader.FindElementByHash(SimHashes.OxyRock).tag;
		manualDeliveryKG2.refillMass = storage.capacityKg / 2f;
		manualDeliveryKG2.capacity = storage.capacityKg / 2f;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		BuildingTemplates.ExtendBuildingToRocketModule(go, "rocket_solid_booster_bg_kanim");
	}
}