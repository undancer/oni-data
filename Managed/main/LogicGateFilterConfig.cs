using STRINGS;
using UnityEngine;

public class LogicGateFilterConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateFILTER";

	protected override CellOffset[] InputPortOffsets => new CellOffset[1]
	{
		CellOffset.none
	};

	protected override CellOffset[] OutputPortOffsets => new CellOffset[1]
	{
		new CellOffset(1, 0)
	};

	protected override CellOffset[] ControlPortOffsets => null;

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			outputOne = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateFILTER", "logic_filter_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateFilter logicGateFilter = go.AddComponent<LogicGateFilter>();
		logicGateFilter.op = GetLogicOp();
		logicGateFilter.inputPortOffsets = InputPortOffsets;
		logicGateFilter.outputPortOffsets = OutputPortOffsets;
		logicGateFilter.controlPortOffsets = ControlPortOffsets;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateFilter component = game_object.GetComponent<LogicGateFilter>();
			component.SetPortDescriptions(GetDescriptions());
		};
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
