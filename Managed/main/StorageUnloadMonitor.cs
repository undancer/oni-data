public class StorageUnloadMonitor : GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public ObjectParameter<Storage> internalStorage = new ObjectParameter<Storage>();

	public ObjectParameter<Storage> sweepLocker;

	public State notFull;

	public State full;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = notFull;
		notFull.Transition(full, WantsToUnload);
		full.ToggleStatusItem(Db.Get().RobotStatusItems.DustBinFull, (Instance smi) => smi.gameObject).ToggleBehaviour(GameTags.Robots.Behaviours.UnloadBehaviour, (Instance data) => true).Transition(notFull, GameStateMachine<StorageUnloadMonitor, Instance, IStateMachineTarget, Def>.Not(WantsToUnload), UpdateRate.SIM_1000ms)
			.Enter(delegate(Instance smi)
			{
				if (smi.master.gameObject.GetComponents<Storage>()[1].RemainingCapacity() <= 0f)
				{
					smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_full");
				}
			});
	}

	public static bool WantsToUnload(Instance smi)
	{
		Storage storage = smi.sm.sweepLocker.Get(smi);
		if (storage == null)
		{
			return false;
		}
		if (smi.sm.internalStorage.Get(smi) == null)
		{
			return false;
		}
		if (smi.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour))
		{
			return false;
		}
		if (smi.sm.internalStorage.Get(smi).IsFull())
		{
			return true;
		}
		if (storage != null && !smi.sm.internalStorage.Get(smi).IsEmpty() && Grid.PosToCell(storage) == Grid.PosToCell(smi))
		{
			return true;
		}
		return false;
	}
}
