using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasConduitDiseaseSensorConfig : ConduitSensorConfig
{
	public static string ID = "GasConduitDiseaseSensor";

	protected override ConduitType ConduitType => ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef(ID, "gas_germs_sensor_kanim", new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[2]
		{
			"RefinedMetal",
			"Plastic"
		}, new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		});
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
		return result;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitDiseaseSensor conduitDiseaseSensor = go.AddComponent<ConduitDiseaseSensor>();
		conduitDiseaseSensor.conduitType = ConduitType;
		conduitDiseaseSensor.Threshold = 0f;
		conduitDiseaseSensor.ActivateAboveThreshold = true;
		conduitDiseaseSensor.manuallyControlled = false;
		conduitDiseaseSensor.defaultState = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
