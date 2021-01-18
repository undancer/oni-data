using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LogicGateBase")]
public class LogicGateBase : KMonoBehaviour
{
	public enum PortId
	{
		InputOne,
		InputTwo,
		InputThree,
		InputFour,
		OutputOne,
		OutputTwo,
		OutputThree,
		OutputFour,
		ControlOne,
		ControlTwo
	}

	public enum Op
	{
		And,
		Or,
		Not,
		Xor,
		CustomSingle,
		Multiplexer,
		Demultiplexer
	}

	public static LogicModeUI uiSrcData;

	public static readonly HashedString OUTPUT_TWO_PORT_ID = new HashedString("LogicGateOutputTwo");

	public static readonly HashedString OUTPUT_THREE_PORT_ID = new HashedString("LogicGateOutputThree");

	public static readonly HashedString OUTPUT_FOUR_PORT_ID = new HashedString("LogicGateOutputFour");

	[SerializeField]
	public Op op;

	public static CellOffset[] portOffsets = new CellOffset[3]
	{
		CellOffset.none,
		new CellOffset(0, 1),
		new CellOffset(1, 0)
	};

	public CellOffset[] inputPortOffsets;

	public CellOffset[] outputPortOffsets;

	public CellOffset[] controlPortOffsets;

	public int InputCellOne => GetActualCell(inputPortOffsets[0]);

	public int InputCellTwo => GetActualCell(inputPortOffsets[1]);

	public int InputCellThree => GetActualCell(inputPortOffsets[2]);

	public int InputCellFour => GetActualCell(inputPortOffsets[3]);

	public int OutputCellOne => GetActualCell(outputPortOffsets[0]);

	public int OutputCellTwo => GetActualCell(outputPortOffsets[1]);

	public int OutputCellThree => GetActualCell(outputPortOffsets[2]);

	public int OutputCellFour => GetActualCell(outputPortOffsets[3]);

	public int ControlCellOne => GetActualCell(controlPortOffsets[0]);

	public int ControlCellTwo => GetActualCell(controlPortOffsets[1]);

	public bool RequiresTwoInputs => OpRequiresTwoInputs(op);

	public bool RequiresFourInputs => OpRequiresFourInputs(op);

	public bool RequiresFourOutputs => OpRequiresFourOutputs(op);

	public bool RequiresControlInputs => OpRequiresControlInputs(op);

	private int GetActualCell(CellOffset offset)
	{
		Rotatable component = GetComponent<Rotatable>();
		if (component != null)
		{
			offset = component.GetRotatedCellOffset(offset);
		}
		return Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset);
	}

	public int PortCell(PortId port)
	{
		return port switch
		{
			PortId.InputOne => InputCellOne, 
			PortId.InputTwo => InputCellTwo, 
			PortId.InputThree => InputCellThree, 
			PortId.InputFour => InputCellFour, 
			PortId.OutputOne => OutputCellOne, 
			PortId.OutputTwo => OutputCellTwo, 
			PortId.OutputThree => OutputCellThree, 
			PortId.OutputFour => OutputCellFour, 
			PortId.ControlOne => ControlCellOne, 
			PortId.ControlTwo => ControlCellTwo, 
			_ => OutputCellOne, 
		};
	}

	public bool TryGetPortAtCell(int cell, out PortId port)
	{
		if (cell == InputCellOne)
		{
			port = PortId.InputOne;
			return true;
		}
		if ((RequiresTwoInputs || RequiresFourInputs) && cell == InputCellTwo)
		{
			port = PortId.InputTwo;
			return true;
		}
		if (RequiresFourInputs && cell == InputCellThree)
		{
			port = PortId.InputThree;
			return true;
		}
		if (RequiresFourInputs && cell == InputCellFour)
		{
			port = PortId.InputFour;
			return true;
		}
		if (cell == OutputCellOne)
		{
			port = PortId.OutputOne;
			return true;
		}
		if (RequiresFourOutputs && cell == OutputCellTwo)
		{
			port = PortId.OutputTwo;
			return true;
		}
		if (RequiresFourOutputs && cell == OutputCellThree)
		{
			port = PortId.OutputThree;
			return true;
		}
		if (RequiresFourOutputs && cell == OutputCellFour)
		{
			port = PortId.OutputFour;
			return true;
		}
		if (RequiresControlInputs && cell == ControlCellOne)
		{
			port = PortId.ControlOne;
			return true;
		}
		if (RequiresControlInputs && cell == ControlCellTwo)
		{
			port = PortId.ControlTwo;
			return true;
		}
		port = PortId.InputOne;
		return false;
	}

	public static bool OpRequiresTwoInputs(Op op)
	{
		if (op == Op.Not || (uint)(op - 4) <= 2u)
		{
			return false;
		}
		return true;
	}

	public static bool OpRequiresFourInputs(Op op)
	{
		if (op == Op.Multiplexer)
		{
			return true;
		}
		return false;
	}

	public static bool OpRequiresFourOutputs(Op op)
	{
		if (op == Op.Demultiplexer)
		{
			return true;
		}
		return false;
	}

	public static bool OpRequiresControlInputs(Op op)
	{
		if ((uint)(op - 5) <= 1u)
		{
			return true;
		}
		return false;
	}
}
