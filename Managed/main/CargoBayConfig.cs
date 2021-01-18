using TUNING;
using UnityEngine;

public class CargoBayConfig : IBuildingConfig
{
	public const string ID = "CargoBay";

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"BuildableRaw",
			SimHashes.Steel.ToString()
		};
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS,
			ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS
		}, construction_materials: array, melting_point: 9999f, build_location_rule: BuildLocationRule.BuildingAttachPoint, noise: NOISE_POLLUTION.NOISY.TIER2, id: "CargoBay", width: 5, height: 5, anim: "rocket_storage_solid_kanim", hitpoints: 1000, construction_time: 60f, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.Invincible = true;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.CanMove = true;
		obj.OutputConduitType = ConduitType.Solid;
		obj.UtilityOutputOffset = new CellOffset(0, 3);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		CargoBay cargoBay = go.AddOrGet<CargoBay>();
		cargoBay.storage = go.AddOrGet<Storage>();
		cargoBay.storageType = CargoBay.CargoType.solids;
		cargoBay.storage.capacityKg = 1000f;
		cargoBay.storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<RocketModule>().SetBGKAnim(Assets.GetAnim("rocket_storage_solid_bg_kanim"));
		EntityTemplates.ExtendBuildingToRocketModule(go);
		go.AddOrGet<SolidConduitDispenser>();
	}
}
