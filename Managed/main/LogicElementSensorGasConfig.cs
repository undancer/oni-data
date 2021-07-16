using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicElementSensorGasConfig : IBuildingConfig
{
	public static string ID = "LogicElementSensorGas";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "world_element_sensor_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = true;
		obj.Entombable = true;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORGAS.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORGAS.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICELEMENTSENSORGAS.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		SoundEventVolumeCache.instance.AddVolume("world_element_sensor_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("world_element_sensor_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Gas;
		LogicElementSensor logicElementSensor = go.AddOrGet<LogicElementSensor>();
		logicElementSensor.manuallyControlled = false;
		logicElementSensor.desiredState = Element.State.Gas;
	}
}
