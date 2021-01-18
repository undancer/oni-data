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
				.ParamTransition(should_extend, retracted_pre, GameStateMachine<States, Gantry.Instance, Gantry, object>.IsFalse);
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
			return IsAutomated() ? logic_on : manual_on;
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

	public static CellOffset[] TileOffsets = new CellOffset[2]
	{
		new CellOffset(-2, 1),
		new CellOffset(-1, 1)
	};

	public static CellOffset[] RetractableOffsets = new CellOffset[4]
	{
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(2, 1),
		new CellOffset(3, 1)
	};

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
		int cell = Grid.PosToCell(this);
		PrimaryElement component = GetComponent<PrimaryElement>();
		for (int i = 0; i < TileOffsets.Length; i++)
		{
			CellOffset rotatedOffset = building.GetRotatedOffset(TileOffsets[i]);
			int num = Grid.OffsetCell(cell, rotatedOffset);
			SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature);
			Grid.Objects[num, 1] = base.gameObject;
			Grid.Foundation[num] = true;
			Grid.Objects[num, 9] = base.gameObject;
			Grid.SetSolid(num, solid: true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num] = false;
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
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
		int cell = Grid.PosToCell(this);
		CellOffset[] tileOffsets = TileOffsets;
		foreach (CellOffset offset in tileOffsets)
		{
			CellOffset rotatedOffset = building.GetRotatedOffset(offset);
			int num = Grid.OffsetCell(cell, rotatedOffset);
			SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f);
			Grid.Objects[num, 1] = null;
			Grid.Objects[num, 9] = null;
			Grid.Foundation[num] = false;
			Grid.SetSolid(num, solid: false, CellEventLogger.Instance.SimCellOccupierDestroy);
			Grid.RenderedByWorld[num] = true;
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		CellOffset[] retractableOffsets = RetractableOffsets;
		foreach (CellOffset offset2 in retractableOffsets)
		{
			CellOffset rotatedOffset2 = building.GetRotatedOffset(offset2);
			int num2 = Grid.OffsetCell(cell, rotatedOffset2);
			Grid.FakeFloor[num2] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num2);
		}
		base.OnCleanUp();
	}

	public void SetWalkable(bool active)
	{
		int cell = Grid.PosToCell(this);
		CellOffset[] retractableOffsets = RetractableOffsets;
		foreach (CellOffset offset in retractableOffsets)
		{
			CellOffset rotatedOffset = building.GetRotatedOffset(offset);
			int num = Grid.OffsetCell(cell, rotatedOffset);
			Grid.FakeFloor[num] = active;
			Pathfinding.Instance.AddDirtyNavGridCell(num);
		}
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
		Instance instance = (Instance)data;
		string format = (instance.IsAutomated() ? BUILDING.STATUSITEMS.GANTRY.AUTOMATION_CONTROL : BUILDING.STATUSITEMS.GANTRY.MANUAL_CONTROL);
		string arg = (instance.IsExtended() ? BUILDING.STATUSITEMS.GANTRY.EXTENDED : BUILDING.STATUSITEMS.GANTRY.RETRACTED);
		return string.Format(format, arg);
	}
}
