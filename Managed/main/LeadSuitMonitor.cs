using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeadSuitMonitor : GameStateMachine<LeadSuitMonitor, LeadSuitMonitor.Instance>
{
	public class WearingSuit : State
	{
		public State hasBattery;

		public State noBattery;
	}

	public new class Instance : GameInstance
	{
		public Navigator navigator;

		public LeadSuitTank lead_suit_tank;

		public List<AttributeModifier> noBatteryModifiers = new List<AttributeModifier>();

		public Instance(IStateMachineTarget master, GameObject owner)
			: base(master)
		{
			base.sm.owner.Set(owner, base.smi);
			navigator = owner.GetComponent<Navigator>();
			lead_suit_tank = master.GetComponent<LeadSuitTank>();
			noBatteryModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, -TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.SUIT_OUT_OF_BATTERIES));
			noBatteryModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, 0f - TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.SUIT_OUT_OF_BATTERIES));
		}
	}

	public WearingSuit wearingSuit;

	public TargetParameter owner;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = wearingSuit;
		Target(owner);
		wearingSuit.DefaultState(wearingSuit.hasBattery);
		wearingSuit.hasBattery.Update(CoolSuit).TagTransition(GameTags.SuitBatteryOut, wearingSuit.noBattery);
		wearingSuit.noBattery.Enter(delegate(Instance smi)
		{
			Attributes attributes2 = smi.sm.owner.Get(smi).GetAttributes();
			if (attributes2 != null)
			{
				foreach (AttributeModifier noBatteryModifier in smi.noBatteryModifiers)
				{
					attributes2.Add(noBatteryModifier);
				}
			}
		}).Exit(delegate(Instance smi)
		{
			Attributes attributes = smi.sm.owner.Get(smi).GetAttributes();
			if (attributes != null)
			{
				foreach (AttributeModifier noBatteryModifier2 in smi.noBatteryModifiers)
				{
					attributes.Remove(noBatteryModifier2);
				}
			}
		}).TagTransition(GameTags.SuitBatteryOut, wearingSuit.noBattery, on_remove: true);
	}

	public static void CoolSuit(Instance smi, float dt)
	{
		if (!smi.navigator)
		{
			return;
		}
		GameObject gameObject = smi.sm.owner.Get(smi);
		if ((bool)gameObject && gameObject.GetSMI<ExternalTemperatureMonitor.Instance>().AverageExternalTemperature >= smi.lead_suit_tank.coolingOperationalTemperature)
		{
			smi.lead_suit_tank.batteryCharge -= 1f / smi.lead_suit_tank.batteryDuration * dt;
			if (smi.lead_suit_tank.IsEmpty())
			{
				gameObject.AddTag(GameTags.SuitBatteryOut);
			}
		}
	}
}
