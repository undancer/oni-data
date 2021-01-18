using KSerialization;
using STRINGS;
using UnityEngine;

public class ObjectDispenser : Switch, IUserControlledCapacity
{
	public class States : GameStateMachine<States, Instance, ObjectDispenser>
	{
		public State load_item;

		public State load_item_pst;

		public State drop_item;

		public State idle;

		public BoolParameter should_open;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			base.serializable = SerializeType.Both_DEPRECATED;
			idle.PlayAnim("on").EventHandler(GameHashes.OnStorageChange, delegate(ObjectDispenser.Instance smi)
			{
				smi.UpdateState();
			}).ParamTransition(should_open, drop_item, (ObjectDispenser.Instance smi, bool p) => p && !smi.master.GetComponent<Storage>().IsEmpty());
			load_item.PlayAnim("working_load").OnAnimQueueComplete(load_item_pst);
			load_item_pst.ParamTransition(should_open, idle, (ObjectDispenser.Instance smi, bool p) => !p).ParamTransition(should_open, drop_item, (ObjectDispenser.Instance smi, bool p) => p);
			drop_item.PlayAnim("working_dispense").OnAnimQueueComplete(idle).Exit(delegate(ObjectDispenser.Instance smi)
			{
				smi.master.DropHeldItems();
			});
		}
	}

	public class Instance : GameStateMachine<States, Instance, ObjectDispenser, object>.GameInstance
	{
		private Operational operational;

		public LogicPorts logic;

		public bool logic_on = true;

		private bool manual_on;

		public bool IsOpened => IsAutomated() ? logic_on : manual_on;

		public Instance(ObjectDispenser master, bool manual_start_state)
			: base(master)
		{
			manual_on = manual_start_state;
			operational = GetComponent<Operational>();
			logic = GetComponent<LogicPorts>();
			Subscribe(-592767678, OnOperationalChanged);
			Subscribe(-801688580, OnLogicValueChanged);
			base.smi.sm.should_open.Set(value: true, base.smi);
		}

		public void UpdateState()
		{
			base.smi.GoTo(base.sm.load_item);
		}

		public bool IsAutomated()
		{
			return logic.IsPortConnected(PORT_ID);
		}

		public void SetSwitchState(bool on)
		{
			manual_on = on;
			UpdateShouldOpen();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(active);
		}

		private void OnOperationalChanged(object data)
		{
			UpdateShouldOpen();
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (!(logicValueChanged.portID != PORT_ID))
			{
				logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
				UpdateShouldOpen();
			}
		}

		private void UpdateShouldOpen()
		{
			SetActive(operational.IsOperational);
			if (operational.IsOperational)
			{
				if (IsAutomated())
				{
					base.smi.sm.should_open.Set(logic_on, base.smi);
				}
				else
				{
					base.smi.sm.should_open.Set(manual_on, base.smi);
				}
			}
		}
	}

	public static readonly HashedString PORT_ID = "ObjectDispenser";

	private LoggerFS log;

	public CellOffset dropOffset;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	private Instance smi;

	private static StatusItem infoStatusItem;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	protected FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<ObjectDispenser> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ObjectDispenser>(delegate(ObjectDispenser component, object data)
	{
		component.OnCopySettings(data);
	});

	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, GetComponent<Storage>().capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			filteredStorage.FilterChanged();
		}
	}

	public float AmountStored => GetComponent<Storage>().MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => GetComponent<Storage>().capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	protected override void OnPrefabInit()
	{
		Initialize();
	}

	protected void Initialize()
	{
		base.OnPrefabInit();
		log = new LoggerFS("ObjectDispenser");
		filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter: false, Db.Get().ChoreTypes.StorageFetch);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new Instance(this, base.IsSwitchedOn);
		smi.StartSM();
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("ObjectDispenserAutomationInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		filteredStorage.FilterChanged();
		GetComponent<KSelectable>().ToggleStatusItem(infoStatusItem, on: true, smi);
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
		base.OnCleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!(gameObject == null))
		{
			ObjectDispenser component = gameObject.GetComponent<ObjectDispenser>();
			if (!(component == null))
			{
				UserMaxCapacity = component.UserMaxCapacity;
			}
		}
	}

	public void DropHeldItems()
	{
		while (storage.Count > 0)
		{
			GameObject gameObject = storage.Drop(storage.items[0]);
			if (rotatable != null)
			{
				gameObject.transform.SetPosition(base.transform.GetPosition() + rotatable.GetRotatedCellOffset(dropOffset).ToVector3());
			}
			else
			{
				gameObject.transform.SetPosition(base.transform.GetPosition() + dropOffset.ToVector3());
			}
		}
		smi.GetMaster().GetComponent<Storage>().DropAll();
	}

	protected override void Toggle()
	{
		base.Toggle();
	}

	protected override void OnRefreshUserMenu(object data)
	{
		if (!smi.IsAutomated())
		{
			base.OnRefreshUserMenu(data);
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		Instance instance = (Instance)data;
		string format = (instance.IsAutomated() ? BUILDING.STATUSITEMS.OBJECTDISPENSER.AUTOMATION_CONTROL : BUILDING.STATUSITEMS.OBJECTDISPENSER.MANUAL_CONTROL);
		string arg = (instance.IsOpened ? BUILDING.STATUSITEMS.OBJECTDISPENSER.OPENED : BUILDING.STATUSITEMS.OBJECTDISPENSER.CLOSED);
		return string.Format(format, arg);
	}
}
