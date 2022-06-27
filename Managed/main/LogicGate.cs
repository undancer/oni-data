using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGate : LogicGateBase, ILogicEventSender, ILogicNetworkConnection
{
	public class LogicGateDescriptions
	{
		public class Description
		{
			public string name;

			public string active;

			public string inactive;
		}

		public Description inputOne;

		public Description inputTwo;

		public Description inputThree;

		public Description inputFour;

		public Description outputOne;

		public Description outputTwo;

		public Description outputThree;

		public Description outputFour;

		public Description controlOne;

		public Description controlTwo;
	}

	private static readonly LogicGateDescriptions.Description INPUT_ONE_SINGLE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_ONE_MULTI_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_TWO_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_THREE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_FOUR_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description OUTPUT_ONE_SINGLE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description OUTPUT_ONE_MULTI_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description OUTPUT_TWO_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description OUTPUT_THREE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description OUTPUT_FOUR_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description CONTROL_ONE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description CONTROL_TWO_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_NAME,
		active = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_ACTIVE,
		inactive = UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_INACTIVE
	};

	private LogicGateDescriptions descriptions;

	private LogicEventSender[] additionalOutputs;

	private const bool IS_CIRCUIT_ENDPOINT = true;

	private bool connected;

	protected bool cleaningUp;

	private int lastAnimState = -1;

	[Serialize]
	protected int outputValueOne;

	[Serialize]
	protected int outputValueTwo;

	[Serialize]
	protected int outputValueThree;

	[Serialize]
	protected int outputValueFour;

	private LogicEventHandler inputOne;

	private LogicEventHandler inputTwo;

	private LogicEventHandler inputThree;

	private LogicEventHandler inputFour;

	private LogicPortVisualizer outputOne;

	private LogicPortVisualizer outputTwo;

	private LogicPortVisualizer outputThree;

	private LogicPortVisualizer outputFour;

	private LogicEventSender outputTwoSender;

	private LogicEventSender outputThreeSender;

	private LogicEventSender outputFourSender;

	private LogicEventHandler controlOne;

	private LogicEventHandler controlTwo;

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private static KAnimHashedString INPUT1_SYMBOL = "input1";

	private static KAnimHashedString INPUT2_SYMBOL = "input2";

	private static KAnimHashedString INPUT3_SYMBOL = "input3";

	private static KAnimHashedString INPUT4_SYMBOL = "input4";

	private static KAnimHashedString OUTPUT1_SYMBOL = "output1";

	private static KAnimHashedString OUTPUT2_SYMBOL = "output2";

	private static KAnimHashedString OUTPUT3_SYMBOL = "output3";

	private static KAnimHashedString OUTPUT4_SYMBOL = "output4";

	private static KAnimHashedString INPUT1_SYMBOL_BLM_RED = "input1_red_bloom";

	private static KAnimHashedString INPUT1_SYMBOL_BLM_GRN = "input1_green_bloom";

	private static KAnimHashedString INPUT2_SYMBOL_BLM_RED = "input2_red_bloom";

	private static KAnimHashedString INPUT2_SYMBOL_BLM_GRN = "input2_green_bloom";

	private static KAnimHashedString INPUT3_SYMBOL_BLM_RED = "input3_red_bloom";

	private static KAnimHashedString INPUT3_SYMBOL_BLM_GRN = "input3_green_bloom";

	private static KAnimHashedString INPUT4_SYMBOL_BLM_RED = "input4_red_bloom";

	private static KAnimHashedString INPUT4_SYMBOL_BLM_GRN = "input4_green_bloom";

	private static KAnimHashedString OUTPUT1_SYMBOL_BLM_RED = "output1_red_bloom";

	private static KAnimHashedString OUTPUT1_SYMBOL_BLM_GRN = "output1_green_bloom";

	private static KAnimHashedString OUTPUT2_SYMBOL_BLM_RED = "output2_red_bloom";

	private static KAnimHashedString OUTPUT2_SYMBOL_BLM_GRN = "output2_green_bloom";

	private static KAnimHashedString OUTPUT3_SYMBOL_BLM_RED = "output3_red_bloom";

	private static KAnimHashedString OUTPUT3_SYMBOL_BLM_GRN = "output3_green_bloom";

	private static KAnimHashedString OUTPUT4_SYMBOL_BLM_RED = "output4_red_bloom";

	private static KAnimHashedString OUTPUT4_SYMBOL_BLM_GRN = "output4_green_bloom";

	private static KAnimHashedString LINE_LEFT_1_SYMBOL = "line_left_1";

	private static KAnimHashedString LINE_LEFT_2_SYMBOL = "line_left_2";

	private static KAnimHashedString LINE_LEFT_3_SYMBOL = "line_left_3";

	private static KAnimHashedString LINE_LEFT_4_SYMBOL = "line_left_4";

	private static KAnimHashedString LINE_RIGHT_1_SYMBOL = "line_right_1";

	private static KAnimHashedString LINE_RIGHT_2_SYMBOL = "line_right_2";

	private static KAnimHashedString LINE_RIGHT_3_SYMBOL = "line_right_3";

	private static KAnimHashedString LINE_RIGHT_4_SYMBOL = "line_right_4";

	private static KAnimHashedString FLIPPER_1_SYMBOL = "flipper1";

	private static KAnimHashedString FLIPPER_2_SYMBOL = "flipper2";

	private static KAnimHashedString FLIPPER_3_SYMBOL = "flipper3";

	private static KAnimHashedString INPUT_SYMBOL = "input";

	private static KAnimHashedString OUTPUT_SYMBOL = "output";

	private static KAnimHashedString INPUT1_SYMBOL_BLOOM = "input1_bloom";

	private static KAnimHashedString INPUT2_SYMBOL_BLOOM = "input2_bloom";

	private static KAnimHashedString INPUT3_SYMBOL_BLOOM = "input3_bloom";

	private static KAnimHashedString INPUT4_SYMBOL_BLOOM = "input4_bloom";

	private static KAnimHashedString OUTPUT1_SYMBOL_BLOOM = "output1_bloom";

	private static KAnimHashedString OUTPUT2_SYMBOL_BLOOM = "output2_bloom";

	private static KAnimHashedString OUTPUT3_SYMBOL_BLOOM = "output3_bloom";

	private static KAnimHashedString OUTPUT4_SYMBOL_BLOOM = "output4_bloom";

	private static KAnimHashedString LINE_LEFT_1_SYMBOL_BLOOM = "line_left_1_bloom";

	private static KAnimHashedString LINE_LEFT_2_SYMBOL_BLOOM = "line_left_2_bloom";

	private static KAnimHashedString LINE_LEFT_3_SYMBOL_BLOOM = "line_left_3_bloom";

	private static KAnimHashedString LINE_LEFT_4_SYMBOL_BLOOM = "line_left_4_bloom";

	private static KAnimHashedString LINE_RIGHT_1_SYMBOL_BLOOM = "line_right_1_bloom";

	private static KAnimHashedString LINE_RIGHT_2_SYMBOL_BLOOM = "line_right_2_bloom";

	private static KAnimHashedString LINE_RIGHT_3_SYMBOL_BLOOM = "line_right_3_bloom";

	private static KAnimHashedString LINE_RIGHT_4_SYMBOL_BLOOM = "line_right_4_bloom";

	private static KAnimHashedString FLIPPER_1_SYMBOL_BLOOM = "flipper1_bloom";

	private static KAnimHashedString FLIPPER_2_SYMBOL_BLOOM = "flipper2_bloom";

	private static KAnimHashedString FLIPPER_3_SYMBOL_BLOOM = "flipper3_bloom";

	private static KAnimHashedString INPUT_SYMBOL_BLOOM = "input_bloom";

	private static KAnimHashedString OUTPUT_SYMBOL_BLOOM = "output_bloom";

	private static KAnimHashedString[][] multiplexerSymbolPaths = new KAnimHashedString[4][]
	{
		new KAnimHashedString[5] { LINE_LEFT_1_SYMBOL_BLOOM, FLIPPER_1_SYMBOL_BLOOM, LINE_RIGHT_1_SYMBOL_BLOOM, FLIPPER_3_SYMBOL_BLOOM, OUTPUT_SYMBOL_BLOOM },
		new KAnimHashedString[5] { LINE_LEFT_2_SYMBOL_BLOOM, FLIPPER_1_SYMBOL_BLOOM, LINE_RIGHT_1_SYMBOL_BLOOM, FLIPPER_3_SYMBOL_BLOOM, OUTPUT_SYMBOL_BLOOM },
		new KAnimHashedString[5] { LINE_LEFT_3_SYMBOL_BLOOM, FLIPPER_2_SYMBOL_BLOOM, LINE_RIGHT_2_SYMBOL_BLOOM, FLIPPER_3_SYMBOL_BLOOM, OUTPUT_SYMBOL_BLOOM },
		new KAnimHashedString[5] { LINE_LEFT_4_SYMBOL_BLOOM, FLIPPER_2_SYMBOL_BLOOM, LINE_RIGHT_2_SYMBOL_BLOOM, FLIPPER_3_SYMBOL_BLOOM, OUTPUT_SYMBOL_BLOOM }
	};

	private static KAnimHashedString[] multiplexerSymbols = new KAnimHashedString[10] { LINE_LEFT_1_SYMBOL, LINE_LEFT_2_SYMBOL, LINE_LEFT_3_SYMBOL, LINE_LEFT_4_SYMBOL, LINE_RIGHT_1_SYMBOL, LINE_RIGHT_2_SYMBOL, FLIPPER_1_SYMBOL, FLIPPER_2_SYMBOL, FLIPPER_3_SYMBOL, OUTPUT_SYMBOL };

	private static KAnimHashedString[] multiplexerBloomSymbols = new KAnimHashedString[10] { LINE_LEFT_1_SYMBOL_BLOOM, LINE_LEFT_2_SYMBOL_BLOOM, LINE_LEFT_3_SYMBOL_BLOOM, LINE_LEFT_4_SYMBOL_BLOOM, LINE_RIGHT_1_SYMBOL_BLOOM, LINE_RIGHT_2_SYMBOL_BLOOM, FLIPPER_1_SYMBOL_BLOOM, FLIPPER_2_SYMBOL_BLOOM, FLIPPER_3_SYMBOL_BLOOM, OUTPUT_SYMBOL_BLOOM };

	private static KAnimHashedString[][] demultiplexerSymbolPaths = new KAnimHashedString[4][]
	{
		new KAnimHashedString[4] { INPUT_SYMBOL_BLOOM, LINE_LEFT_1_SYMBOL_BLOOM, LINE_RIGHT_1_SYMBOL_BLOOM, OUTPUT1_SYMBOL },
		new KAnimHashedString[4] { INPUT_SYMBOL_BLOOM, LINE_LEFT_1_SYMBOL_BLOOM, LINE_RIGHT_2_SYMBOL_BLOOM, OUTPUT2_SYMBOL },
		new KAnimHashedString[4] { INPUT_SYMBOL_BLOOM, LINE_LEFT_2_SYMBOL_BLOOM, LINE_RIGHT_3_SYMBOL_BLOOM, OUTPUT3_SYMBOL },
		new KAnimHashedString[4] { INPUT_SYMBOL_BLOOM, LINE_LEFT_2_SYMBOL_BLOOM, LINE_RIGHT_4_SYMBOL_BLOOM, OUTPUT4_SYMBOL }
	};

	private static KAnimHashedString[] demultiplexerSymbols = new KAnimHashedString[7] { INPUT_SYMBOL, LINE_LEFT_1_SYMBOL, LINE_LEFT_2_SYMBOL, LINE_RIGHT_1_SYMBOL, LINE_RIGHT_2_SYMBOL, LINE_RIGHT_3_SYMBOL, LINE_RIGHT_4_SYMBOL };

	private static KAnimHashedString[] demultiplexerBloomSymbols = new KAnimHashedString[7] { INPUT_SYMBOL_BLOOM, LINE_LEFT_1_SYMBOL_BLOOM, LINE_LEFT_2_SYMBOL_BLOOM, LINE_RIGHT_1_SYMBOL_BLOOM, LINE_RIGHT_2_SYMBOL_BLOOM, LINE_RIGHT_3_SYMBOL_BLOOM, LINE_RIGHT_4_SYMBOL_BLOOM };

	private static KAnimHashedString[] demultiplexerOutputSymbols = new KAnimHashedString[4] { OUTPUT1_SYMBOL, OUTPUT2_SYMBOL, OUTPUT3_SYMBOL, OUTPUT4_SYMBOL };

	private static KAnimHashedString[] demultiplexerOutputRedSymbols = new KAnimHashedString[4] { OUTPUT1_SYMBOL_BLM_RED, OUTPUT2_SYMBOL_BLM_RED, OUTPUT3_SYMBOL_BLM_RED, OUTPUT4_SYMBOL_BLM_RED };

	private static KAnimHashedString[] demultiplexerOutputGreenSymbols = new KAnimHashedString[4] { OUTPUT1_SYMBOL_BLM_GRN, OUTPUT2_SYMBOL_BLM_GRN, OUTPUT3_SYMBOL_BLM_GRN, OUTPUT4_SYMBOL_BLM_GRN };

	private Color activeTintColor = new Color(46f / 85f, 84f / 85f, 0.29803923f);

	private Color inactiveTintColor = Color.red;

	protected override void OnSpawn()
	{
		inputOne = new LogicEventHandler(base.InputCellOne, UpdateState, null, LogicPortSpriteType.Input);
		if (base.RequiresTwoInputs)
		{
			inputTwo = new LogicEventHandler(base.InputCellTwo, UpdateState, null, LogicPortSpriteType.Input);
		}
		else if (base.RequiresFourInputs)
		{
			inputTwo = new LogicEventHandler(base.InputCellTwo, UpdateState, null, LogicPortSpriteType.Input);
			inputThree = new LogicEventHandler(base.InputCellThree, UpdateState, null, LogicPortSpriteType.Input);
			inputFour = new LogicEventHandler(base.InputCellFour, UpdateState, null, LogicPortSpriteType.Input);
		}
		if (base.RequiresControlInputs)
		{
			controlOne = new LogicEventHandler(base.ControlCellOne, UpdateState, null, LogicPortSpriteType.ControlInput);
			controlTwo = new LogicEventHandler(base.ControlCellTwo, UpdateState, null, LogicPortSpriteType.ControlInput);
		}
		if (base.RequiresFourOutputs)
		{
			outputTwo = new LogicPortVisualizer(base.OutputCellTwo, LogicPortSpriteType.Output);
			outputThree = new LogicPortVisualizer(base.OutputCellThree, LogicPortSpriteType.Output);
			outputFour = new LogicPortVisualizer(base.OutputCellFour, LogicPortSpriteType.Output);
			outputTwoSender = new LogicEventSender(LogicGateBase.OUTPUT_TWO_PORT_ID, base.OutputCellTwo, delegate(int new_value)
			{
				if (this != null)
				{
					OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_TWO_PORT_ID, new_value);
				}
			}, null, LogicPortSpriteType.Output);
			outputThreeSender = new LogicEventSender(LogicGateBase.OUTPUT_THREE_PORT_ID, base.OutputCellThree, delegate(int new_value)
			{
				if (this != null)
				{
					OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_THREE_PORT_ID, new_value);
				}
			}, null, LogicPortSpriteType.Output);
			outputFourSender = new LogicEventSender(LogicGateBase.OUTPUT_FOUR_PORT_ID, base.OutputCellFour, delegate(int new_value)
			{
				if (this != null)
				{
					OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_FOUR_PORT_ID, new_value);
				}
			}, null, LogicPortSpriteType.Output);
		}
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		BuildingHP component = GetComponent<BuildingHP>();
		if (component == null || !component.IsBroken)
		{
			Connect();
		}
	}

	protected override void OnCleanUp()
	{
		cleaningUp = true;
		Disconnect();
		Unsubscribe(774203113, OnBuildingBrokenDelegate);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		base.OnCleanUp();
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
	}

	private void Connect()
	{
		if (!connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			connected = true;
			int outputCellOne = base.OutputCellOne;
			logicCircuitSystem.AddToNetworks(outputCellOne, this, is_endpoint: true);
			outputOne = new LogicPortVisualizer(outputCellOne, LogicPortSpriteType.Output);
			logicCircuitManager.AddVisElem(outputOne);
			if (base.RequiresFourOutputs)
			{
				outputTwo = new LogicPortVisualizer(base.OutputCellTwo, LogicPortSpriteType.Output);
				logicCircuitSystem.AddToNetworks(base.OutputCellTwo, outputTwoSender, is_endpoint: true);
				logicCircuitManager.AddVisElem(outputTwo);
				outputThree = new LogicPortVisualizer(base.OutputCellThree, LogicPortSpriteType.Output);
				logicCircuitSystem.AddToNetworks(base.OutputCellThree, outputThreeSender, is_endpoint: true);
				logicCircuitManager.AddVisElem(outputThree);
				outputFour = new LogicPortVisualizer(base.OutputCellFour, LogicPortSpriteType.Output);
				logicCircuitSystem.AddToNetworks(base.OutputCellFour, outputFourSender, is_endpoint: true);
				logicCircuitManager.AddVisElem(outputFour);
			}
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.AddToNetworks(inputCellOne, inputOne, is_endpoint: true);
			logicCircuitManager.AddVisElem(inputOne);
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.AddToNetworks(inputCellTwo, inputTwo, is_endpoint: true);
				logicCircuitManager.AddVisElem(inputTwo);
			}
			else if (base.RequiresFourInputs)
			{
				logicCircuitSystem.AddToNetworks(base.InputCellTwo, inputTwo, is_endpoint: true);
				logicCircuitManager.AddVisElem(inputTwo);
				logicCircuitSystem.AddToNetworks(base.InputCellThree, inputThree, is_endpoint: true);
				logicCircuitManager.AddVisElem(inputThree);
				logicCircuitSystem.AddToNetworks(base.InputCellFour, inputFour, is_endpoint: true);
				logicCircuitManager.AddVisElem(inputFour);
			}
			if (base.RequiresControlInputs)
			{
				logicCircuitSystem.AddToNetworks(base.ControlCellOne, controlOne, is_endpoint: true);
				logicCircuitManager.AddVisElem(controlOne);
				logicCircuitSystem.AddToNetworks(base.ControlCellTwo, controlTwo, is_endpoint: true);
				logicCircuitManager.AddVisElem(controlTwo);
			}
			RefreshAnimation();
		}
	}

	private void Disconnect()
	{
		if (connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			connected = false;
			int outputCellOne = base.OutputCellOne;
			logicCircuitSystem.RemoveFromNetworks(outputCellOne, this, is_endpoint: true);
			logicCircuitManager.RemoveVisElem(outputOne);
			outputOne = null;
			if (base.RequiresFourOutputs)
			{
				logicCircuitSystem.RemoveFromNetworks(base.OutputCellTwo, outputTwoSender, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(outputTwo);
				outputTwo = null;
				logicCircuitSystem.RemoveFromNetworks(base.OutputCellThree, outputThreeSender, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(outputThree);
				outputThree = null;
				logicCircuitSystem.RemoveFromNetworks(base.OutputCellFour, outputFourSender, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(outputFour);
				outputFour = null;
			}
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.RemoveFromNetworks(inputCellOne, inputOne, is_endpoint: true);
			logicCircuitManager.RemoveVisElem(inputOne);
			inputOne = null;
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.RemoveFromNetworks(inputCellTwo, inputTwo, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(inputTwo);
				inputTwo = null;
			}
			else if (base.RequiresFourInputs)
			{
				logicCircuitSystem.RemoveFromNetworks(base.InputCellTwo, inputTwo, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(inputTwo);
				inputTwo = null;
				logicCircuitSystem.RemoveFromNetworks(base.InputCellThree, inputThree, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(inputThree);
				inputThree = null;
				logicCircuitSystem.RemoveFromNetworks(base.InputCellFour, inputFour, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(inputFour);
				inputFour = null;
			}
			if (base.RequiresControlInputs)
			{
				logicCircuitSystem.RemoveFromNetworks(base.ControlCellOne, controlOne, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(controlOne);
				controlOne = null;
				logicCircuitSystem.RemoveFromNetworks(base.ControlCellTwo, controlTwo, is_endpoint: true);
				logicCircuitManager.RemoveVisElem(controlTwo);
				controlTwo = null;
			}
			RefreshAnimation();
		}
	}

	private void UpdateState(int new_value)
	{
		if (cleaningUp)
		{
			return;
		}
		int value = inputOne.Value;
		int num = ((inputTwo != null) ? inputTwo.Value : 0);
		int num2 = ((inputThree != null) ? inputThree.Value : 0);
		int num3 = ((inputFour != null) ? inputFour.Value : 0);
		int value2 = ((controlOne != null) ? controlOne.Value : 0);
		int value3 = ((controlTwo != null) ? controlTwo.Value : 0);
		if (base.RequiresFourInputs && base.RequiresControlInputs)
		{
			outputValueOne = 0;
			if (op == Op.Multiplexer)
			{
				if (!LogicCircuitNetwork.IsBitActive(0, value3))
				{
					if (!LogicCircuitNetwork.IsBitActive(0, value2))
					{
						outputValueOne = value;
					}
					else
					{
						outputValueOne = num;
					}
				}
				else if (!LogicCircuitNetwork.IsBitActive(0, value2))
				{
					outputValueOne = num2;
				}
				else
				{
					outputValueOne = num3;
				}
			}
		}
		if (base.RequiresFourOutputs && base.RequiresControlInputs)
		{
			outputValueOne = 0;
			outputValueTwo = 0;
			outputTwoSender.SetValue(0);
			outputValueThree = 0;
			outputThreeSender.SetValue(0);
			outputValueFour = 0;
			outputFourSender.SetValue(0);
			if (op == Op.Demultiplexer)
			{
				if (!LogicCircuitNetwork.IsBitActive(0, value2))
				{
					if (!LogicCircuitNetwork.IsBitActive(0, value3))
					{
						outputValueOne = value;
					}
					else
					{
						outputValueTwo = value;
						outputTwoSender.SetValue(value);
					}
				}
				else if (!LogicCircuitNetwork.IsBitActive(0, value3))
				{
					outputValueThree = value;
					outputThreeSender.SetValue(value);
				}
				else
				{
					outputValueFour = value;
					outputFourSender.SetValue(value);
				}
			}
		}
		switch (op)
		{
		case Op.And:
			outputValueOne = value & num;
			break;
		case Op.Or:
			outputValueOne = value | num;
			break;
		case Op.Xor:
			outputValueOne = value ^ num;
			break;
		case Op.Not:
		{
			LogicWire.BitDepth bitDepth = LogicWire.BitDepth.NumRatings;
			int inputCellOne = base.InputCellOne;
			GameObject gameObject = Grid.Objects[inputCellOne, 31];
			if (gameObject != null)
			{
				LogicWire component = gameObject.GetComponent<LogicWire>();
				if (component != null)
				{
					bitDepth = component.MaxBitDepth;
				}
			}
			if (bitDepth != 0 && bitDepth == LogicWire.BitDepth.FourBit)
			{
				uint num4 = (uint)value;
				num4 = ~num4;
				num4 = (uint)(outputValueOne = (int)(num4 & 0xF));
			}
			else
			{
				outputValueOne = ((value == 0) ? 1 : 0);
			}
			break;
		}
		case Op.CustomSingle:
			outputValueOne = GetCustomValue(value, num);
			break;
		}
		RefreshAnimation();
	}

	private void OnAdditionalOutputsLogicValueChanged(HashedString port_id, int new_value)
	{
		if (base.gameObject != null)
		{
			base.gameObject.Trigger(-801688580, new LogicValueChanged
			{
				portID = port_id,
				newValue = new_value
			});
		}
	}

	public virtual void LogicTick()
	{
	}

	protected virtual int GetCustomValue(int val1, int val2)
	{
		return val1;
	}

	public int GetPortValue(PortId port)
	{
		switch (port)
		{
		case PortId.InputOne:
			return inputOne.Value;
		case PortId.InputTwo:
			if (base.RequiresTwoInputs || base.RequiresFourInputs)
			{
				return inputTwo.Value;
			}
			return 0;
		case PortId.InputThree:
			if (!base.RequiresFourInputs)
			{
				return 0;
			}
			return inputThree.Value;
		case PortId.InputFour:
			if (!base.RequiresFourInputs)
			{
				return 0;
			}
			return inputFour.Value;
		case PortId.OutputOne:
			return outputValueOne;
		case PortId.OutputTwo:
			return outputValueTwo;
		case PortId.OutputThree:
			return outputValueThree;
		case PortId.OutputFour:
			return outputValueFour;
		case PortId.ControlOne:
			return controlOne.Value;
		case PortId.ControlTwo:
			return controlTwo.Value;
		default:
			return outputValueOne;
		}
	}

	public bool GetPortConnected(PortId port)
	{
		if ((port == PortId.InputTwo && !base.RequiresTwoInputs && !base.RequiresFourInputs) || (port == PortId.InputThree && !base.RequiresFourInputs) || (port == PortId.InputFour && !base.RequiresFourInputs))
		{
			return false;
		}
		int cell = PortCell(port);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(cell) != null;
	}

	public void SetPortDescriptions(LogicGateDescriptions descriptions)
	{
		this.descriptions = descriptions;
	}

	public LogicGateDescriptions.Description GetPortDescription(PortId port)
	{
		switch (port)
		{
		case PortId.InputOne:
			if (descriptions.inputOne == null)
			{
				if (!base.RequiresTwoInputs && !base.RequiresFourInputs)
				{
					return INPUT_ONE_SINGLE_DESCRIPTION;
				}
				return INPUT_ONE_MULTI_DESCRIPTION;
			}
			return descriptions.inputOne;
		case PortId.InputTwo:
			if (descriptions.inputTwo == null)
			{
				return INPUT_TWO_DESCRIPTION;
			}
			return descriptions.inputTwo;
		case PortId.InputThree:
			if (descriptions.inputThree == null)
			{
				return INPUT_THREE_DESCRIPTION;
			}
			return descriptions.inputThree;
		case PortId.InputFour:
			if (descriptions.inputFour == null)
			{
				return INPUT_FOUR_DESCRIPTION;
			}
			return descriptions.inputFour;
		case PortId.OutputOne:
			if (descriptions.inputOne == null)
			{
				if (!base.RequiresFourOutputs)
				{
					return OUTPUT_ONE_SINGLE_DESCRIPTION;
				}
				return OUTPUT_ONE_MULTI_DESCRIPTION;
			}
			return descriptions.inputOne;
		case PortId.OutputTwo:
			if (descriptions.outputTwo == null)
			{
				return OUTPUT_TWO_DESCRIPTION;
			}
			return descriptions.outputTwo;
		case PortId.OutputThree:
			if (descriptions.outputThree == null)
			{
				return OUTPUT_THREE_DESCRIPTION;
			}
			return descriptions.outputThree;
		case PortId.OutputFour:
			if (descriptions.outputFour == null)
			{
				return OUTPUT_FOUR_DESCRIPTION;
			}
			return descriptions.outputFour;
		case PortId.ControlOne:
			if (descriptions.controlOne == null)
			{
				return CONTROL_ONE_DESCRIPTION;
			}
			return descriptions.controlOne;
		case PortId.ControlTwo:
			if (descriptions.controlTwo == null)
			{
				return CONTROL_TWO_DESCRIPTION;
			}
			return descriptions.controlTwo;
		default:
			return descriptions.outputOne;
		}
	}

	public int GetLogicValue()
	{
		return outputValueOne;
	}

	public int GetLogicCell()
	{
		return GetLogicUICell();
	}

	public int GetLogicUICell()
	{
		return base.OutputCellOne;
	}

	public bool IsLogicInput()
	{
		return false;
	}

	private LogicEventHandler GetInputFromControlValue(int val)
	{
		return val switch
		{
			3 => inputFour, 
			2 => inputThree, 
			1 => inputTwo, 
			_ => inputOne, 
		};
	}

	private void ShowSymbolConditionally(bool showAnything, bool active, KBatchedAnimController kbac, KAnimHashedString ifTrue, KAnimHashedString ifFalse)
	{
		if (!showAnything)
		{
			kbac.SetSymbolVisiblity(ifTrue, is_visible: false);
			kbac.SetSymbolVisiblity(ifFalse, is_visible: false);
		}
		else
		{
			kbac.SetSymbolVisiblity(ifTrue, active);
			kbac.SetSymbolVisiblity(ifFalse, !active);
		}
	}

	private void TintSymbolConditionally(bool tintAnything, bool condition, KBatchedAnimController kbac, KAnimHashedString symbol, Color ifTrue, Color ifFalse)
	{
		if (tintAnything)
		{
			kbac.SetSymbolTint(symbol, condition ? ifTrue : ifFalse);
		}
		else
		{
			kbac.SetSymbolTint(symbol, Color.white);
		}
	}

	private void SetBloomSymbolShowing(bool showing, KBatchedAnimController kbac, KAnimHashedString symbol, KAnimHashedString bloomSymbol)
	{
		kbac.SetSymbolVisiblity(bloomSymbol, showing);
		kbac.SetSymbolVisiblity(symbol, !showing);
	}

	protected void RefreshAnimation()
	{
		if (cleaningUp)
		{
			return;
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (op == Op.Multiplexer)
		{
			int num = LogicCircuitNetwork.GetBitValue(0, controlOne.Value) + LogicCircuitNetwork.GetBitValue(0, controlTwo.Value) * 2;
			if (lastAnimState != num)
			{
				if (lastAnimState == -1)
				{
					component.Play(num.ToString());
				}
				else
				{
					component.Play(lastAnimState + "_" + num);
				}
			}
			lastAnimState = num;
			LogicEventHandler inputFromControlValue = GetInputFromControlValue(num);
			KAnimHashedString[] array = multiplexerSymbolPaths[num];
			LogicCircuitNetwork logicCircuitNetwork = Game.Instance.logicCircuitSystem.GetNetworkForCell(inputFromControlValue.GetLogicCell()) as LogicCircuitNetwork;
			UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(base.InputCellOne);
			UtilityNetwork networkForCell2 = Game.Instance.logicCircuitSystem.GetNetworkForCell(base.InputCellTwo);
			UtilityNetwork networkForCell3 = Game.Instance.logicCircuitSystem.GetNetworkForCell(base.InputCellThree);
			UtilityNetwork networkForCell4 = Game.Instance.logicCircuitSystem.GetNetworkForCell(base.InputCellFour);
			ShowSymbolConditionally(networkForCell != null, inputOne.Value == 0, component, INPUT1_SYMBOL_BLM_RED, INPUT1_SYMBOL_BLM_GRN);
			ShowSymbolConditionally(networkForCell2 != null, inputTwo.Value == 0, component, INPUT2_SYMBOL_BLM_RED, INPUT2_SYMBOL_BLM_GRN);
			ShowSymbolConditionally(networkForCell3 != null, inputThree.Value == 0, component, INPUT3_SYMBOL_BLM_RED, INPUT3_SYMBOL_BLM_GRN);
			ShowSymbolConditionally(networkForCell4 != null, inputFour.Value == 0, component, INPUT4_SYMBOL_BLM_RED, INPUT4_SYMBOL_BLM_GRN);
			ShowSymbolConditionally(logicCircuitNetwork != null, inputFromControlValue.Value == 0, component, OUTPUT1_SYMBOL_BLM_RED, OUTPUT1_SYMBOL_BLM_GRN);
			TintSymbolConditionally(networkForCell != null, inputOne.Value == 0, component, INPUT1_SYMBOL, inactiveTintColor, activeTintColor);
			TintSymbolConditionally(networkForCell2 != null, inputTwo.Value == 0, component, INPUT2_SYMBOL, inactiveTintColor, activeTintColor);
			TintSymbolConditionally(networkForCell3 != null, inputThree.Value == 0, component, INPUT3_SYMBOL, inactiveTintColor, activeTintColor);
			TintSymbolConditionally(networkForCell4 != null, inputFour.Value == 0, component, INPUT4_SYMBOL, inactiveTintColor, activeTintColor);
			TintSymbolConditionally(Game.Instance.logicCircuitSystem.GetNetworkForCell(base.OutputCellOne) != null && logicCircuitNetwork != null, inputFromControlValue.Value == 0, component, OUTPUT1_SYMBOL, inactiveTintColor, activeTintColor);
			for (int i = 0; i < multiplexerSymbols.Length; i++)
			{
				KAnimHashedString symbol = multiplexerSymbols[i];
				KAnimHashedString kAnimHashedString = multiplexerBloomSymbols[i];
				bool flag = Array.IndexOf(array, kAnimHashedString) != -1 && logicCircuitNetwork != null;
				SetBloomSymbolShowing(flag, component, symbol, kAnimHashedString);
				if (flag)
				{
					component.SetSymbolTint(kAnimHashedString, (inputFromControlValue.Value == 0) ? inactiveTintColor : activeTintColor);
				}
			}
		}
		else if (op == Op.Demultiplexer)
		{
			int num2 = LogicCircuitNetwork.GetBitValue(0, controlOne.Value) * 2 + LogicCircuitNetwork.GetBitValue(0, controlTwo.Value);
			if (lastAnimState != num2)
			{
				if (lastAnimState == -1)
				{
					component.Play(num2.ToString());
				}
				else
				{
					component.Play(lastAnimState + "_" + num2);
				}
			}
			lastAnimState = num2;
			KAnimHashedString[] array2 = demultiplexerSymbolPaths[num2];
			LogicCircuitNetwork logicCircuitNetwork2 = Game.Instance.logicCircuitSystem.GetNetworkForCell(inputOne.GetLogicCell()) as LogicCircuitNetwork;
			for (int j = 0; j < demultiplexerSymbols.Length; j++)
			{
				KAnimHashedString symbol2 = demultiplexerSymbols[j];
				KAnimHashedString kAnimHashedString2 = demultiplexerBloomSymbols[j];
				bool flag2 = Array.IndexOf(array2, kAnimHashedString2) != -1 && logicCircuitNetwork2 != null;
				SetBloomSymbolShowing(flag2, component, symbol2, kAnimHashedString2);
				if (flag2)
				{
					component.SetSymbolTint(kAnimHashedString2, (inputOne.Value == 0) ? inactiveTintColor : activeTintColor);
				}
			}
			ShowSymbolConditionally(logicCircuitNetwork2 != null, inputOne.Value == 0, component, INPUT1_SYMBOL_BLM_RED, INPUT1_SYMBOL_BLM_GRN);
			if (logicCircuitNetwork2 != null)
			{
				component.SetSymbolTint(INPUT1_SYMBOL_BLOOM, (inputOne.Value == 0) ? inactiveTintColor : activeTintColor);
			}
			int[] array3 = new int[4] { base.OutputCellOne, base.OutputCellTwo, base.OutputCellThree, base.OutputCellFour };
			for (int k = 0; k < demultiplexerOutputSymbols.Length; k++)
			{
				KAnimHashedString kAnimHashedString3 = demultiplexerOutputSymbols[k];
				bool flag3 = Array.IndexOf(array2, kAnimHashedString3) == -1 || inputOne.Value == 0;
				UtilityNetwork networkForCell5 = Game.Instance.logicCircuitSystem.GetNetworkForCell(array3[k]);
				TintSymbolConditionally(logicCircuitNetwork2 != null && networkForCell5 != null, flag3, component, kAnimHashedString3, inactiveTintColor, activeTintColor);
				ShowSymbolConditionally(logicCircuitNetwork2 != null && networkForCell5 != null, flag3, component, demultiplexerOutputRedSymbols[k], demultiplexerOutputGreenSymbols[k]);
			}
		}
		else if (op == Op.And || op == Op.Xor || op == Op.Not || op == Op.Or)
		{
			int outputCellOne = base.OutputCellOne;
			if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCellOne) is LogicCircuitNetwork))
			{
				component.Play("off");
				return;
			}
			if (base.RequiresTwoInputs)
			{
				int num3 = inputOne.Value * 2 + inputTwo.Value;
				if (lastAnimState != num3)
				{
					if (lastAnimState == -1)
					{
						component.Play(num3.ToString());
					}
					else
					{
						component.Play(lastAnimState + "_" + num3);
					}
					lastAnimState = num3;
				}
				return;
			}
			int value = inputOne.Value;
			if (lastAnimState != value)
			{
				if (lastAnimState == -1)
				{
					component.Play(value.ToString());
				}
				else
				{
					component.Play(lastAnimState + "_" + value);
				}
				lastAnimState = value;
			}
		}
		else
		{
			int outputCellOne2 = base.OutputCellOne;
			if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCellOne2) is LogicCircuitNetwork))
			{
				component.Play("off");
			}
			else if (base.RequiresTwoInputs)
			{
				component.Play("on_" + (inputOne.Value + inputTwo.Value * 2 + outputValueOne * 4));
			}
			else
			{
				component.Play("on_" + (inputOne.Value + outputValueOne * 4));
			}
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
	}
}
