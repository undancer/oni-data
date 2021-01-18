public class StorageLockerSmart : StorageLocker
{
	[MyCmpGet]
	private LogicPorts ports;

	[MyCmpGet]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<StorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<StorageLockerSmart>(delegate(StorageLockerSmart component, object data)
	{
		component.UpdateLogicCircuitCB(data);
	});

	public override float UserMaxCapacity
	{
		get
		{
			return base.UserMaxCapacity;
		}
		set
		{
			base.UserMaxCapacity = value;
			UpdateLogicAndActiveState();
		}
	}

	protected override void OnPrefabInit()
	{
		Initialize(use_logic_meter: true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ports = base.gameObject.GetComponent<LogicPorts>();
		Subscribe(-1697596308, UpdateLogicCircuitCBDelegate);
		Subscribe(-592767678, UpdateLogicCircuitCBDelegate);
		UpdateLogicAndActiveState();
	}

	private void UpdateLogicCircuitCB(object data)
	{
		UpdateLogicAndActiveState();
	}

	private void UpdateLogicAndActiveState()
	{
		bool num = filteredStorage.IsFull();
		bool isOperational = operational.IsOperational;
		bool flag = num && isOperational;
		ports.SendSignal(FilteredStorage.FULL_PORT_ID, flag ? 1 : 0);
		filteredStorage.SetLogicMeter(flag);
		operational.SetActive(isOperational);
	}
}
