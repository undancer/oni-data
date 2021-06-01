public class PoweredActiveTransitionController : GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>
{
	public class Def : BaseDef
	{
		public bool showWorkingStatus = false;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public State off;

	public State on;

	public State on_pre;

	public State on_pst;

	public State working;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on_pre, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		on_pre.PlayAnim("working_pre").OnAnimQueueComplete(on);
		on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, on_pst, (Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, working, (Instance smi) => smi.GetComponent<Operational>().IsActive);
		on_pst.PlayAnim("working_pst").OnAnimQueueComplete(off);
		working.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, on_pst, (Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, on, (Instance smi) => !smi.GetComponent<Operational>().IsActive)
			.Enter(delegate(Instance smi)
			{
				if (smi.def.showWorkingStatus)
				{
					smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Working);
				}
			})
			.Exit(delegate(Instance smi)
			{
				if (smi.def.showWorkingStatus)
				{
					smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Working);
				}
			});
	}
}
