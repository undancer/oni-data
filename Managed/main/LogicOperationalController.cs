using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LogicOperationalController")]
public class LogicOperationalController : KMonoBehaviour
{
	public static readonly HashedString PORT_ID = "LogicOperational";

	public int unNetworkedValue = 1;

	public static readonly Operational.Flag LogicOperationalFlag = new Operational.Flag("LogicOperational", Operational.Flag.Type.Requirement);

	private static StatusItem infoStatusItem;

	[MyCmpGet]
	public Operational operational;

	private static readonly EventSystem.IntraObjectHandler<LogicOperationalController> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicOperationalController>(delegate(LogicOperationalController component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset)
	{
		return new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(PORT_ID, offset, UI.LOGIC_PORTS.CONTROL_OPERATIONAL, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE)
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		CheckWireState();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private LogicCircuitNetwork CheckWireState()
	{
		LogicCircuitNetwork network = GetNetwork();
		int value = network?.OutputValue ?? unNetworkedValue;
		operational.SetFlag(LogicOperationalFlag, LogicCircuitNetwork.IsBitActive(0, value));
		return network;
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicOperationalController logicOperationalController = (LogicOperationalController)data;
		Operational operational = logicOperationalController.operational;
		return operational.GetFlag(LogicOperationalFlag) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED;
	}

	private void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == PORT_ID)
		{
			LogicCircuitNetwork logicCircuitNetwork = CheckWireState();
			GetComponent<KSelectable>().ToggleStatusItem(infoStatusItem, logicCircuitNetwork != null, this);
		}
	}
}
