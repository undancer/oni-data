using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OxidizerTankConfig : IBuildingConfig
{
	public const string ID = "OxidizerTank";

	public const float FuelCapacity = 2700f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("OxidizerTank", 5, 5, "rocket_oxidizer_tank_kanim", 1000, 60f, BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_DRY_MASS, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 9999f, BuildLocationRule.BuildingAttachPoint, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.DefaultAnimState = "grounded";
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
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
		storage.capacityKg = 2700f;
		storage.allowSublimation = false;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		go.AddOrGet<OxidizerTank>().storage = storage;
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<DropToUserCapacity>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = ElementLoader.FindElementByHash(SimHashes.OxyRock).tag;
		manualDeliveryKG.refillMass = storage.capacityKg;
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.None;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		go.AddOrGet<RocketModule>().SetBGKAnim(Assets.GetAnim("rocket_oxidizer_tank_bg_kanim"));
		EntityTemplates.ExtendBuildingToRocketModule(go);
	}
}
