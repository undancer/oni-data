using TUNING;
using UnityEngine;

public class TravelTubeEntranceConfig : IBuildingConfig
{
	public const string ID = "TravelTubeEntrance";

	private const float JOULES_PER_LAUNCH = 10000f;

	private const float LAUNCHES_FROM_FULL_CHARGE = 4f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("TravelTubeEntrance", 3, 2, "tube_launcher_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Overheatable = false;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 960f;
		obj.Entombable = true;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		TravelTubeEntrance travelTubeEntrance = go.AddOrGet<TravelTubeEntrance>();
		travelTubeEntrance.joulesPerLaunch = 10000f;
		travelTubeEntrance.jouleCapacity = 40000f;
		go.AddOrGet<TravelTubeEntrance.Work>();
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<EnergyConsumerSelfSustaining>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().visualizeRequirements = false;
	}
}
