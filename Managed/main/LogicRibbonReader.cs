using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonReader")]
public class LogicRibbonReader : KMonoBehaviour, ILogicRibbonBitSelector, IRender200ms
{
	public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicRibbonReaderInput");

	public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicRibbonReaderOutput");

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicRibbonReader> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicRibbonReader>(delegate(LogicRibbonReader component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicRibbonReader> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicRibbonReader>(delegate(LogicRibbonReader component, object data)
	{
		component.OnCopySettings(data);
	});

	private KAnimHashedString BIT_ONE_SYMBOL = "bit1_bloom";

	private KAnimHashedString BIT_TWO_SYMBOL = "bit2_bloom";

	private KAnimHashedString BIT_THREE_SYMBOL = "bit3_bloom";

	private KAnimHashedString BIT_FOUR_SYMBOL = "bit4_bloom";

	private KAnimHashedString OUTPUT_SYMBOL = "output_light_bloom";

	private KBatchedAnimController kbac;

	private Color colorOn = new Color(29f / 85f, 37f / 51f, 94f / 255f);

	private Color colorOff = new Color(81f / 85f, 74f / 255f, 71f / 255f);

	private LogicPorts ports;

	public int bitDepth = 4;

	[Serialize]
	public int selectedBit = 0;

	[Serialize]
	private int currentValue;

	public string SideScreenTitle => "STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_READER_TITLE";

	public string SideScreenDescription => UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_READER_DESCRIPTION;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		ports = GetComponent<LogicPorts>();
		kbac = GetComponent<KBatchedAnimController>();
		kbac.Play("idle");
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
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
		GameObject gameObject = (GameObject)data;
		LogicRibbonReader component = gameObject.GetComponent<LogicRibbonReader>();
		if (component != null)
		{
			SetBitSelection(component.selectedBit);
		}
	}

	private void UpdateLogicCircuit()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		LogicWire.BitDepth bitDepth = LogicWire.BitDepth.NumRatings;
		int portCell = component.GetPortCell(OUTPUT_PORT_ID);
		GameObject gameObject = Grid.Objects[portCell, 31];
		if (gameObject != null)
		{
			LogicWire component2 = gameObject.GetComponent<LogicWire>();
			if (component2 != null)
			{
				bitDepth = component2.MaxBitDepth;
			}
		}
		LogicWire.BitDepth bitDepth2 = bitDepth;
		if (bitDepth2 != 0 && bitDepth2 == LogicWire.BitDepth.FourBit)
		{
			int new_value = currentValue >> selectedBit;
			component.SendSignal(OUTPUT_PORT_ID, new_value);
		}
		else
		{
			int new_value = currentValue & (1 << selectedBit);
			component.SendSignal(OUTPUT_PORT_ID, (new_value > 0) ? 1 : 0);
		}
		UpdateVisuals();
	}

	public void Render200ms(float dt)
	{
		UpdateVisuals();
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
		return false;
	}

	public bool SideScreenDisplayReaderDescription()
	{
		return true;
	}

	public bool IsBitActive(int bit)
	{
		LogicCircuitNetwork logicCircuitNetwork = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(INPUT_PORT_ID);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitNetwork = logicCircuitManager.GetNetworkForCell(portCell);
		}
		return logicCircuitNetwork?.IsBitActive(bit) ?? false;
	}

	public int GetInputValue()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		return (component != null) ? component.GetInputValue(INPUT_PORT_ID) : 0;
	}

	public int GetOutputValue()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		return (component != null) ? component.GetOutputValue(OUTPUT_PORT_ID) : 0;
	}

	private LogicCircuitNetwork GetInputNetwork()
	{
		LogicCircuitNetwork result = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(INPUT_PORT_ID);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			result = logicCircuitManager.GetNetworkForCell(portCell);
		}
		return result;
	}

	private LogicCircuitNetwork GetOutputNetwork()
	{
		LogicCircuitNetwork result = null;
		if (ports != null)
		{
			int portCell = ports.GetPortCell(OUTPUT_PORT_ID);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			result = logicCircuitManager.GetNetworkForCell(portCell);
		}
		return result;
	}

	public void UpdateVisuals()
	{
		LogicCircuitNetwork inputNetwork = GetInputNetwork();
		LogicCircuitNetwork outputNetwork = GetOutputNetwork();
		int inputValue = GetInputValue();
		int num = 0;
		if (inputNetwork != null)
		{
			num += 4;
			kbac.SetSymbolTint(BIT_ONE_SYMBOL, IsBitActive(0) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_TWO_SYMBOL, IsBitActive(1) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_THREE_SYMBOL, IsBitActive(2) ? colorOn : colorOff);
			kbac.SetSymbolTint(BIT_FOUR_SYMBOL, IsBitActive(3) ? colorOn : colorOff);
		}
		if (outputNetwork != null)
		{
			num++;
			kbac.SetSymbolTint(OUTPUT_SYMBOL, LogicCircuitNetwork.IsBitActive(0, GetOutputValue()) ? colorOn : colorOff);
		}
		kbac.Play(num + "_" + (GetBitSelection() + 1));
	}
}
