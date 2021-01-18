using Klei.AI;

public class RobotBatteryMonitor : GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>
{
	public class Def : BaseDef
	{
		public string batteryAmountId;

		public float lowBatteryWarningPercent;

		public bool canCharge;
	}

	public class DrainingStates : State
	{
		public State highBattery;

		public State lowBattery;
	}

	public class NeedsRechargeStates : State
	{
		public State lowBattery;

		public State mediumBattery;

		public State trickleCharge;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance amountInstance;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			amountInstance = Db.Get().Amounts.Get(def.batteryAmountId).Lookup(base.gameObject);
			amountInstance.SetValue(amountInstance.GetMax());
		}
	}

	public ObjectParameter<Storage> internalStorage = new ObjectParameter<Storage>();

	public NeedsRechargeStates needsRechargeStates;

	public DrainingStates drainingStates;

	public State deadBattery;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = drainingStates;
		drainingStates.DefaultState(drainingStates.highBattery).Transition(deadBattery, BatteryDead).Transition(needsRechargeStates, NeedsRecharge);
		drainingStates.highBattery.Transition(drainingStates.lowBattery, GameStateMachine<RobotBatteryMonitor, Instance, IStateMachineTarget, Def>.Not(ChargeDecent));
		drainingStates.lowBattery.Transition(drainingStates.highBattery, ChargeDecent).ToggleStatusItem((Instance smi) => smi.def.canCharge ? Db.Get().RobotStatusItems.LowBattery : Db.Get().RobotStatusItems.LowBatteryNoCharge, (Instance smi) => smi.gameObject);
		needsRechargeStates.DefaultState(needsRechargeStates.lowBattery).Transition(deadBattery, BatteryDead).Transition(drainingStates, ChargeComplete)
			.ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (Instance smi) => smi.def.canCharge);
		needsRechargeStates.lowBattery.ToggleStatusItem((Instance smi) => smi.def.canCharge ? Db.Get().RobotStatusItems.LowBattery : Db.Get().RobotStatusItems.LowBatteryNoCharge, (Instance smi) => smi.gameObject).Transition(needsRechargeStates.mediumBattery, ChargeDecent);
		needsRechargeStates.mediumBattery.Transition(needsRechargeStates.lowBattery, GameStateMachine<RobotBatteryMonitor, Instance, IStateMachineTarget, Def>.Not(ChargeDecent)).Transition(needsRechargeStates.trickleCharge, ChargeFull);
		needsRechargeStates.trickleCharge.Transition(needsRechargeStates.mediumBattery, GameStateMachine<RobotBatteryMonitor, Instance, IStateMachineTarget, Def>.Not(ChargeFull));
		deadBattery.ToggleStatusItem(Db.Get().RobotStatusItems.DeadBattery, (Instance smi) => smi.gameObject).Enter(delegate(Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.DeadBattery);
		});
	}

	public static bool NeedsRecharge(Instance smi)
	{
		return smi.amountInstance.value <= 0f || GameClock.Instance.IsNighttime();
	}

	public static bool ChargeDecent(Instance smi)
	{
		return smi.amountInstance.value >= smi.amountInstance.GetMax() * smi.def.lowBatteryWarningPercent;
	}

	public static bool ChargeFull(Instance smi)
	{
		return smi.amountInstance.value >= smi.amountInstance.GetMax();
	}

	public static bool ChargeComplete(Instance smi)
	{
		return smi.amountInstance.value >= smi.amountInstance.GetMax() && !GameClock.Instance.IsNighttime();
	}

	public static bool BatteryDead(Instance smi)
	{
		return !smi.def.canCharge && smi.amountInstance.value == 0f;
	}
}
