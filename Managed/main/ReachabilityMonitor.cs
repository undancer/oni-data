public class ReachabilityMonitor : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>
{
	private class UpdateReachabilityCB : UpdateBucketWithUpdater<Instance>.IUpdater
	{
		public void Update(Instance smi, float dt)
		{
			smi.UpdateReachability();
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Workable workable)
			: base(workable)
		{
			UpdateReachability();
		}

		public void TriggerEvent()
		{
			bool flag = base.sm.isReachable.Get(base.smi);
			Trigger(-1432940121, flag);
		}

		public void UpdateReachability()
		{
			if (!(base.master == null))
			{
				int cell = Grid.PosToCell(base.master);
				base.sm.isReachable.Set(MinionGroupProber.Get().IsAllReachable(cell, base.master.GetOffsets(cell)), base.smi);
			}
		}
	}

	public State reachable;

	public State unreachable;

	public BoolParameter isReachable = new BoolParameter(default_value: false);

	private static UpdateReachabilityCB updateReachabilityCB = new UpdateReachabilityCB();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = unreachable;
		base.serializable = false;
		root.FastUpdate("UpdateReachability", updateReachabilityCB, UpdateRate.SIM_1000ms, load_balance: true);
		reachable.ToggleTag(GameTags.Reachable).Enter("TriggerEvent", delegate(Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition(isReachable, unreachable, GameStateMachine<ReachabilityMonitor, Instance, Workable, object>.IsFalse);
		unreachable.Enter("TriggerEvent", delegate(Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition(isReachable, reachable, GameStateMachine<ReachabilityMonitor, Instance, Workable, object>.IsTrue);
	}
}
