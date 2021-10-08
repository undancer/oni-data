using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicPorts")]
public class LogicPorts : KMonoBehaviour, IGameObjectEffectDescriptor, IRenderEveryTick
{
	[Serializable]
	public struct Port
	{
		public HashedString id;

		public CellOffset cellOffset;

		public string description;

		public string activeDescription;

		public string inactiveDescription;

		public bool requiresConnection;

		public LogicPortSpriteType spriteType;

		public bool displayCustomName;

		public Port(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon, LogicPortSpriteType sprite_type, bool display_custom_name = false)
		{
			this.id = id;
			cellOffset = cell_offset;
			this.description = description;
			this.activeDescription = activeDescription;
			this.inactiveDescription = inactiveDescription;
			requiresConnection = show_wire_missing_icon;
			spriteType = sprite_type;
			displayCustomName = display_custom_name;
		}

		public static Port InputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Input, display_custom_name);
		}

		public static Port OutputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Output, display_custom_name);
		}

		public static Port RibbonInputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonInput, display_custom_name);
		}

		public static Port RibbonOutputPort(HashedString id, CellOffset cell_offset, string description, string activeDescription, string inactiveDescription, bool show_wire_missing_icon = false, bool display_custom_name = false)
		{
			return new Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonOutput, display_custom_name);
		}
	}

	[SerializeField]
	public Port[] outputPortInfo;

	[SerializeField]
	public Port[] inputPortInfo;

	public List<ILogicUIElement> outputPorts;

	public List<ILogicUIElement> inputPorts;

	private int cell = -1;

	private Orientation orientation = Orientation.NumRotations;

	[Serialize]
	private int[] serializedOutputValues;

	private bool isPhysical;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		isPhysical = GetComponent<BuildingComplete>() != null;
		if (!isPhysical && GetComponent<BuildingUnderConstruction>() == null)
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
			OnOverlayChanged(OverlayScreen.Instance.mode);
			CreateVisualizers();
			SimAndRenderScheduler.instance.Add(this);
		}
		else if (isPhysical)
		{
			UpdateMissingWireIcon();
			CreatePhysicalPorts();
		}
		else
		{
			CreateVisualizers();
		}
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
		DestroyVisualizers();
		if (isPhysical)
		{
			DestroyPhysicalPorts();
		}
		base.OnCleanUp();
	}

	public void RenderEveryTick(float dt)
	{
		CreateVisualizers();
	}

	public void HackRefreshVisualizers()
	{
		CreateVisualizers();
	}

	private void CreateVisualizers()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		bool flag = num != cell;
		cell = num;
		if (!flag)
		{
			Rotatable component = GetComponent<Rotatable>();
			if (component != null)
			{
				Orientation orientation = component.GetOrientation();
				flag = orientation != this.orientation;
				this.orientation = orientation;
			}
		}
		if (!flag)
		{
			return;
		}
		DestroyVisualizers();
		if (outputPortInfo != null)
		{
			outputPorts = new List<ILogicUIElement>();
			for (int i = 0; i < outputPortInfo.Length; i++)
			{
				Port port = outputPortInfo[i];
				LogicPortVisualizer logicPortVisualizer = new LogicPortVisualizer(GetActualCell(port.cellOffset), port.spriteType);
				outputPorts.Add(logicPortVisualizer);
				Game.Instance.logicCircuitManager.AddVisElem(logicPortVisualizer);
			}
		}
		if (inputPortInfo != null)
		{
			inputPorts = new List<ILogicUIElement>();
			for (int j = 0; j < inputPortInfo.Length; j++)
			{
				Port port2 = inputPortInfo[j];
				LogicPortVisualizer logicPortVisualizer2 = new LogicPortVisualizer(GetActualCell(port2.cellOffset), port2.spriteType);
				inputPorts.Add(logicPortVisualizer2);
				Game.Instance.logicCircuitManager.AddVisElem(logicPortVisualizer2);
			}
		}
	}

	private void DestroyVisualizers()
	{
		if (outputPorts != null)
		{
			foreach (ILogicUIElement outputPort in outputPorts)
			{
				Game.Instance.logicCircuitManager.RemoveVisElem(outputPort);
			}
		}
		if (inputPorts == null)
		{
			return;
		}
		foreach (ILogicUIElement inputPort in inputPorts)
		{
			Game.Instance.logicCircuitManager.RemoveVisElem(inputPort);
		}
	}

	private void CreatePhysicalPorts(bool forceCreate = false)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num == cell && !forceCreate)
		{
			return;
		}
		cell = num;
		DestroyVisualizers();
		if (outputPortInfo != null)
		{
			outputPorts = new List<ILogicUIElement>();
			for (int i = 0; i < outputPortInfo.Length; i++)
			{
				Port info = outputPortInfo[i];
				LogicEventSender logicEventSender = new LogicEventSender(info.id, GetActualCell(info.cellOffset), delegate(int new_value)
				{
					if (this != null)
					{
						OnLogicValueChanged(info.id, new_value);
					}
				}, OnLogicNetworkConnectionChanged, info.spriteType);
				outputPorts.Add(logicEventSender);
				Game.Instance.logicCircuitManager.AddVisElem(logicEventSender);
				Game.Instance.logicCircuitSystem.AddToNetworks(logicEventSender.GetLogicUICell(), logicEventSender, is_endpoint: true);
			}
			if (serializedOutputValues != null && serializedOutputValues.Length == outputPorts.Count)
			{
				for (int j = 0; j < outputPorts.Count; j++)
				{
					(outputPorts[j] as LogicEventSender).SetValue(serializedOutputValues[j]);
				}
			}
		}
		serializedOutputValues = null;
		if (inputPortInfo == null)
		{
			return;
		}
		inputPorts = new List<ILogicUIElement>();
		for (int k = 0; k < inputPortInfo.Length; k++)
		{
			Port info2 = inputPortInfo[k];
			LogicEventHandler logicEventHandler = new LogicEventHandler(GetActualCell(info2.cellOffset), delegate(int new_value)
			{
				if (this != null)
				{
					OnLogicValueChanged(info2.id, new_value);
				}
			}, OnLogicNetworkConnectionChanged, info2.spriteType);
			inputPorts.Add(logicEventHandler);
			Game.Instance.logicCircuitManager.AddVisElem(logicEventHandler);
			Game.Instance.logicCircuitSystem.AddToNetworks(logicEventHandler.GetLogicUICell(), logicEventHandler, is_endpoint: true);
		}
	}

	private bool ShowMissingWireIcon()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		if (outputPortInfo != null)
		{
			for (int i = 0; i < outputPortInfo.Length; i++)
			{
				Port port = outputPortInfo[i];
				if (port.requiresConnection)
				{
					int portCell = GetPortCell(port.id);
					if (logicCircuitManager.GetNetworkForCell(portCell) == null)
					{
						return true;
					}
				}
			}
		}
		if (inputPortInfo != null)
		{
			for (int j = 0; j < inputPortInfo.Length; j++)
			{
				Port port2 = inputPortInfo[j];
				if (port2.requiresConnection)
				{
					int portCell2 = GetPortCell(port2.id);
					if (logicCircuitManager.GetNetworkForCell(portCell2) == null)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void OnMove()
	{
		DestroyPhysicalPorts();
		CreatePhysicalPorts();
	}

	private void OnLogicNetworkConnectionChanged(int cell, bool connected)
	{
		UpdateMissingWireIcon();
	}

	private void UpdateMissingWireIcon()
	{
		LogicCircuitManager.ToggleNoWireConnected(ShowMissingWireIcon(), base.gameObject);
	}

	private void DestroyPhysicalPorts()
	{
		if (outputPorts != null)
		{
			foreach (ILogicEventSender outputPort in outputPorts)
			{
				Game.Instance.logicCircuitSystem.RemoveFromNetworks(outputPort.GetLogicCell(), outputPort, is_endpoint: true);
			}
		}
		if (inputPorts == null)
		{
			return;
		}
		for (int i = 0; i < inputPorts.Count; i++)
		{
			LogicEventHandler logicEventHandler = inputPorts[i] as LogicEventHandler;
			if (logicEventHandler != null)
			{
				Game.Instance.logicCircuitSystem.RemoveFromNetworks(logicEventHandler.GetLogicCell(), logicEventHandler, is_endpoint: true);
			}
		}
	}

	private void OnLogicValueChanged(HashedString port_id, int new_value)
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

	private int GetActualCell(CellOffset offset)
	{
		Rotatable component = GetComponent<Rotatable>();
		if (component != null)
		{
			offset = component.GetRotatedCellOffset(offset);
		}
		return Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset);
	}

	public bool TryGetPortAtCell(int cell, out Port port, out bool isInput)
	{
		Port[] array = inputPortInfo;
		for (int i = 0; i < array.Length; i++)
		{
			Port port2 = array[i];
			if (GetActualCell(port2.cellOffset) == cell)
			{
				port = port2;
				isInput = true;
				return true;
			}
		}
		array = outputPortInfo;
		for (int i = 0; i < array.Length; i++)
		{
			Port port3 = array[i];
			if (GetActualCell(port3.cellOffset) == cell)
			{
				port = port3;
				isInput = false;
				return true;
			}
		}
		port = default(Port);
		isInput = false;
		return false;
	}

	public void SendSignal(HashedString port_id, int new_value)
	{
		if (outputPortInfo != null && outputPorts == null)
		{
			CreatePhysicalPorts(forceCreate: true);
		}
		foreach (LogicEventSender outputPort in outputPorts)
		{
			if (outputPort.ID == port_id)
			{
				outputPort.SetValue(new_value);
				break;
			}
		}
	}

	public int GetPortCell(HashedString port_id)
	{
		Port[] array = inputPortInfo;
		for (int i = 0; i < array.Length; i++)
		{
			Port port = array[i];
			if (port.id == port_id)
			{
				return GetActualCell(port.cellOffset);
			}
		}
		array = outputPortInfo;
		for (int i = 0; i < array.Length; i++)
		{
			Port port2 = array[i];
			if (port2.id == port_id)
			{
				return GetActualCell(port2.cellOffset);
			}
		}
		return -1;
	}

	public int GetInputValue(HashedString port_id)
	{
		for (int i = 0; i < inputPortInfo.Length; i++)
		{
			if (inputPorts == null)
			{
				break;
			}
			if (inputPortInfo[i].id == port_id)
			{
				return (inputPorts[i] as LogicEventHandler)?.Value ?? 0;
			}
		}
		return 0;
	}

	public int GetOutputValue(HashedString port_id)
	{
		for (int i = 0; i < outputPorts.Count; i++)
		{
			LogicEventSender logicEventSender = outputPorts[i] as LogicEventSender;
			if (logicEventSender == null)
			{
				return 0;
			}
			if (logicEventSender.ID == port_id)
			{
				return logicEventSender.GetLogicValue();
			}
		}
		return 0;
	}

	public bool IsPortConnected(HashedString port_id)
	{
		int portCell = GetPortCell(port_id);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell) != null;
	}

	private void OnOverlayChanged(HashedString mode)
	{
		if (mode == OverlayModes.Logic.ID)
		{
			base.enabled = true;
			CreateVisualizers();
		}
		else
		{
			base.enabled = false;
			DestroyVisualizers();
		}
	}

	public LogicWire.BitDepth GetConnectedWireBitDepth(HashedString port_id)
	{
		LogicWire.BitDepth result = LogicWire.BitDepth.NumRatings;
		int portCell = GetPortCell(port_id);
		GameObject gameObject = Grid.Objects[portCell, 31];
		if (gameObject != null)
		{
			LogicWire component = gameObject.GetComponent<LogicWire>();
			if (component != null)
			{
				result = component.MaxBitDepth;
			}
		}
		return result;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		LogicPorts component = go.GetComponent<LogicPorts>();
		if (component != null)
		{
			if (component.inputPortInfo != null && component.inputPortInfo.Length != 0)
			{
				Descriptor item = new Descriptor(UI.LOGIC_PORTS.INPUT_PORTS, UI.LOGIC_PORTS.INPUT_PORTS_TOOLTIP);
				list.Add(item);
				Port[] array = component.inputPortInfo;
				for (int i = 0; i < array.Length; i++)
				{
					Port port = array[i];
					string tooltip = string.Format(UI.LOGIC_PORTS.INPUT_PORT_TOOLTIP, port.activeDescription, port.inactiveDescription);
					item = new Descriptor(port.description, tooltip);
					item.IncreaseIndent();
					list.Add(item);
				}
			}
			if (component.outputPortInfo != null && component.outputPortInfo.Length != 0)
			{
				Descriptor item2 = new Descriptor(UI.LOGIC_PORTS.OUTPUT_PORTS, UI.LOGIC_PORTS.OUTPUT_PORTS_TOOLTIP);
				list.Add(item2);
				Port[] array = component.outputPortInfo;
				for (int i = 0; i < array.Length; i++)
				{
					Port port2 = array[i];
					string tooltip2 = string.Format(UI.LOGIC_PORTS.OUTPUT_PORT_TOOLTIP, port2.activeDescription, port2.inactiveDescription);
					item2 = new Descriptor(port2.description, tooltip2);
					item2.IncreaseIndent();
					list.Add(item2);
				}
			}
		}
		return list;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (isPhysical && outputPorts != null)
		{
			serializedOutputValues = new int[outputPorts.Count];
			for (int i = 0; i < outputPorts.Count; i++)
			{
				LogicEventSender logicEventSender = outputPorts[i] as LogicEventSender;
				serializedOutputValues[i] = logicEventSender.GetLogicValue();
			}
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		serializedOutputValues = null;
	}
}
