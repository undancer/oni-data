using STRINGS;

public class LogicGateAndConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateAND";

	protected override CellOffset[] InputPortOffsets => new CellOffset[2]
	{
		CellOffset.none,
		new CellOffset(0, 1)
	};

	protected override CellOffset[] OutputPortOffsets => new CellOffset[1]
	{
		new CellOffset(1, 0)
	};

	protected override CellOffset[] ControlPortOffsets => null;

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			outputOne = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateAND", "logic_and_kanim");
	}
}
