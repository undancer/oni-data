using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LiquidFuelTankConfig : IBuildingConfig
{
	public const string ID = "LiquidFuelTank";

	public const float FuelCapacity = 900f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_VANILLA_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidFuelTank", 5, 5, "rocket_liquid_fuel_tank_kanim", 1000, 60f, BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_DRY_MASS, new string[1] { SimHashes.Steel.ToString() }, 9999f, BuildLocationRule.BuildingAttachPoint, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.UtilityInputOffset = new CellOffset(2, 3);
		obj.InputConduitType = ConduitType.Liquid;
		obj.RequiresPowerInput = false;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.CanMove = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		FuelTank fuelTank = go.AddOrGet<FuelTank>();
		fuelTank.consumeFuelOnLand = !DlcManager.FeatureClusterSpaceEnabled();
		fuelTank.storage = storage;
		fuelTank.physicalFuelCapacity = storage.capacityKg;
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<DropToUserCapacity>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.refillMass = storage.capacityKg;
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.None;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = storage.capacityKg;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		BuildingTemplates.ExtendBuildingToRocketModule(go, "rocket_liquid_fuel_tank_bg_kanim");
	}
}
