using TUNING;
using UnityEngine;

public class OxygenMaskMarkerConfig : IBuildingConfig
{
	public const string ID = "OxygenMaskMarker";

	public override BuildingDef CreateBuildingDef()
	{
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: rAW_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "OxygenMaskMarker", width: 1, height: 2, anim: "oxygen_checkpoint_arrow_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "OxygenMaskMarker");
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitMarker suitMarker = go.AddOrGet<SuitMarker>();
		suitMarker.LockerTags = new Tag[1]
		{
			new Tag("OxygenMaskLocker")
		};
		suitMarker.PathFlag = PathFinder.PotentialPath.Flags.HasOxygenMask;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[2]
		{
			new Tag("OxygenMaskMarker"),
			new Tag("OxygenMaskLocker")
		};
		go.AddTag(GameTags.JetSuitBlocker);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
