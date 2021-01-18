using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidConduitTemperatureSensorConfig : ConduitSensorConfig
{
	public static string ID = "LiquidConduitTemperatureSensor";

	protected override ConduitType ConduitType => ConduitType.Liquid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef(ID, "liquid_temperature_sensor_kanim", TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITTEMPERATURESENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITTEMPERATURESENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITTEMPERATURESENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		});
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
		return result;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitTemperatureSensor conduitTemperatureSensor = go.AddComponent<ConduitTemperatureSensor>();
		conduitTemperatureSensor.conduitType = ConduitType;
		conduitTemperatureSensor.Threshold = 280f;
		conduitTemperatureSensor.ActivateAboveThreshold = true;
		conduitTemperatureSensor.manuallyControlled = false;
		conduitTemperatureSensor.rangeMin = 0f;
		conduitTemperatureSensor.rangeMax = 9999f;
		conduitTemperatureSensor.defaultState = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
