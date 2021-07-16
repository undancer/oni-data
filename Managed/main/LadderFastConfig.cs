using TUNING;
using UnityEngine;

public class LadderFastConfig : IBuildingConfig
{
	public const string ID = "LadderFast";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LadderFast", 1, 1, "ladder_plastic_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.PLASTICS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		BuildingTemplates.CreateLadderDef(obj);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.AudioCategory = "Plastic";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.DragBuild = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		Ladder ladder = go.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.2f;
		ladder.downwardsMovementSpeedMultiplier = 1.2f;
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
