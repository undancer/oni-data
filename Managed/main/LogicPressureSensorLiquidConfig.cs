using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicPressureSensorLiquidConfig : IBuildingConfig
{
	public static string ID = "LogicPressureSensorLiquid";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchliquidpressure_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICPRESSURESENSORLIQUID.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICPRESSURESENSORLIQUID.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICPRESSURESENSORLIQUID.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicPressureSensor logicPressureSensor = go.AddOrGet<LogicPressureSensor>();
		logicPressureSensor.rangeMin = 0f;
		logicPressureSensor.rangeMax = 2000f;
		logicPressureSensor.Threshold = 500f;
		logicPressureSensor.ActivateAboveThreshold = false;
		logicPressureSensor.manuallyControlled = false;
		logicPressureSensor.desiredState = Element.State.Liquid;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
