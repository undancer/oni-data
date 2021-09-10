using STRINGS;

public class Gantry : Switch
{
	public class States : GameStateMachine<States, Instance, Gantry>
	{
		public State retracted_pre;

		public State retracted;

		public State extended_pre;

		public State extended;

		public BoolParameter should_extend;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = extended;
			base.serializable = SerializeType.Both_DEPRECATED;
			retracted_pre.Enter(delegate(Gantry.Instance smi)
			{
				smi.SetActive(active: true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.SetActive(active: false);
			}).PlayAnim("off_pre")
				.OnAnimQueueComplete(retracted);
			retracted.PlayAnim("off").ParamTransition(should_extend, extended_pre, GameStateMachine<States, Gantry.Instance, Gantry, object>.IsTrue);
			extended_pre.Enter(delegate(Gantry.Instance smi)
			{
				smi.SetActive(active: true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.SetActive(active: false);
			}).PlayAnim("on_pre")
				.OnAnimQueueComplete(extended);
			extended.Enter(delegate(Gantry.Instance smi)
			{
				smi.master.SetWalkable(active: true);
			}).Exit(delegate(Gantry.Instance smi)
			{
				smi.master.SetWalkable(active: false);
			}).PlayAnim("on")
				.ParamTransition(should_extend, retracted_pre, GameStateMachine<States, Gantry.Instance, Gantry, object>.IsFalse)
				.ToggleTag(GameTags.GantryExtended);
		}
	}

	public class Instance : GameStateMachine<States, Instance, Gantry, object>.GameInstance
	{
		private Operational operational;

		public LogicPorts logic;

		public bool logic_on = true;

		private bool manual_on;

		public Instance(Gantry master, bool manual_start_state)
			: base(master)
		{
			manual_on = manual_start_state;
			operational = GetComponent<Operational>();
			logic = GetComponent<LogicPorts>();
			Subscribe(-592767678, OnOperationalChanged);
			Subscribe(-801688580, OnLogicValueChanged);
			base.smi.sm.should_extend.Set(value: true, base.smi);
		}

		public bool IsAutomated()
		{
			return logic.IsPortConnected(PORT_ID);
		}

		public bool IsExtended()
		{
			if (!IsAutomated())
			{
				return manual_on;
			}
			return logic_on;
		}

		public void SetSwitchState(bool on)
		{
			manual_on = on;
			UpdateShouldExtend();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active);
		}

		private void OnOperationalChanged(object data)
		{
			UpdateShouldExtend();
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (!(logicValueChanged.portID != PORT_ID))
			{
				logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
				UpdateShouldExtend();
			}
		}

		private void UpdateShouldExtend()
		{
			if (operational.IsOperational)
			{
				if (IsAutomated())
				{
					base.smi.sm.should_extend.Set(logic_on, base.smi);
				}
				else
				{
					base.smi.sm.should_extend.Set(manual_on, base.smi);
				}
			}
		}
	}

	public static readonly HashedString PORT_ID = "Gantry";

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private FakeFloorAdder fakeFloorAdder;

	private Instance smi;

	private static StatusItem infoStatusItem;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("GantryAutomationInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 0.5f;
		smi = new Instance(this, base.IsSwitchedOn);
		smi.StartSM();
		GetComponent<KSelectable>().ToggleStatusItem(infoStatusItem, on: true, smi);
	}

	protected override void OnCleanUp()
	{
		if (smi != null)
		{
			smi.StopSM("cleanup");
		}
		base.OnCleanUp();
	}

	public void SetWalkable(bool active)
	{
		fakeFloorAdder.SetFloor(active);
	}

	protected override void Toggle()
	{
		base.Toggle();
		smi.SetSwitchState(switchedOn);
	}

	protected override void OnRefreshUserMenu(object data)
	{
		if (!smi.IsAutomated())
		{
			base.OnRefreshUserMenu(data);
		}
	}

	protected override void UpdateSwitchStatus()
	{
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		Instance obj = (Instance)data;
		string format = (obj.IsAutomated() ? BUILDING.STATUSITEMS.GANTRY.AUTOMATION_CONTROL : BUILDING.STATUSITEMS.GANTRY.MANUAL_CONTROL);
		string arg = (obj.IsExtended() ? BUILDING.STATUSITEMS.GANTRY.EXTENDED : BUILDING.STATUSITEMS.GANTRY.RETRACTED);
		return string.Format(format, arg);
	}
}
