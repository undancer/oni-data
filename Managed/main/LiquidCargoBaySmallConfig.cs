using TUNING;
using UnityEngine;

public class LiquidCargoBaySmallConfig : IBuildingConfig
{
	public const string ID = "LiquidCargoBaySmall";

	public float CAPACITY = 900f * ROCKETRY.CARGO_CAPACITY_SCALE;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidCargoBaySmall", 3, 3, "rocket_storage_liquid_small_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.CanMove = true;
		obj.Cancellable = false;
		obj.attachablePosition = new CellOffset(0, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go = BuildingTemplates.ExtendBuildingToClusterCargoBay(go, CAPACITY, STORAGEFILTERS.LIQUIDS, CargoBay.CargoType.Liquids);
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR_PLUS);
	}
}
