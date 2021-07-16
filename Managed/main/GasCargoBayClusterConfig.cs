using TUNING;
using UnityEngine;

public class GasCargoBayClusterConfig : IBuildingConfig
{
	public const string ID = "GasCargoBayCluster";

	public float CAPACITY = ROCKETRY.GAS_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasCargoBayCluster", 5, 5, "rocket_cluster_storage_gas_kanim", 1000, 60f, BUILDINGS.ROCKETRY_MASS_KG.CARGO_MASS, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.CanMove = true;
		obj.Cancellable = false;
		obj.ShowInBuildMenu = false;
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
		go = BuildingTemplates.ExtendBuildingToClusterCargoBay(go, CAPACITY, STORAGEFILTERS.GASES, CargoBay.CargoType.Gasses);
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE);
	}
}
