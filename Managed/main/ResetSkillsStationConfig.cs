using TUNING;
using UnityEngine;

public class ResetSkillsStationConfig : IBuildingConfig
{
	public const string ID = "ResetSkillsStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ResetSkillsStation", 3, 3, "reSpeccer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddTag(GameTags.NotRoomAssignable);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.ResetSkillsStation.Id;
		ResetSkillsStation resetSkillsStation = go.AddOrGet<ResetSkillsStation>();
		resetSkillsStation.workTime = 180f;
		resetSkillsStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_reSpeccer_kanim")
		};
		resetSkillsStation.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
