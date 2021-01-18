using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicMemory")]
public class LogicMemory : KMonoBehaviour
{
	[MyCmpGet]
	private LogicPorts ports;

	[Serialize]
	private int value;

	private static StatusItem infoStatusItem;

	public static readonly HashedString READ_PORT_ID = new HashedString("LogicMemoryRead");

	public static readonly HashedString SET_PORT_ID = new HashedString("LogicMemorySet");

	public static readonly HashedString RESET_PORT_ID = new HashedString("LogicMemoryReset");

	private static readonly EventSystem.IntraObjectHandler<LogicMemory> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicMemory>(delegate(LogicMemory component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	protected override void OnSpawn()
	{
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("StoredValue", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	public void OnLogicValueChanged(object data)
	{
		if (ports == null || base.gameObject == null || this == null || !(((LogicValueChanged)data).portID != READ_PORT_ID))
		{
			return;
		}
		int inputValue = ports.GetInputValue(SET_PORT_ID);
		int inputValue2 = ports.GetInputValue(RESET_PORT_ID);
		int num = value;
		if (LogicCircuitNetwork.IsBitActive(0, inputValue2))
		{
			num = 0;
		}
		else if (LogicCircuitNetwork.IsBitActive(0, inputValue))
		{
			num = 1;
		}
		if (num != value)
		{
			value = num;
			ports.SendSignal(READ_PORT_ID, value);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.Play(LogicCircuitNetwork.IsBitActive(0, value) ? "on" : "off");
			}
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicMemory logicMemory = (LogicMemory)data;
		int outputValue = logicMemory.ports.GetOutputValue(READ_PORT_ID);
		return string.Format(BUILDINGS.PREFABS.LOGICMEMORY.STATUS_ITEM_VALUE, outputValue);
	}
}
