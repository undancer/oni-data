using STRINGS;

public class LogicGateDemultiplexerConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateDemultiplexer";

	protected override CellOffset[] InputPortOffsets => new CellOffset[1]
	{
		new CellOffset(-1, 3)
	};

	protected override CellOffset[] OutputPortOffsets => new CellOffset[4]
	{
		new CellOffset(1, 3),
		new CellOffset(1, 2),
		new CellOffset(1, 1),
		new CellOffset(1, 0)
	};

	protected override CellOffset[] ControlPortOffsets => new CellOffset[2]
	{
		new CellOffset(-1, 0),
		new CellOffset(0, 0)
	};

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Demultiplexer;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		return new LogicGate.LogicGateDescriptions
		{
			outputOne = new LogicGate.LogicGateDescriptions.Description
			{
				name = BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_NAME,
				active = BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_ACTIVE,
				inactive = BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_INACTIVE
			}
		};
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateDemultiplexer", "logic_demultiplexer_kanim", 3, 4);
	}
}
