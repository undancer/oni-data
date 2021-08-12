using System;
using UnityEngine;

public class BeIncapacitatedChore : Chore<BeIncapacitatedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BeIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(BeIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BeIncapacitatedChore>
	{
		public class IncapacitatedStates : State
		{
			public State lookingForBed;

			public BeingRescued rescue;

			public State death;

			public State recovering;
		}

		public class BeingRescued : State
		{
			public State waitingForPickup;

			public State carried;
		}

		public IncapacitatedStates incapacitation_root;

		public TargetParameter clinic;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleAnims("anim_incapacitated_kanim").ToggleStatusItem(Db.Get().DuplicantStatusItems.Incapacitated, (StatesInstance smi) => smi.master.gameObject.GetSMI<IncapacitationMonitor.Instance>()).Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.GoTo(incapacitation_root.lookingForBed);
			});
			incapacitation_root.EventHandler(GameHashes.Died, delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("died");
			});
			incapacitation_root.lookingForBed.Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				smi.master.FindAvailableMedicalBed(smi.master.GetComponent<Navigator>());
			}, UpdateRate.SIM_1000ms).Enter("PlayAnim", delegate(StatesInstance smi)
			{
				smi.sm.clinic.Set(null, smi);
				smi.Play(IncapacitatedDuplicantAnim_pre);
				smi.Queue(IncapacitatedDuplicantAnim_loop, KAnim.PlayMode.Loop);
			});
			incapacitation_root.rescue.ToggleChore((StatesInstance smi) => new RescueIncapacitatedChore(smi.master, masterTarget.Get(smi)), incapacitation_root.recovering, incapacitation_root.lookingForBed);
			incapacitation_root.rescue.waitingForPickup.EventTransition(GameHashes.OnStore, incapacitation_root.rescue.carried).Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				bool flag2 = false;
				if (smi.sm.clinic.Get(smi) == null)
				{
					flag2 = true;
				}
				else if (!smi.master.gameObject.GetComponent<Navigator>().CanReach(clinic.Get(smi).GetComponent<Clinic>()))
				{
					flag2 = true;
				}
				else if (!clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
				{
					flag2 = true;
				}
				if (flag2)
				{
					smi.GoTo(incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms);
			incapacitation_root.rescue.carried.Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				bool flag = false;
				if (smi.sm.clinic.Get(smi) == null)
				{
					flag = true;
				}
				else if (!clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
				{
					flag = true;
				}
				if (flag)
				{
					smi.GoTo(incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms).Enter(delegate(StatesInstance smi)
			{
				smi.Queue(IncapacitatedDuplicantAnim_carry, KAnim.PlayMode.Loop);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.Play(IncapacitatedDuplicantAnim_place);
			});
			incapacitation_root.death.PlayAnim(IncapacitatedDuplicantAnim_death).Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("died");
			});
			incapacitation_root.recovering.ToggleUrge(Db.Get().Urges.HealCritical).Enter(delegate(StatesInstance smi)
			{
				smi.Trigger(-1256572400);
				smi.SetStatus(Status.Success);
				smi.StopSM("recovering");
			});
		}
	}

	private static string IncapacitatedDuplicantAnim_pre = "incapacitate_pre";

	private static string IncapacitatedDuplicantAnim_loop = "incapacitate_loop";

	private static string IncapacitatedDuplicantAnim_death = "incapacitate_death";

	private static string IncapacitatedDuplicantAnim_carry = "carry_loop";

	private static string IncapacitatedDuplicantAnim_place = "place";

	public void FindAvailableMedicalBed(Navigator navigator)
	{
		Clinic clinic = null;
		AssignableSlot clinic2 = Db.Get().AssignableSlots.Clinic;
		Ownables soleOwner = gameObject.GetComponent<MinionIdentity>().GetSoleOwner();
		AssignableSlotInstance slot = soleOwner.GetSlot(clinic2);
		if (slot.assignable == null)
		{
			Assignable assignable = soleOwner.AutoAssignSlot(clinic2);
			if (assignable != null)
			{
				clinic = assignable.GetComponent<Clinic>();
			}
		}
		else
		{
			clinic = slot.assignable.GetComponent<Clinic>();
		}
		if (clinic != null && navigator.CanReach(clinic))
		{
			base.smi.sm.clinic.Set(clinic.gameObject, base.smi);
			base.smi.GoTo(base.smi.sm.incapacitation_root.rescue.waitingForPickup);
		}
	}

	public GameObject GetChosenClinic()
	{
		return base.smi.sm.clinic.Get(base.smi);
	}

	public BeIncapacitatedChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BeIncapacitated, master, master.GetComponent<ChoreProvider>(), run_until_complete: true, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
	}
}
