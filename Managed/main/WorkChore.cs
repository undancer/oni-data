using System;
using UnityEngine;

public class WorkChore<WorkableType> : Chore<WorkChore<WorkableType>.StatesInstance> where WorkableType : Workable
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, WorkChore<WorkableType>, object>.GameInstance
	{
		private KAnimFile overrideAnims;

		public StatesInstance(WorkChore<WorkableType> master, GameObject workable, KAnimFile override_anims)
			: base(master)
		{
			overrideAnims = override_anims;
			base.sm.workable.Set(workable, base.smi);
		}

		public void EnableAnimOverrides()
		{
			if (overrideAnims != null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).AddAnimOverrides(overrideAnims);
			}
		}

		public void DisableAnimOverrides()
		{
			if (overrideAnims != null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).RemoveAnimOverrides(overrideAnims);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, WorkChore<WorkableType>>
	{
		public ApproachSubState<WorkableType> approach;

		public State work;

		public State success;

		public TargetParameter workable;

		public TargetParameter worker;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(worker);
			approach.InitializeStates(worker, workable, work).Update("CheckOperational", delegate(StatesInstance smi, float dt)
			{
				if (!smi.master.IsOperationalValid())
				{
					smi.StopSM("Building not operational");
				}
			});
			work.Enter(delegate(StatesInstance smi)
			{
				smi.EnableAnimOverrides();
			}).ToggleWork<WorkableType>(workable, success, null, (StatesInstance smi) => smi.master.IsOperationalValid()).Exit(delegate(StatesInstance smi)
			{
				smi.DisableAnimOverrides();
			});
			success.ReturnSuccess();
		}
	}

	public Func<Precondition.Context, bool> preemption_cb;

	public bool onlyWhenOperational
	{
		get;
		private set;
	}

	public override string ToString()
	{
		return "WorkChore<" + typeof(WorkableType).ToString() + ">";
	}

	public WorkChore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, bool allow_in_red_alert = true, ScheduleBlockType schedule_block = null, bool ignore_schedule_block = false, bool only_when_operational = true, KAnimFile override_anims = null, bool is_preemptable = false, bool allow_in_context_menu = true, bool allow_prioritization = true, PriorityScreen.PriorityClass priority_class = PriorityScreen.PriorityClass.basic, int priority_class_value = 5, bool ignore_building_assignment = false, bool add_to_daily_report = true)
		: base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, priority_class, priority_class_value, is_preemptable, allow_in_context_menu, 0, add_to_daily_report, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, override_anims);
		onlyWhenOperational = only_when_operational;
		if (allow_prioritization)
		{
			SetPrioritizable(target.GetComponent<Prioritizable>());
		}
		AddPrecondition(ChorePreconditions.instance.IsNotTransferArm);
		if (!allow_in_red_alert)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		}
		if (schedule_block != null)
		{
			AddPrecondition(ChorePreconditions.instance.IsScheduledTime, schedule_block);
		}
		else if (!ignore_schedule_block)
		{
			AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		}
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, base.smi.sm.workable.Get<WorkableType>(base.smi));
		Operational component = target.GetComponent<Operational>();
		if (only_when_operational && component != null)
		{
			AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
		if (only_when_operational)
		{
			Deconstructable component2 = target.GetComponent<Deconstructable>();
			if (component2 != null)
			{
				AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
			}
			BuildingEnabledButton component3 = target.GetComponent<BuildingEnabledButton>();
			if (component3 != null)
			{
				AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
			}
		}
		if (!ignore_building_assignment && base.smi.sm.workable.Get(base.smi).GetComponent<Assignable>() != null)
		{
			AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, base.smi.sm.workable.Get<Assignable>(base.smi));
		}
		WorkableType val = target as WorkableType;
		if ((UnityEngine.Object)val != (UnityEngine.Object)null)
		{
			if (!string.IsNullOrEmpty(val.requiredSkillPerk))
			{
				HashedString hashedString = val.requiredSkillPerk;
				AddPrecondition(ChorePreconditions.instance.HasSkillPerk, hashedString);
			}
			if (val.requireMinionToWork)
			{
				AddPrecondition(ChorePreconditions.instance.IsMinion);
			}
		}
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.worker.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}

	public bool IsOperationalValid()
	{
		if (onlyWhenOperational)
		{
			Operational component = base.smi.master.GetComponent<Operational>();
			if (component != null && !component.IsOperational)
			{
				return false;
			}
		}
		return true;
	}

	public override bool CanPreempt(Precondition.Context context)
	{
		if (!base.CanPreempt(context))
		{
			return false;
		}
		if (context.chore.driver == null)
		{
			return false;
		}
		if (context.chore.driver == context.consumerState.choreDriver)
		{
			return false;
		}
		Workable workable = base.smi.sm.workable.Get<WorkableType>(base.smi);
		if (workable == null)
		{
			return false;
		}
		if (preemption_cb != null)
		{
			if (!preemption_cb(context))
			{
				return false;
			}
		}
		else
		{
			int num = 4;
			int navigationCost = context.chore.driver.GetComponent<Navigator>().GetNavigationCost(workable);
			if (navigationCost == -1 || navigationCost < num)
			{
				return false;
			}
			int navigationCost2 = context.consumerState.navigator.GetNavigationCost(workable);
			if (navigationCost2 * 2 > navigationCost)
			{
				return false;
			}
		}
		return true;
	}
}
