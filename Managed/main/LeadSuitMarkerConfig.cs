using TUNING;
using UnityEngine;

public class LeadSuitMarkerConfig : IBuildingConfig
{
	public const string ID = "LeadSuitMarker";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: rEFINED_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "LeadSuitMarker", width: 2, height: 4, anim: "changingarea_radiation_arrow_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.Deprecated = !Sim.IsRadiationEnabled();
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "LeadSuitMarker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitMarker suitMarker = go.AddOrGet<SuitMarker>();
		suitMarker.LockerTags = new Tag[1]
		{
			new Tag("LeadSuitLocker")
		};
		suitMarker.PathFlag = PathFinder.PotentialPath.Flags.HasAtmoSuit;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[2]
		{
			new Tag("LeadSuitMarker"),
			new Tag("LeadSuitLocker")
		};
		go.AddTag(GameTags.JetSuitBlocker);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
