using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonWriter")]
public class LogicRibbonWriter : KMonoBehaviour, ILogicRibbonBitSelector, IRender200ms
{
	public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicRibbonWriterInput");

	public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicRibbonWriterOutput");

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicRibbonWriter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicRibbonWriter>(delegate(LogicRibbonWriter component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicRibbonWriter> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicRibbonWriter>(delegate(LogicRibbonWriter component, object data)
	{
		component.OnCopySettings(data);
	});

	private LogicPorts ports;

	public int bitDepth = 4;

	[Serialize]
	public int selectedBit;

	[Serialize]
	private int currentValue;

	private KBatchedAnimController kbac;

	private Color colorOn = new Color(29f / 85f, 37f / 51f, 0.36862746f);

	private Color colorOff = new Color(81f / 85f, 0.2901961f, 0.2784314f);

	private static KAnimHashedString BIT_ONE_SYMBOL = "bit1_bloom";

	private static KAnimHashedString BIT_TWO_SYMBOL = "bit2_bloom";

	private static KAnimHashedString BIT_THREE_SYMBOL = "bit3_bloom";

	private static KAnimHashedString BIT_FOUR_SYMBOL = "bit4_bloom";

	private static KAnimHashedString INPUT_SYMBOL = "input_light_bloom";

	public string SideScreenTitle => "STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_WRITER_TITLE";

	public string SideScreenDescription => UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_WRITER_DESCRIPTION;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		ports = GetComponent<LogicPorts>();
		kbac = GetComponent<KBatchedAnimController>();
		kbac.Play("idle");
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (!(logicValueChanged.portID != INPUT_PORT_ID))
		{
			currentValue = logicValueChanged.newValue;
			UpdateLogicCircuit();
			UpdateVisuals();
		}
	}

	private void OnCopySettings(object data)
	{
		LogicRibbonWriter component = ((GameObject)data).GetComponent<LogicRibbonWriter>();
		if (component != null)
		{
			SetBitSelection(component.selectedBit);
		}
	}

	private void UpdateLogicCircuit()
	{
		int new_value = currentValue << selectedBit;
		GetComponent<LogicPorts>().SendSignal(OUTPUT_PORT_ID, new_value);
	}

	public void Render200ms(float dt)
	{
		UpdateVisuals();
	}

	private LogicCircuitNetwork GetInputNetwork()
	{
		LogicCircuitNetwork result = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(INPUT_PORT_ID);
			result = Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
		}
		return result;
	}

	private LogicCircuitNetwork GetOutputNetwork()
	{
		LogicCircuitNetwork result = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(OUTPUT_PORT_ID);
			result = Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
		}
		return result;
	}

	public void SetBitSelection(int bit)
	{
		selectedBit = bit;
		UpdateLogicCircuit();
	}

	public int GetBitSelection()
	{
		return selectedBit;
	}

	public int GetBitDepth()
	{
		return bitDepth;
	}

	public bool SideScreenDisplayWriterDescription()
	{
		return true;
	}

	public bool SideScreenDisplayReaderDescription()
	{
		return false;
	}

	public bool IsBitActive(int bit)
	{
		LogicCircuitNetwork logicCircuitNetwork = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(OUTPUT_PORT_ID);
			logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
		}
		return logicCircuitNetwork?.IsBitActive(bit) ?? false;
	}

	public int GetInputValue()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		if (!(component != null))
		{
			return 0;
		}
		return component.GetInputValue(INPUT_PORT_ID);
	}

	public int GetOutputValue()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		if (!(component != null))
		{
			return 0;
		}
		return component.GetOutputValue(OUTPUT_PORT_ID);
	}

	public void UpdateVisuals()
	{
		LogicCircuitNetwork inputNetwork = GetInputNetwork();
		LogicCircuitNetwork outputNetwork = GetOutputNetwork();
		int num = 0;
		if (inputNetwork != null)
		{
			num++;
			kbac.SetSymbolTint(INPUT_SYMBOL, LogicCircuitNetwork.IsBitActive(0, GetInputValue()) ? colorOn : colorOff);
		}
		if (outputNetwork != null)
		{
			num += 4;
			kbac.SetSymbolTint(BIT_ONE_SYMBOL, IsBitActive(0) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_TWO_SYMBOL, IsBitActive(1) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_THREE_SYMBOL, IsBitActive(2) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_FOUR_SYMBOL, IsBitActive(3) ? colorOn : colorOff);
		}
		kbac.Play(num + "_" + (GetBitSelection() + 1));
	}
}
