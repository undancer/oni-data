using TUNING;
using UnityEngine;

public class FirePoleConfig : IBuildingConfig
{
	public const string ID = "FirePole";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FirePole", 1, 1, "firepole_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		BuildingTemplates.CreateLadderDef(buildingDef);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		Ladder ladder = go.AddOrGet<Ladder>();
		ladder.isPole = true;
		ladder.upwardsMovementSpeedMultiplier = 0.25f;
		ladder.downwardsMovementSpeedMultiplier = 4f;
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
