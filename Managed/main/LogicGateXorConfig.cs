using STRINGS;

public class LogicGateXorConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateXOR";

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
		return LogicGateBase.Op.Xor;
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
		return CreateBuildingDef("LogicGateXOR", "logic_xor_kanim");
	}
}
