using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidConduitElementSensorConfig : ConduitSensorConfig
{
	public static string ID = "LiquidConduitElementSensor";

	protected override ConduitType ConduitType => ConduitType.Liquid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef(ID, "liquid_element_sensor_kanim", TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITELEMENTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITELEMENTSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITELEMENTSENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		});
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
		return result;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		Filterable filterable = go.AddOrGet<Filterable>();
		filterable.filterElementState = Filterable.ElementState.Liquid;
		ConduitElementSensor conduitElementSensor = go.AddOrGet<ConduitElementSensor>();
		conduitElementSensor.manuallyControlled = false;
		conduitElementSensor.conduitType = ConduitType;
		conduitElementSensor.defaultState = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
