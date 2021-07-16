using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicPowerRelayConfig : IBuildingConfig
{
	public static string ID = "LogicPowerRelay";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchpowershutoff_kanim", 10, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICPOWERRELAY.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICPOWERRELAY.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICPOWERRELAY.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<OperationalControlledSwitch>().objectLayer = ObjectLayer.Wire;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
