using TUNING;
using UnityEngine;

public class WireBridgeConfig : IBuildingConfig
{
	public const string ID = "WireBridge";

	protected virtual string GetID()
	{
		return "WireBridge";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(GetID(), 3, 1, "utilityelectricbridge_kanim", 30, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.WireBridge, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	protected virtual WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = go.AddOrGet<WireUtilityNetworkLink>();
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max1000;
		wireUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		wireUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return wireUtilityNetworkLink;
	}
}
