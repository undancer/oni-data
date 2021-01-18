using TUNING;
using UnityEngine;

public class SuitMarkerConfig : IBuildingConfig
{
	public const string ID = "SuitMarker";

	public override BuildingDef CreateBuildingDef()
	{
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: rEFINED_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "SuitMarker", width: 1, height: 3, anim: "changingarea_arrow_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitMarker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitMarker suitMarker = go.AddOrGet<SuitMarker>();
		suitMarker.LockerTags = new Tag[1]
		{
			new Tag("SuitLocker")
		};
		suitMarker.PathFlag = PathFinder.PotentialPath.Flags.HasAtmoSuit;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[2]
		{
			new Tag("SuitMarker"),
			new Tag("SuitLocker")
		};
		go.AddTag(GameTags.JetSuitBlocker);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
