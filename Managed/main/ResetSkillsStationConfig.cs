using TUNING;
using UnityEngine;

public class ResetSkillsStationConfig : IBuildingConfig
{
	public const string ID = "ResetSkillsStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ResetSkillsStation", 3, 3, "reSpeccer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 480f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddTag(GameTags.NotRoomAssignable);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.ResetSkillsStation.Id;
		ResetSkillsStation resetSkillsStation = go.AddOrGet<ResetSkillsStation>();
		resetSkillsStation.workTime = 180f;
		resetSkillsStation.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_reSpeccer_kanim") };
		resetSkillsStation.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
