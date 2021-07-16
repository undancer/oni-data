using TUNING;
using UnityEngine;

public class ShearingStationConfig : IBuildingConfig
{
	public const string ID = "ShearingStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ShearingStation", 3, 3, "shearing_station_kanim", 100, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.Floodable = true;
		obj.Entombable = true;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.DefaultAnimState = "on";
		obj.ShowInBuildMenu = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStation);
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = (GameObject creature_go, RanchStation.Instance ranch_station_smi) => creature_go.GetSMI<ScaleGrowthMonitor.Instance>()?.IsFullyGrown() ?? false;
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			creature_go.GetSMI<ScaleGrowthMonitor.Instance>().Shear();
		};
		def.interactLoopCount = 6;
		def.rancherInteractAnim = "anim_interacts_shearingstation_kanim";
		def.synchronizeBuilding = true;
		Prioritizable.AddRef(go);
	}
}
