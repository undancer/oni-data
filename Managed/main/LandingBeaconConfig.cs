using TUNING;
using UnityEngine;

public class LandingBeaconConfig : IBuildingConfig
{
	public const string ID = "LandingBeacon";

	public const int LANDING_ACCURACY = 3;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LandingBeacon", 1, 3, "landing_beacon_kanim", 1000, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.OverheatTemperature = 398.15f;
		obj.Floodable = false;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.CanMove = false;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ViewMode = OverlayModes.Power.ID;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGetDef<LandingBeacon.Def>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
