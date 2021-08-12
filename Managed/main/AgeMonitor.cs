using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class AgeMonitor : GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>
{
	public class Def : BaseDef
	{
		public float maxAgePercentOnSpawn = 0.75f;

		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Age.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance age;

		public Guid oldStatusGuid;

		public float CyclesUntilDeath => age.GetMax() - age.value;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			age = Db.Get().Amounts.Age.Lookup(base.gameObject);
			Subscribe(1119167081, delegate
			{
				RandomizeAge();
			});
		}

		public void RandomizeAge()
		{
			age.value = UnityEngine.Random.value * age.GetMax() * base.def.maxAgePercentOnSpawn;
			AmountInstance amountInstance = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				amountInstance.value = age.value / age.GetMax() * amountInstance.GetMax() * 1.75f;
				amountInstance.value = Mathf.Min(amountInstance.value, amountInstance.GetMax() * 0.9f);
			}
		}
	}

	public State alive;

	public State time_to_die;

	private AttributeModifier aging;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = alive;
		alive.ToggleAttributeModifier("Aging", (Instance smi) => aging).Transition(time_to_die, TimeToDie, UpdateRate.SIM_1000ms).Update(UpdateOldStatusItem, UpdateRate.SIM_1000ms);
		time_to_die.Enter(Die);
		aging = new AttributeModifier(Db.Get().Amounts.Age.deltaAttribute.Id, 0.0016666667f, CREATURES.MODIFIERS.AGE.NAME);
	}

	private static void Die(Instance smi)
	{
		smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Generic);
	}

	private static bool TimeToDie(Instance smi)
	{
		return smi.age.value >= smi.age.GetMax();
	}

	private static void UpdateOldStatusItem(Instance smi, float dt)
	{
		KSelectable component = smi.GetComponent<KSelectable>();
		bool show = smi.age.value > smi.age.GetMax() * 0.9f;
		smi.oldStatusGuid = component.ToggleStatusItem(Db.Get().CreatureStatusItems.Old, smi.oldStatusGuid, show, smi);
	}
}
