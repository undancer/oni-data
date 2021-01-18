using TUNING;
using UnityEngine;

public class ScannerModuleConfig : IBuildingConfig
{
	public const string ID = "ScannerModule";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ScannerModule", 5, 5, "rocket_scanner_module_kanim", 1000, 120f, new float[3]
		{
			350f,
			250f,
			1000f
		}, new string[3]
		{
			SimHashes.Steel.ToString(),
			SimHashes.Isoresin.ToString(),
			SimHashes.Polypropylene.ToString()
		}, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGetDef<ScannerModule.Def>();
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.ExtendBuildingToRocketModule(go, ROCKETRY.BURDEN.MINOR_PLUS, null);
	}
}
