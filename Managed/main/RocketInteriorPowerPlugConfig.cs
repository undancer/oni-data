using TUNING;
using UnityEngine;

public class RocketInteriorPowerPlugConfig : IBuildingConfig
{
	public const string ID = "RocketInteriorPowerPlug";

	protected virtual string GetID()
	{
		return "RocketInteriorPowerPlug";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(GetID(), 1, 1, "rocket_floor_plug_kanim", 30, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnRocketEnvelope, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.RequiresPowerOutput = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "RocketInteriorPowerPlug");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding);
		go.AddComponent<RequireInputs>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<OperationalController.Def>();
		WireUtilitySemiVirtualNetworkLink wireUtilitySemiVirtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
		wireUtilitySemiVirtualNetworkLink.link1 = new CellOffset(0, 0);
	}
}
