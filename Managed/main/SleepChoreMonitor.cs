using System.Collections.Generic;
using UnityEngine;

public class SleepChoreMonitor : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private int locatorCell;

		public GameObject locator;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateBed()
		{
			Ownables soleOwner = base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			Assignable assignable = null;
			Assignable assignable2 = soleOwner.GetAssignable(Db.Get().AssignableSlots.MedicalBed);
			if (assignable2 != null && assignable2.CanAutoAssignTo(base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().assignableProxy.Get()))
			{
				assignable = assignable2;
			}
			else
			{
				assignable = soleOwner.GetAssignable(Db.Get().AssignableSlots.Bed);
				if (assignable == null)
				{
					assignable = soleOwner.AutoAssignSlot(Db.Get().AssignableSlots.Bed);
					if (assignable != null)
					{
						GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>().Update();
					}
				}
			}
			base.smi.sm.bed.Set(assignable, base.smi);
		}

		public bool HasSleepUrge()
		{
			return GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Sleep);
		}

		public bool IsBedReachable()
		{
			AssignableReachabilitySensor sensor = GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>();
			if (!sensor.IsReachable(Db.Get().AssignableSlots.Bed))
			{
				return sensor.IsReachable(Db.Get().AssignableSlots.MedicalBed);
			}
			return true;
		}

		public GameObject CreatePassedOutLocator()
		{
			Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(base.master.gameObject);
			safeFloorLocator.effectName = "PassedOutSleep";
			safeFloorLocator.wakeEffects = new List<string>
			{
				"SoreBack"
			};
			safeFloorLocator.stretchOnWake = false;
			return safeFloorLocator.gameObject;
		}

		public GameObject CreateFloorLocator()
		{
			Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(base.master.gameObject);
			safeFloorLocator.effectName = "FloorSleep";
			safeFloorLocator.wakeEffects = new List<string>
			{
				"SoreBack"
			};
			safeFloorLocator.stretchOnWake = false;
			return safeFloorLocator.gameObject;
		}
	}

	public State satisfied;

	public State checkforbed;

	public State passingout;

	public State sleeponfloor;

	public State bedassigned;

	public TargetParameter bed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = SerializeType.Never;
		root.EventHandler(GameHashes.AssignablesChanged, delegate(Instance smi)
		{
			smi.UpdateBed();
		});
		satisfied.EventTransition(GameHashes.AddUrge, checkforbed, (Instance smi) => smi.HasSleepUrge());
		checkforbed.Enter("SetBed", delegate(Instance smi)
		{
			smi.UpdateBed();
			if (smi.GetSMI<StaminaMonitor.Instance>().NeedsToSleep())
			{
				smi.GoTo(passingout);
			}
			else if (bed.Get(smi) == null || !smi.IsBedReachable())
			{
				smi.GoTo(sleeponfloor);
			}
			else
			{
				smi.GoTo(bedassigned);
			}
		});
		passingout.ToggleChore(CreatePassingOutChore, satisfied, satisfied);
		sleeponfloor.EventTransition(GameHashes.AssignablesChanged, checkforbed).EventTransition(GameHashes.AssignableReachabilityChanged, checkforbed, (Instance smi) => smi.IsBedReachable()).ToggleChore(CreateSleepOnFloorChore, satisfied, satisfied);
		bedassigned.ParamTransition(bed, checkforbed, (Instance smi, GameObject p) => p == null).EventTransition(GameHashes.AssignablesChanged, checkforbed).EventTransition(GameHashes.AssignableReachabilityChanged, checkforbed, (Instance smi) => !smi.IsBedReachable())
			.ToggleChore(CreateSleepChore, satisfied, satisfied);
	}

	private Chore CreatePassingOutChore(Instance smi)
	{
		GameObject gameObject = smi.CreatePassedOutLocator();
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, gameObject, bedIsLocator: true, isInterruptable: false);
	}

	private Chore CreateSleepOnFloorChore(Instance smi)
	{
		GameObject gameObject = smi.CreateFloorLocator();
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, gameObject, bedIsLocator: true, isInterruptable: true);
	}

	private Chore CreateSleepChore(Instance smi)
	{
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, bed.Get(smi), bedIsLocator: false, isInterruptable: true);
	}
}
