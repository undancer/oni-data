using TUNING;
using UnityEngine;

public class ArcadeMachineConfig : IBuildingConfig
{
	public const string ID = "ArcadeMachine";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ArcadeMachine", 3, 3, "arcade_cabinet_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.ViewMode = OverlayModes.Power.ID;
		obj.Floodable = true;
		obj.AudioCategory = "Metal";
		obj.Overheatable = true;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 1200f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
		go.AddOrGet<ArcadeMachine>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
