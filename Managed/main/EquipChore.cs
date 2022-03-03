using System;

public class EquipChore : Chore<EquipChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EquipChore, object>.GameInstance
	{
		public StatesInstance(EquipChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EquipChore>
	{
		public class Equip : State
		{
			public State pre;

			public State pst;
		}

		public TargetParameter equipper;

		public TargetParameter equippable_source;

		public TargetParameter equippable_result;

		public FloatParameter requested_units;

		public FloatParameter actual_units;

		public FetchSubState fetch;

		public Equip equip;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(equipper);
			root.DoNothing();
			fetch.InitializeStates(equipper, equippable_source, equippable_result, requested_units, actual_units, equip);
			equip.ToggleWork<EquippableWorkable>(equippable_result, null, null, null);
		}
	}

	public EquipChore(IStateMachineTarget equippable)
		: base(Db.Get().ChoreTypes.Equip, equippable, (ChoreProvider)null, run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		base.smi.sm.equippable_source.Set(equippable.gameObject, base.smi);
		base.smi.sm.requested_units.Set(1f, base.smi);
		showAvailabilityInHoverText = false;
		Prioritizable.AddRef(equippable.gameObject);
		Game.Instance.Trigger(1980521255, equippable.gameObject);
		AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, equippable.GetComponent<Assignable>());
		AddPrecondition(ChorePreconditions.instance.CanPickup, equippable.GetComponent<Pickupable>());
	}

	public override void Begin(Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			Debug.LogError("EquipChore null context.consumer");
			return;
		}
		if (base.smi == null)
		{
			Debug.LogError("EquipChore null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			Debug.LogError("EquipChore null smi.sm");
			return;
		}
		if (base.smi.sm.equippable_source == null)
		{
			Debug.LogError("EquipChore null smi.sm.equippable_source");
			return;
		}
		base.smi.sm.equipper.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
