using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SteamEngineConfig : IBuildingConfig
{
	public const string ID = "SteamEngine";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SteamEngine", 7, 5, "rocket_steam_engine_kanim", 1000, 480f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.UtilityInputOffset = new CellOffset(2, 3);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		buildingDef.ShowInBuildMenu = !DlcManager.IsExpansion1Active();
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
		RocketEngine rocketEngine = go.AddOrGet<RocketEngine>();
		rocketEngine.maxModules = 5;
		rocketEngine.fuelTag = ElementLoader.FindElementByHash(SimHashes.Steam).tag;
		rocketEngine.efficiency = ROCKETRY.ENGINE_EFFICIENCY.WEAK;
		rocketEngine.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		rocketEngine.requireOxidizer = false;
		rocketEngine.exhaustElement = SimHashes.Steam;
		rocketEngine.exhaustTemperature = ElementLoader.FindElementByHash(SimHashes.Steam).lowTemp + 50f;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		FuelTank fuelTank = go.AddOrGet<FuelTank>();
		fuelTank.consumeFuelOnLand = !DlcManager.IsExpansion1Active();
		fuelTank.storage = storage;
		fuelTank.FuelType = ElementLoader.FindElementByHash(SimHashes.Steam).tag;
		fuelTank.physicalFuelCapacity = storage.capacityKg;
		go.AddOrGet<CopyBuildingSettings>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = fuelTank.FuelType;
		conduitConsumer.capacityKG = storage.capacityKg;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		BuildingTemplates.ExtendBuildingToRocketModule(go, ROCKETRY.BURDEN.MODERATE, "rocket_steam_engine_bg_kanim", ROCKETRY.ENGINE_POWER.WEAKER, ROCKETRY.FUEL_COST_PER_DISTANCE.LOW);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate
		{
		};
	}
}
