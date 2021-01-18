using TUNING;
using UnityEngine;

public class TravelTubeEntranceConfig : IBuildingConfig
{
	public const string ID = "TravelTubeEntrance";

	private const float JOULES_PER_LAUNCH = 10000f;

	private const float LAUNCHES_FROM_FULL_CHARGE = 4f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TravelTubeEntrance", 3, 2, "tube_launcher_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
		return buildingDef;
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
