using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicInterasteroidReceiverConfig : IBuildingConfig
{
	public const string ID = "LogicInterasteroidReceiver";

	public const string OUTPUT_PORT_ID = "OutputPort";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LogicInterasteroidReceiver", 1, 1, "inter_asteroid_automation_signal_receiver_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
		buildingDef.AlwaysOperational = false;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("OutputPort", new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDRECEIVER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDRECEIVER.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICINTERASTEROIDRECEIVER.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true) };
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicInterasteroidReceiver");
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicBroadcastReceiver>().PORT_ID = "OutputPort";
	}
}
