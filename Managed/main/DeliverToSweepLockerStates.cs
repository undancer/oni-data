using UnityEngine;

public class DeliverToSweepLockerStates : GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Robots.Behaviours.UnloadBehaviour);
		}

		public override void StartSM()
		{
			base.StartSM();
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().RobotStatusItems.UnloadingStorage, base.gameObject);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().RobotStatusItems.UnloadingStorage);
		}
	}

	public State idle;

	public State movingToStorage;

	public State unloading;

	public State lockerFull;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = movingToStorage;
		idle.ScheduleGoTo(1f, movingToStorage);
		movingToStorage.MoveTo((Instance smi) => (!(GetSweepLocker(smi) == null)) ? Grid.PosToCell(GetSweepLocker(smi)) : Grid.InvalidCell, unloading, idle);
		unloading.Enter(delegate(Instance smi)
		{
			Storage sweepLocker = GetSweepLocker(smi);
			if (sweepLocker == null)
			{
				smi.GoTo(behaviourcomplete);
			}
			else
			{
				Storage storage = smi.master.gameObject.GetComponents<Storage>()[1];
				float num = Mathf.Max(0f, Mathf.Min(storage.MassStored(), sweepLocker.RemainingCapacity()));
				for (int num2 = storage.items.Count - 1; num2 >= 0; num2--)
				{
					GameObject gameObject = storage.items[num2];
					if (!(gameObject == null))
					{
						float num3 = Mathf.Min(gameObject.GetComponent<PrimaryElement>().Mass, num);
						if (num3 != 0f)
						{
							storage.Transfer(sweepLocker, gameObject.GetComponent<KPrefabID>().PrefabTag, num3);
						}
						num -= num3;
						if (num <= 0f)
						{
							break;
						}
					}
				}
				smi.master.GetComponent<KBatchedAnimController>().Play("dropoff");
				smi.master.GetComponent<KBatchedAnimController>().FlipX = false;
				sweepLocker.GetComponent<KBatchedAnimController>().Play("dropoff");
				if (storage.MassStored() > 0f)
				{
					smi.ScheduleGoTo(2f, lockerFull);
				}
				else
				{
					smi.ScheduleGoTo(2f, behaviourcomplete);
				}
			}
		});
		lockerFull.PlayAnim("react_bored", KAnim.PlayMode.Once).OnAnimQueueComplete(movingToStorage);
		behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.UnloadBehaviour);
	}

	public Storage GetSweepLocker(Instance smi)
	{
		StorageUnloadMonitor.Instance sMI = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
		return sMI?.sm.sweepLocker.Get(sMI);
	}
}
