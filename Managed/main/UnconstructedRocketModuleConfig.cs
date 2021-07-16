using TUNING;
using UnityEngine;

public class UnconstructedRocketModuleConfig : IBuildingConfig
{
	public const string ID = "UnconstructedRocketModule";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("UnconstructedRocketModule", 5, 5, "rocket_oxidizer_tank_bg_kanim", 1000, 0f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.DefaultAnimState = "on";
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.InputConduitType = ConduitType.None;
		obj.RequiresPowerInput = false;
		obj.CanMove = true;
		obj.Cancellable = false;
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

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<CopyBuildingSettings>();
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.INSIGNIFICANT);
	}
}
