using STRINGS;
using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateBUFFER";

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
				name = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateBUFFER", "logic_buffer_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateBuffer logicGateBuffer = go.AddComponent<LogicGateBuffer>();
		logicGateBuffer.op = GetLogicOp();
		logicGateBuffer.inputPortOffsets = InputPortOffsets;
		logicGateBuffer.outputPortOffsets = OutputPortOffsets;
		logicGateBuffer.controlPortOffsets = ControlPortOffsets;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateBuffer component = game_object.GetComponent<LogicGateBuffer>();
			component.SetPortDescriptions(GetDescriptions());
		};
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
