using Klei.AI;

public class RobotBatteryMonitor : GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public class LowBatteryStates : State
	{
		public State lowBattery;

		public State mediumBattery;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			AmountInstance amountInstance = Db.Get().Amounts.InternalBattery.Lookup(base.gameObject);
			amountInstance.value = amountInstance.GetMax();
		}
	}

	public ObjectParameter<Storage> internalStorage = new ObjectParameter<Storage>();

	public LowBatteryStates lowBatteryStates;

	public State scheduledBatteryCharge;

	public State highBattery;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = highBattery;
		lowBatteryStates.ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (Instance data) => true).Enter(delegate
		{
		});
		lowBatteryStates.lowBattery.ToggleStatusItem(Db.Get().RobotStatusItems.LowBattery, (Instance smi) => smi.gameObject).Transition(lowBatteryStates.mediumBattery, ChargeDecent).Exit(delegate
		{
		});
		lowBatteryStates.mediumBattery.Transition(lowBatteryStates.lowBattery, GameStateMachine<RobotBatteryMonitor, Instance, IStateMachineTarget, Def>.Not(ChargeDecent)).Transition(highBattery, ChargeComplete);
		scheduledBatteryCharge.ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (Instance data) => true).Transition(highBattery, GameStateMachine<RobotBatteryMonitor, Instance, IStateMachineTarget, Def>.Not(IsScheduledRecharge));
		highBattery.Transition(lowBatteryStates.lowBattery, NeedsRecharge).Transition(scheduledBatteryCharge, IsScheduledRecharge);
	}

	public static bool NeedsRecharge(Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) <= 0f;
	}

	public static bool IsScheduledRecharge(Instance smi)
	{
		return GameClock.Instance.IsNighttime();
	}

	public static bool ChargeDecent(Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) >= smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.InternalBattery.Id).GetMax() * 0.5f;
	}

	public static bool ChargeComplete(Instance smi)
	{
		return smi.master.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id) >= smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.InternalBattery.Id).GetMax();
	}
}
