using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RocketControlStationConfig : IBuildingConfig
{
	public static string ID = "RocketControlStation";

	public const float CONSOLE_WORK_TIME = 30f;

	public const float CONSOLE_IDLE_TIME = 120f;

	public const float WARNING_COOLDOWN = 30f;

	public const float DEFAULT_SPEED = 1f;

	public const float SLOW_SPEED = 0.5f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "rocket_control_station_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER2);
		obj.Overheatable = false;
		obj.Repairable = false;
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.DefaultAnimState = "off";
		obj.OnePerWorld = true;
		obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(RocketControlStation.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT_INACTIVE) };
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.RocketInteriorBuilding);
		component.AddTag(GameTags.UniquePerWorld);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<RocketControlStationIdleWorkable>().workLayer = Grid.SceneLayer.BuildingUse;
		go.AddOrGet<RocketControlStationLaunchWorkable>().workLayer = Grid.SceneLayer.BuildingUse;
		go.AddOrGet<RocketControlStation>();
		go.AddOrGetDef<PoweredController.Def>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RocketInterior);
	}
}
