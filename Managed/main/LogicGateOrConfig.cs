using STRINGS;

public class LogicGateOrConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateOR";

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
		return LogicGateBase.Op.Or;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			outputOne = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateOR", "logic_or_kanim");
	}
}
