using TUNING;
using UnityEngine;

public class LeadSuitLocker : StateMachineComponent<LeadSuitLocker.StatesInstance>
{
	public class States : GameStateMachine<States, StatesInstance, LeadSuitLocker>
	{
		public class ChargingStates : State
		{
			public State notoperational;

			public State operational;
		}

		public State empty;

		public ChargingStates charging;

		public State charged;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.Update("RefreshMeter", delegate(StatesInstance smi, float dt)
			{
				smi.master.RefreshMeter();
			}, UpdateRate.RENDER_200ms);
			empty.EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => smi.master.GetStoredOutfit() != null);
			charging.DefaultState(charging.notoperational).EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.master.GetStoredOutfit() == null).Transition(charged, (StatesInstance smi) => smi.master.IsSuitFullyCharged());
			charging.notoperational.TagTransition(GameTags.Operational, charging.operational);
			charging.operational.TagTransition(GameTags.Operational, charging.notoperational, on_remove: true).Update("FillBattery", delegate(StatesInstance smi, float dt)
			{
				smi.master.FillBattery(dt);
			}, UpdateRate.SIM_1000ms);
			charged.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.master.GetStoredOutfit() == null);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, LeadSuitLocker, object>.GameInstance
	{
		public StatesInstance(LeadSuitLocker lead_suit_locker)
			: base(lead_suit_locker)
		{
		}
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SuitLocker suit_locker;

	[MyCmpReq]
	private KBatchedAnimController anim_controller;

	private MeterController o2_meter;

	private MeterController battery_meter;

	private float batteryChargeTime = 60f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		o2_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target_top", "meter_oxygen", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, "meter_target_top");
		battery_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target_side", "meter_petrol", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, "meter_target_side");
		base.smi.StartSM();
	}

	public bool IsSuitFullyCharged()
	{
		return suit_locker.IsSuitFullyCharged();
	}

	public KPrefabID GetStoredOutfit()
	{
		return suit_locker.GetStoredOutfit();
	}

	private void FillBattery(float dt)
	{
		KPrefabID storedOutfit = suit_locker.GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			LeadSuitTank component = storedOutfit.GetComponent<LeadSuitTank>();
			if (!component.IsFull())
			{
				component.batteryCharge += dt / batteryChargeTime;
			}
		}
	}

	private void RefreshMeter()
	{
		o2_meter.SetPositionPercent(suit_locker.OxygenAvailable);
		battery_meter.SetPositionPercent(suit_locker.BatteryAvailable);
		anim_controller.SetSymbolVisiblity("oxygen_yes_bloom", IsOxygenTankAboveMinimumLevel());
		anim_controller.SetSymbolVisiblity("petrol_yes_bloom", IsBatteryAboveMinimumLevel());
	}

	public bool IsOxygenTankAboveMinimumLevel()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (storedOutfit != null)
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			if (component == null)
			{
				return true;
			}
			return component.PercentFull() >= EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE;
		}
		return false;
	}

	public bool IsBatteryAboveMinimumLevel()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (storedOutfit != null)
		{
			LeadSuitTank component = storedOutfit.GetComponent<LeadSuitTank>();
			if (component == null)
			{
				return true;
			}
			return component.PercentFull() >= EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE;
		}
		return false;
	}
}
