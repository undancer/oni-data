using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidConduitElementSensorConfig : ConduitSensorConfig
{
	public static string ID = "SolidConduitElementSensor";

	protected override ConduitType ConduitType => ConduitType.Solid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef(ID, "conveyor_element_sensor_kanim", TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		});
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
		return result;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Solid;
		ConduitElementSensor conduitElementSensor = go.AddOrGet<ConduitElementSensor>();
		conduitElementSensor.manuallyControlled = false;
		conduitElementSensor.conduitType = ConduitType;
		conduitElementSensor.defaultState = false;
	}
}
